using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Filters
{
    /// <summary>
    /// Action filter to track page views and visit sessions for analytics
    /// </summary>
    public class AnalyticsTrackingAttribute : ActionFilterAttribute
    {
        private const int SESSION_TIMEOUT_MINUTES = 30;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            // Only track successful GET requests
            if (filterContext.HttpContext.Request.HttpMethod != "GET" ||
                !(filterContext.Result is ViewResult) ||
                filterContext.Exception != null)
            {
                return;
            }

            try
            {
                TrackPageView(filterContext.HttpContext);
            }
            catch (Exception)
            {
                // Silently fail - don't break the page if tracking fails
            }
        }

        private void TrackPageView(HttpContextBase context)
        {
            var request = context.Request;
            var path = request.Url?.PathAndQuery ?? "/";
            var userAgent = request.UserAgent ?? string.Empty;
            var ipAddress = GetClientIpAddress(request);
            var utcNow = DateTime.UtcNow;

            // Check if this is a bot
            bool isBot = IsBot(userAgent);

            // Generate session key
            string sessionKey = GetOrCreateSessionKey(context, ipAddress, userAgent);

            using (var db = new Model1())
            {
                // Find or create session
                var sessionKeyHashed = sessionKey; // Already hashed
                var session = db.tblVisitSessions
                    .FirstOrDefault(s => s.SessionKey == sessionKeyHashed);

                bool isNewSession = false;
                if (session == null)
                {
                    // Create new session
                    session = new tblVisitSession
                    {
                        SessionKey = sessionKeyHashed,
                        FirstSeenUtc = utcNow,
                        LastSeenUtc = utcNow,
                        PageViews = 0,
                        IpHash = HashIpAddress(ipAddress),
                        UserAgent = TruncateString(userAgent, 256),
                        ReferrerHost = GetReferrerHost(request),
                        IsBot = isBot,
                        UserId = null // Set if you have user authentication
                    };
                    db.tblVisitSessions.Add(session);
                    isNewSession = true;
                }
                else
                {
                    // Update existing session
                    session.LastSeenUtc = utcNow;
                    session.PageViews++;
                }

                // Save session first to get the ID
                db.SaveChanges();

                // Create page view record
                var pageView = new tblPageView
                {
                    VisitSessionId = session.VisitSessionId,
                    UserId = null, // Set if you have user authentication
                    Path = TruncateString(path, 300),
                    ViewUtc = utcNow,
                    StatusCode = (short)(context.Response.StatusCode)
                };

                db.tblPageViews.Add(pageView);
                db.SaveChanges();
            }
        }

        private string GetOrCreateSessionKey(HttpContextBase context, string ipAddress, string userAgent)
        {
            const string COOKIE_NAME = "_hitecAnalytics";
            
            // Check if session cookie exists
            var cookie = context.Request.Cookies[COOKIE_NAME];
            if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
            {
                return cookie.Value;
            }

            // Generate new session key
            string rawKey = $"{ipAddress}_{userAgent}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";
            string sessionKey = ComputeSha256Hash(rawKey);

            // Set cookie (expires in 30 minutes)
            var newCookie = new HttpCookie(COOKIE_NAME, sessionKey)
            {
                Expires = DateTime.Now.AddMinutes(SESSION_TIMEOUT_MINUTES),
                HttpOnly = true,
                Secure = context.Request.IsSecureConnection
            };
            context.Response.Cookies.Add(newCookie);

            return sessionKey;
        }

        private string GetClientIpAddress(HttpRequestBase request)
        {
            // Try to get real IP from proxy headers
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = request.ServerVariables["HTTP_X_REAL_IP"];
            }
            
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = request.UserHostAddress;
            }

            // Take first IP if multiple
            if (!string.IsNullOrWhiteSpace(ip) && ip.Contains(","))
            {
                ip = ip.Split(',')[0].Trim();
            }

            return ip ?? "0.0.0.0";
        }

        private byte[] HashIpAddress(string ipAddress)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(ipAddress));
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string GetReferrerHost(HttpRequestBase request)
        {
            var referrer = request.UrlReferrer;
            if (referrer != null && !string.IsNullOrWhiteSpace(referrer.Host))
            {
                return TruncateString(referrer.Host, 255);
            }
            return null;
        }

        private bool IsBot(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return false;

            string ua = userAgent.ToLowerInvariant();
            
            string[] botPatterns = new[]
            {
                "bot", "crawler", "spider", "crawling", "slurp", "mediapartners",
                "bingpreview", "lighthouse", "gtmetrix", "pagespeed", "headless",
                "phantomjs", "selenium", "webdriver"
            };

            return botPatterns.Any(pattern => ua.Contains(pattern));
        }

        private string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
