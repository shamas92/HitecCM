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

                // Department activity levels - optimize to avoid complex navigation queries
                var departmentActivityData = db.tblDepartments
                    .Select(d => new {
                        d.Title,
                        d.Code,
                        d.DepartmentID,
                        TotalFaculty = d.tblFaculties.Count(),
                        TotalPrograms = d.tblDepartmentPrograms.Count()
                    })
                    .ToList();

                // Calculate recent activity separately to avoid EF translation issues
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

                // Menu usage statistics - simplified to avoid navigation property issues
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

                // Growth metrics (comparing current month with previous month)
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
    }
}