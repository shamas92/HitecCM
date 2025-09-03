using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class HomeController : Controller
    {
        private Model1 db = new Model1();

        public ActionResult Index()
        {
            try
            {
                // Pre-calculate dates to avoid Entity Framework translation issues
                var now = DateTime.Now;
                var currentYear = now.Year;
                var currentMonth = now.Month;
                var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
                var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;
                var threeMonthsAgo = now.AddMonths(-3);

                // Core metrics
                ViewBag.TotalDepartments = db.tblDepartments.Count();
                ViewBag.ActiveEvents = db.tblEvents.Where(e => e.Active == true).Count();
                ViewBag.TotalEvents = db.tblEvents.Count();
                ViewBag.TotalMenus = db.tblMenus.Count();
                ViewBag.TotalFacilities = db.tblFacilites.Count();
                ViewBag.TotalPrograms = db.tblDepartmentPrograms.Count();
                ViewBag.TotalFaculties = db.tblFaculties.Count();
                ViewBag.TotalNews = db.tblNews.Count();

                // Extended metrics for comprehensive dashboard
                ViewBag.TotalDownloads = db.tblDownloads.Count();
                ViewBag.TotalNotices = db.tblNoticeBoards.Count();
                ViewBag.TotalPolicies = db.tblPolicyItems.Count();
                ViewBag.TotalQACItems = db.tblQACs.Count();
                ViewBag.TotalAccreditations = db.tblQACAccreditations.Count();

                // Student services metrics
                ViewBag.HostelFacilities = db.Hostel_Info.Count();
                ViewBag.TransportRoutes = db.Transport_Info.Count();
                ViewBag.SportsFacilities = db.Sport_Info.Count();
                ViewBag.CafeteriaOptions = db.Cafeteria_Info.Count();

                // Employee-related metrics
                ViewBag.TotalEmployees = db.Employee_Appointment_Details.Count();
                ViewBag.ActiveEmployees = db.Employee_Appointment_Details
                    .Where(e => e.ConfirmaQon_Status != null && e.ConfirmaQon_Status.ToLower() == "confirmed").Count();

                // Recent activities
                ViewBag.RecentEvents = db.tblEvents
                    .Where(e => e.Active == true && e.EventDate != null)
                    .OrderByDescending(e => e.EventDate)
                    .Take(5)
                    .Select(e => new {
                        e.Title,
                        e.EventDate,
                        e.Venue,
                        e.Views,
                        e.Banner
                    })
                    .ToList();

                // Upcoming events
                ViewBag.UpcomingEvents = db.tblEvents
                    .Where(e => e.Active == true && e.EventDate > now)
                    .OrderBy(e => e.EventDate)
                    .Take(5)
                    .Select(e => new {
                        e.Title,
                        e.EventDate,
                        e.Venue,
                        e.Banner
                    })
                    .ToList();

                // Department performance
                ViewBag.DepartmentStats = db.tblDepartments
                    .Select(d => new {
                        DepartmentName = d.Title,
                        Code = d.Code,
                        ProgramCount = d.tblDepartmentPrograms.Count(),
                        FacultyCount = d.tblFaculties.Count(),
                        EventCount = d.tblDepartmentPastEvents.Count + d.tblDepartmentUpcomingEvents.Count
                    })
                    .OrderByDescending(d => d.EventCount + d.FacultyCount)
                    .ToList();

                // Monthly event statistics for trends
                ViewBag.MonthlyEventStats = Enumerable.Range(1, 12)
                    .Select(month => new {
                        Month = month,
                        MonthName = new DateTime(currentYear, month, 1).ToString("MMM"),
                        EventCount = db.tblEvents.Where(e => e.EventDate.HasValue && 
                                                       e.EventDate.Value.Month == month && 
                                                       e.EventDate.Value.Year == currentYear).Count(),
                        NewsCount = db.tblNews.Where(n => n.Date.HasValue && 
                                                     n.Date.Value.Month == month && 
                                                     n.Date.Value.Year == currentYear).Count()
                    })
                    .ToList();

                // Popular content by views
                ViewBag.PopularEvents = db.tblEvents
                    .Where(e => e.Active == true && e.Views.HasValue)
                    .OrderByDescending(e => e.Views)
                    .Take(5)
                    .Select(e => new {
                        e.Title,
                        e.Views,
                        e.EventDate,
                        e.Banner
                    })
                    .ToList();

                // Department activity levels
                var departmentActivityData = db.tblDepartments
                    .Select(d => new {
                        d.Title,
                        d.Code,
                        d.DepartmentID,
                        TotalFaculty = d.tblFaculties.Count(),
                        TotalPrograms = d.tblDepartmentPrograms.Count()
                    })
                    .ToList();

                ViewBag.DepartmentActivity = departmentActivityData
                    .Select(d => new {
                        d.Title,
                        d.Code,
                        RecentActivity = db.tblDepartmentUpcomingEvents.Count(ue => ue.DepartmentId == d.DepartmentID) + 
                                       db.tblDepartmentPastEvents.Count(pe => pe.DepartmentId == d.DepartmentID && pe.EventDate > threeMonthsAgo),
                        d.TotalFaculty,
                        d.TotalPrograms
                    })
                    .ToList();

                // Menu usage statistics
                ViewBag.MenuStats = db.tblMenuTypes
                    .ToList()
                    .Select(mt => new {
                        TypeName = mt.MenuTypeName,
                        MenuCount = db.tblMenus.Where(m => m.MenuTypeID == mt.MenuTypeID).Count()
                    })
                    .ToList();

                // Recent news and updates
                ViewBag.RecentNews = db.tblNews
                    .Where(n => n.Active == true)
                    .OrderByDescending(n => n.Date)
                    .Take(5)
                    .Select(n => new {
                        n.Title,
                        n.Date,
                        n.Views,
                        n.Thumbnail
                    })
                    .ToList();

                // Content distribution
                ViewBag.ContentStats = new {
                    NewsItems = db.tblNews.Count(),
                    NoticeItems = db.tblNoticeBoards.Count(),
                    DownloadItems = db.tblDownloads.Count(),
                    PolicyItems = db.tblPolicyItems.Count(),
                    QACItems = db.tblQACs.Count(),
                    AccreditationItems = db.tblQACAccreditations.Count()
                };

                // Quick stats for cards
                ViewBag.QuickStats = new {
                    TotalContent = db.tblNews.Count() + db.tblNoticeBoards.Count() + db.tblDownloads.Count(),
                    TotalQAPolicies = db.tblQACs.Count() + db.tblPolicyItems.Count() + db.tblQACAccreditations.Count(),
                    TotalStudentServices = db.Hostel_Info.Count() + db.Transport_Info.Count() + db.Sport_Info.Count(),
                    TotalAcademicPrograms = db.tblDepartmentPrograms.Count()
                };

                // Growth metrics
                ViewBag.GrowthMetrics = new {
                    EventsThisMonth = db.tblEvents.Count(e => e.Date.HasValue && 
                        e.Date.Value.Month == currentMonth && e.Date.Value.Year == currentYear),
                    EventsLastMonth = db.tblEvents.Count(e => e.Date.HasValue && 
                        e.Date.Value.Month == lastMonth && e.Date.Value.Year == lastMonthYear),
                    NewsThisMonth = db.tblNews.Count(n => n.Date.HasValue && 
                        n.Date.Value.Month == currentMonth && n.Date.Value.Year == currentYear),
                    NewsLastMonth = db.tblNews.Count(n => n.Date.HasValue && 
                        n.Date.Value.Month == lastMonth && n.Date.Value.Year == lastMonthYear)
                };

                // ===================== Web Analytics Section (Sessions & Page Views) =====================
                try
                {
                    var utcNow = DateTime.UtcNow;
                    var from30 = utcNow.Date.AddDays(-29); // last 30 days including today
                    var from24h = utcNow.AddHours(-23); // last 24 hours (hour buckets)

                    // Debug: Check if analytics tables exist and have data
                    int sessionCount = 0;
                    int pageViewCount = 0;
                    try
                    {
                        sessionCount = db.tblVisitSessions.Count();
                        pageViewCount = db.tblPageViews.Count();
                    }
                    catch (Exception dbEx)
                    {
                        // Analytics tables might not exist or have different structure
                        ViewBag.ErrorMessage = $"Analytics tables not available: {dbEx.Message}";
                    }

                    if (sessionCount == 0 || pageViewCount == 0)
                    {
                        // Generate sample analytics data for demonstration
                        var sampleData = GenerateSampleAnalyticsData();
                        ViewBag.AnalyticsSummary = sampleData.Summary;
                        ViewBag.TrafficSeries = sampleData.TrafficSeries;
                        ViewBag.TopPages = sampleData.TopPages;
                        ViewBag.HourlyViews = sampleData.HourlyViews;
                        
                        if (sessionCount == 0 && pageViewCount == 0)
                        {
                            ViewBag.ErrorMessage = "No analytics data available. Displaying sample data for demonstration.";
                        }
                    }
                    else
                    {
                        // Get sessions in window
                        var rawSessions = db.tblVisitSessions
                            .Where(s => s.FirstSeenUtc >= from30 && s.FirstSeenUtc <= utcNow)
                            .Select(s => new { s.VisitSessionId, s.FirstSeenUtc, s.LastSeenUtc, s.PageViews, s.IsBot })
                            .ToList();

                        // Filter bots
                        var userSessions = rawSessions.Where(s => !s.IsBot).ToList();
                        var sessionIds = new HashSet<long>(userSessions.Select(s => (long)s.VisitSessionId));

                        // Page views in window (filter by sessions)
                        var rawPageViews = db.tblPageViews
                            .Where(p => p.ViewUtc >= from30 && p.ViewUtc <= utcNow)
                            .Select(p => new { p.ViewUtc, p.Path, p.VisitSessionId, p.StatusCode })
                            .ToList()
                            .Where(p => sessionIds.Contains(p.VisitSessionId))
                            .ToList();

                        // Aggregate per day
                        var daily = userSessions
                            .GroupBy(s => s.FirstSeenUtc.Date)
                            .Select(g => new { Date = g.Key, Sessions = g.Count(), PageViews = g.Sum(x => x.PageViews) })
                            .ToList();

                        var filledDaily = new List<object>();
                        for (var d = from30.Date; d <= utcNow.Date; d = d.AddDays(1))
                        {
                            var existing = daily.FirstOrDefault(x => x.Date == d);
                            filledDaily.Add(new { Date = d.ToString("yyyy-MM-dd"), Sessions = existing?.Sessions ?? 0, PageViews = existing?.PageViews ?? 0 });
                        }

                        // Top pages (strip query)
                        var topPages = rawPageViews
                            .GroupBy(p => (p.Path ?? string.Empty).Split('?')[0])
                            .Select(g => new { Path = g.Key, Views = g.Count() })
                            .OrderByDescending(g => g.Views)
                            .Take(10)
                            .ToList();

                        // Hourly (last 24h)
                        var last24Views = db.tblPageViews
                            .Where(p => p.ViewUtc >= from24h && p.ViewUtc <= utcNow)
                            .Select(p => p.ViewUtc)
                            .ToList();
                        var hourly = new List<object>();
                        for (int i = 0; i < 24; i++)
                        {
                            var hour = from24h.AddHours(i);
                            var cnt = last24Views.Count(v => v.Year == hour.Year && v.Month == hour.Month && v.Day == hour.Day && v.Hour == hour.Hour);
                            hourly.Add(new { HourLabel = hour.ToString("HH:00"), Views = cnt });
                        }

                        // Summary numbers
                        int totalSessions = userSessions.Count;
                        int totalPageViews = rawPageViews.Count;
                        double avgPagesPerSession = totalSessions > 0 ? (double)totalPageViews / totalSessions : 0d;
                        double bounceRate = totalSessions > 0 ? (double)userSessions.Count(s => s.PageViews <= 1) / totalSessions * 100d : 0d;

                        // Improved average session duration logic:
                        // We trust LastSeenUtc as the last interaction timestamp for the session.
                        // To avoid inflated durations caused by very long-lived sessions (tabs left open),
                        // we cap individual session length at 30 minutes of continuous activity (typical analytics standard).
                        // We also ignore negative or obviously corrupt values.
                        var maxSessionSpan = TimeSpan.FromMinutes(30);
                        var sessionDurations = userSessions.Select(s => {
                            TimeSpan span = TimeSpan.Zero;
                            if (s.LastSeenUtc > s.FirstSeenUtc)
                            {
                                span = s.LastSeenUtc - s.FirstSeenUtc;
                                if (span < TimeSpan.Zero) span = TimeSpan.Zero;
                                if (span > maxSessionSpan) span = maxSessionSpan; // cap
                            }
                            return span.TotalSeconds;
                        }).ToList();
                        double avgSessionDuration = sessionDurations.Count > 0 ? sessionDurations.Average() : 0d;

                        ViewBag.AnalyticsSummary = new {
                            TotalSessions = totalSessions,
                            TotalPageViews = totalPageViews,
                            AvgPagesPerSession = Math.Round(avgPagesPerSession, 2),
                            BounceRatePct = Math.Round(bounceRate, 1),
                            AvgSessionDurationSec = Math.Round(avgSessionDuration, 1)
                        };
                        ViewBag.TrafficSeries = filledDaily; // 30-day trend
                        ViewBag.TopPages = topPages;
                        ViewBag.HourlyViews = hourly;
                    }
                }
                catch (Exception ex)
                {
                    // Generate sample data as fallback
                    var sampleData = GenerateSampleAnalyticsData();
                    ViewBag.AnalyticsSummary = sampleData.Summary;
                    ViewBag.TrafficSeries = sampleData.TrafficSeries;
                    ViewBag.TopPages = sampleData.TopPages;
                    ViewBag.HourlyViews = sampleData.HourlyViews;
                    ViewBag.ErrorMessage = $"Analytics data unavailable. Displaying sample data. Error: {ex.Message}";
                }
                // =========================================================================================

                return View();
            }
            catch (Exception ex)
            {
                // Fallback values in case of database errors
                ViewBag.TotalDepartments = 0;
                ViewBag.ActiveEvents = 0;
                ViewBag.TotalEvents = 0;
                ViewBag.TotalMenus = 0;
                ViewBag.TotalFacilities = 0;
                ViewBag.TotalPrograms = 0;
                ViewBag.TotalFaculties = 0;
                ViewBag.TotalNews = 0;
                ViewBag.TotalDownloads = 0;
                ViewBag.TotalNotices = 0;
                ViewBag.TotalPolicies = 0;
                ViewBag.TotalQACItems = 0;
                ViewBag.TotalAccreditations = 0;
                ViewBag.HostelFacilities = 0;
                ViewBag.TransportRoutes = 0;
                ViewBag.SportsFacilities = 0;
                ViewBag.CafeteriaOptions = 0;
                ViewBag.TotalEmployees = 0;
                ViewBag.ActiveEmployees = 0;

                ViewBag.RecentEvents = new List<object>();
                ViewBag.UpcomingEvents = new List<object>();
                ViewBag.DepartmentStats = new List<object>();
                ViewBag.MonthlyEventStats = new List<object>();
                ViewBag.PopularEvents = new List<object>();
                ViewBag.DepartmentActivity = new List<object>();
                ViewBag.MenuStats = new List<object>();
                ViewBag.RecentNews = new List<object>();

                ViewBag.ContentStats = new {
                    NewsItems = 0,
                    NoticeItems = 0,
                    DownloadItems = 0,
                    PolicyItems = 0,
                    QACItems = 0,
                    AccreditationItems = 0
                };

                ViewBag.QuickStats = new {
                    TotalContent = 0,
                    TotalQAPolicies = 0,
                    TotalStudentServices = 0,
                    TotalAcademicPrograms = 0
                };

                ViewBag.GrowthMetrics = new {
                    EventsThisMonth = 0,
                    EventsLastMonth = 0,
                    NewsThisMonth = 0,
                    NewsLastMonth = 0
                };

                ViewBag.AnalyticsSummary = new { TotalSessions = 0, TotalPageViews = 0, AvgPagesPerSession = 0d, BounceRatePct = 0d, AvgSessionDurationSec = 0d };
                ViewBag.TrafficSeries = new List<object>();
                ViewBag.TopPages = new List<object>();
                ViewBag.HourlyViews = new List<object>();

                ViewBag.ErrorMessage = "Unable to load dashboard data. Please check database connectivity.";
                return View();
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Helper method to generate sample analytics data for demonstration
        private dynamic GenerateSampleAnalyticsData()
        {
            var random = new Random();
            var utcNow = DateTime.UtcNow;
            
            // Generate 30 days of sample traffic data
            var trafficSeries = new List<object>();
            for (int i = 29; i >= 0; i--)
            {
                var date = utcNow.Date.AddDays(-i);
                var sessions = random.Next(15, 85);
                var pageViews = sessions + random.Next(20, 150);
                trafficSeries.Add(new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    Sessions = sessions,
                    PageViews = pageViews
                });
            }
            
            // Generate hourly data for last 24 hours
            var hourlyViews = new List<object>();
            for (int i = 23; i >= 0; i--)
            {
                var hour = utcNow.AddHours(-i);
                var views = random.Next(0, 25);
                hourlyViews.Add(new
                {
                    HourLabel = hour.ToString("HH:00"),
                    Views = views
                });
            }
            
            // Generate top pages
            var topPages = new List<object>
            {
                new { Path = "/", Views = random.Next(150, 300) },
                new { Path = "/Departments/ComputerScience", Views = random.Next(80, 150) },
                new { Path = "/Events", Views = random.Next(60, 120) },
                new { Path = "/Admissions", Views = random.Next(40, 100) },
                new { Path = "/Faculty", Views = random.Next(30, 80) },
                new { Path = "/Programs", Views = random.Next(25, 70) },
                new { Path = "/News", Views = random.Next(20, 60) },
                new { Path = "/Contact", Views = random.Next(15, 50) },
                new { Path = "/About", Views = random.Next(10, 40) },
                new { Path = "/QAC", Views = random.Next(5, 30) }
            };
            
            // Calculate summary statistics
            var totalSessions = trafficSeries.Sum(t => (int)((dynamic)t).Sessions);
            var totalPageViews = trafficSeries.Sum(t => (int)((dynamic)t).PageViews);
            var avgPagesPerSession = totalSessions > 0 ? (double)totalPageViews / totalSessions : 0;
            var bounceRate = random.Next(25, 45); // Random bounce rate between 25-45%
            var avgSessionDuration = random.Next(45, 180); // Random session duration 45-180 seconds
            
            return new
            {
                Summary = new
                {
                    TotalSessions = totalSessions,
                    TotalPageViews = totalPageViews,
                    AvgPagesPerSession = Math.Round(avgPagesPerSession, 2),
                    BounceRatePct = bounceRate,
                    AvgSessionDurationSec = avgSessionDuration
                },
                TrafficSeries = trafficSeries,
                TopPages = topPages,
                HourlyViews = hourlyViews
            };
        }
    }
}