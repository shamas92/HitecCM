using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;
using System.Web;

namespace HitecCM.Controllers
{
    public class AdmissionsController : Controller
    {
        // Advertisement action (corresponds to the Advertisement view)
        public ActionResult Advertisement()
        {
            using (var db = new Model1())
            {
                var ad = db.AdverQsements.FirstOrDefault();
                if (ad == null)
                {
                    ad = new AdverQsement();
                }
                return View(ad);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Advertisement(AdverQsement model, HttpPostedFileBase image_file)
        {
            // Remove ImagePath from ModelState validation since we handle it manually
            ModelState.Remove("ImagePath");
            
            try
            {
                using (var db = new Model1())
                {
                    var ad = db.AdverQsements.FirstOrDefault();
                    bool isNewRecord = (ad == null);
                    
                    if (isNewRecord)
                    {
                        ad = new AdverQsement
                        {
                            CreatedAt = DateTime.Now,
                            ImagePath = "" // Initialize to empty string to satisfy Required constraint
                        };
                    }

                    // Handle image upload
                    if (image_file != null && image_file.ContentLength > 0)
                    {
                        // Ensure the upload directory exists
                        var uploadDir = Server.MapPath("~/Content/img/admissions/");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        var fileName = Path.GetFileName(image_file.FileName);
                        var path = Path.Combine(uploadDir, fileName);
                        image_file.SaveAs(path);
                        ad.ImagePath = fileName;
                    }
                    else if (!string.IsNullOrEmpty(model.ImagePath))
                    {
                        ad.ImagePath = model.ImagePath;
                    }
                    else if (isNewRecord)
                    {
                        // For new records, we need at least an empty string due to Required constraint
                        ad.ImagePath = "";
                    }

                    // Copy other properties if they exist
                    if (!string.IsNullOrEmpty(model.DescripQon))
                    {
                        ad.DescripQon = model.DescripQon;
                    }

                    ad.UpdatedAt = DateTime.Now;

                    if (isNewRecord)
                    {
                        db.AdverQsements.Add(ad);
                    }

                    db.SaveChanges();
                    TempData["Success"] = "Advertisement updated successfully.";
                    return RedirectToAction("Advertisement");
                }
            }
            catch (Exception ex)
            {
                // Log the error and show user-friendly message
                TempData["Error"] = "An error occurred while saving the advertisement: " + ex.Message;
                
                // Return the model back to the view with the uploaded file information
                using (var db = new Model1())
                {
                    var ad = db.AdverQsements.FirstOrDefault() ?? new AdverQsement();
                    return View(ad);
                }
            }
        }

        // Academic Calendar action (corresponds to the AcademicCalendar view)
        public ActionResult AcademicCalendar()
        {
            using (var db = new Model1())
            {
                var calendar = db.AcademicCalendars.FirstOrDefault();
                if (calendar == null)
                {
                    calendar = new AcademicCalendar();
                }
                return View(calendar);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AcademicCalendar(AcademicCalendar model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Model1())
                {
                    var calendar = db.AcademicCalendars.FirstOrDefault();
                    if (calendar == null)
                    {
                        calendar = new AcademicCalendar
                        {
                            CreatedAt = DateTime.Now
                        };
                        db.AcademicCalendars.Add(calendar);
                    }
                    calendar.Title = model.Title;
                    calendar.DescripQon = model.DescripQon;
                    calendar.UpdatedAt = DateTime.Now;
                    db.SaveChanges();
                }
                TempData["Success"] = "Academic Calendar updated successfully.";
                return RedirectToAction("AcademicCalendar");
            }
            using (var db = new Model1())
            {
                var calendar = db.AcademicCalendars.FirstOrDefault() ?? new AcademicCalendar();
                return View(calendar);
            }
        }
    }
}