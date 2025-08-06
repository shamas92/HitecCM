using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;
using System.Web;

namespace HitecCM.Controllers
{
    public class AdvertisementController : Controller
    {
        public ActionResult Index()
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
        public ActionResult Index(AdverQsement model, HttpPostedFileBase image_file)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Model1())
                {
                    var ad = db.AdverQsements.FirstOrDefault();
                    if (ad == null)
                    {
                        ad = new AdverQsement
                        {
                            CreatedAt = DateTime.Now
                        };
                        db.AdverQsements.Add(ad);
                    }
                    // Remove DescripQon logic since it no longer exists

                    // Handle image upload (AboutController logic)
                    if (image_file != null && image_file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(image_file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images/about/"), fileName);
                        image_file.SaveAs(path);
                        ad.ImagePath = fileName;
                    }
                    else if (!string.IsNullOrEmpty(model.ImagePath))
                    {
                        ad.ImagePath = model.ImagePath;
                    }

                    ad.UpdatedAt = DateTime.Now;
                    db.SaveChanges();
                }
                TempData["Success"] = "Advertisement updated successfully.";
                return RedirectToAction("Index");
            }
            using (var db = new Model1())
            {
                var ad = db.AdverQsements.FirstOrDefault() ?? new AdverQsement();
                return View(ad);
            }
        }
    }
} 