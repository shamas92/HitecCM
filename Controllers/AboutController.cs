using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;
using System.Web;

namespace HitecCM.Controllers
{
    public class AboutController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new Model1())
            {
                var about = db.tblAbouts.FirstOrDefault();
                if (about == null)
                {
                    about = new tblAbout();
                }
                return View(about);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(tblAbout model, HttpPostedFileBase chairman_image_file, HttpPostedFileBase vc_image_file)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Model1())
                {
                    var about = db.tblAbouts.FirstOrDefault();
                    if (about == null)
                    {
                        about = new tblAbout
                        {
                            CreatedDate = DateTime.Now
                        };
                        db.tblAbouts.Add(about);
                    }
                    about.chairman_message = model.chairman_message;
                    about.vc_message = model.vc_message;
                    about.motto = model.motto;
                    about.vision = model.vision;
                    about.mission = model.mission;
                    about.emblem = model.emblem;

                    // Handle chairman_image upload
                    if (chairman_image_file != null && chairman_image_file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(chairman_image_file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images/about/"), fileName);
                        chairman_image_file.SaveAs(path);
                        about.chairman_image = fileName;
                    }
                    // Handle vc_image upload
                    if (vc_image_file != null && vc_image_file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(vc_image_file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images/about/"), fileName);
                        vc_image_file.SaveAs(path);
                        about.vc_image = fileName;
                    }

                    about.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                }
                TempData["Success"] = "About information updated successfully.";
                return RedirectToAction("Index");
            }
            using (var db = new Model1())
            {
                var about = db.tblAbouts.FirstOrDefault() ?? new tblAbout();
                return View(about);
            }
        }
    }
} 