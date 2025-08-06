using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class FacultyController : Controller
    {
        public ActionResult VisionMission()
        {
            using (var db = new Model1())
            {
                var facultyET = db.tblFacultiesETs.FirstOrDefault();
                if (facultyET == null)
                {
                    facultyET = new tblFacultiesET();
                }
                return View(facultyET);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult VisionMission(tblFacultiesET model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Model1())
                {
                    var facultyET = db.tblFacultiesETs.FirstOrDefault();
                    if (facultyET == null)
                    {
                        facultyET = new tblFacultiesET
                        {
                            CreatedDate = DateTime.Now
                        };
                        db.tblFacultiesETs.Add(facultyET);
                    }
                    
                    // Update the faculty vision and mission fields
                    facultyET.VisionText = model.VisionText;
                    facultyET.MissionText = model.MissionText;

                    facultyET.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                }
                TempData["Success"] = "Faculty Vision & Mission information updated successfully.";
                return RedirectToAction("VisionMission");
            }
            using (var db = new Model1())
            {
                var facultyET = db.tblFacultiesETs.FirstOrDefault() ?? new tblFacultiesET();
                return View(facultyET);
            }
        }
    }
} 