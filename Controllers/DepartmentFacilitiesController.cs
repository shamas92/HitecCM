using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;

namespace HitecCM.Controllers
{
    public class DepartmentFacilitiesController : BaseController
    {
        private Model1 db = new Model1();

        // GET: DepartmentFacilities
        public ActionResult Index()
        {
            var facilities = db.tblFacilites
                //.Include(f => f.tblDepartment) // Removed: no navigation property
                .OrderBy(f => f.C_Order)
                .ToList();
            ViewBag.Departments = db.tblDepartments.ToDictionary(d => d.DepartmentID, d => d.Title);
            return View(facilities);
        }

        // GET: DepartmentFacilities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFacilite facility = db.tblFacilites
                //.Include(f => f.tblDepartment) // Removed: no navigation property
                .Include(f => f.tblFacilityPhotos)
                .FirstOrDefault(f => f.FacilityID == id);
            if (facility == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentTitle = facility.DepartmentID.HasValue ? db.tblDepartments.Where(d => d.DepartmentID == facility.DepartmentID).Select(d => d.Title).FirstOrDefault() : null;
            return View(facility);
        }

        // GET: DepartmentFacilities/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.tblDepartments, "DepartmentID", "Title");
            return View(new tblFacilite());
        }

        // POST: DepartmentFacilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "FacilityID,DepartmentID,Title,Details,Active,C_Order")] tblFacilite facility, HttpPostedFileBase bannerFile, string croppedImageData)
        {
            if (ModelState.IsValid)
            {
                // Handle cropped image data first
                if (!string.IsNullOrEmpty(croppedImageData) && croppedImageData.StartsWith("data:image"))
                {
                    try
                    {
                        // Extract base64 data
                        string base64Data = croppedImageData.Split(',')[1];
                        byte[] imageBytes = Convert.FromBase64String(base64Data);

                        // Generate filename
                        string fileName = Guid.NewGuid().ToString() + ".jpg";
                        string path = Path.Combine(Server.MapPath("~/Content/img/facilities/"), fileName);
                        Directory.CreateDirectory(Server.MapPath("~/Content/img/facilities/")); // Ensure directory exists

                        // Save cropped image
                        System.IO.File.WriteAllBytes(path, imageBytes);
                        facility.Banner = fileName;
                    }
                    catch (Exception ex)
                    {
                        // Log error and fall back to regular file upload
                        System.Diagnostics.Debug.WriteLine("Error processing cropped image: " + ex.Message);
                    }
                }
                // Fall back to regular file upload if no cropped data
                else if (bannerFile != null && bannerFile.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(bannerFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/img/facilities/"), fileName);
                    Directory.CreateDirectory(Server.MapPath("~/Content/img/facilities/")); // Ensure directory exists
                    bannerFile.SaveAs(path);
                    facility.Banner = fileName;
                }

                db.tblFacilites.Add(facility);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.tblDepartments, "DepartmentID", "Title", facility.DepartmentID);
            return View(facility);
        }

        // GET: DepartmentFacilities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFacilite facility = db.tblFacilites.Find(id);
            if (facility == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentID = new SelectList(db.tblDepartments, "DepartmentID", "Title", facility.DepartmentID);
            ViewBag.DepartmentTitle = facility.DepartmentID.HasValue ? db.tblDepartments.Where(d => d.DepartmentID == facility.DepartmentID).Select(d => d.Title).FirstOrDefault() : null;
            return View(facility);
        }

        // POST: DepartmentFacilities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "FacilityID,DepartmentID,Title,Details,Active,C_Order")] tblFacilite facility, HttpPostedFileBase bannerFile, string croppedImageData)
        {
            if (ModelState.IsValid)
            {
                var existingFacility = db.tblFacilites.Find(facility.FacilityID);
                if (existingFacility == null)
                {
                    return HttpNotFound();
                }

                // Store the original banner filename before updating other properties
                string originalBannerFileName = existingFacility.Banner;

                // Update scalar properties from the posted model to the existing entity
                existingFacility.DepartmentID = facility.DepartmentID;
                existingFacility.Title = facility.Title;
                existingFacility.Details = facility.Details;
                existingFacility.Active = facility.Active;
                existingFacility.C_Order = facility.C_Order;

                // Handle cropped image data first
                if (!string.IsNullOrEmpty(croppedImageData) && croppedImageData.StartsWith("data:image"))
                {
                    try
                    {
                        // Delete old banner if it exists (using the original filename)
                        if (!string.IsNullOrEmpty(originalBannerFileName))
                        {
                            string oldPath = Path.Combine(Server.MapPath("~/Content/img/facilities/"), originalBannerFileName);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // Extract base64 data
                        string base64Data = croppedImageData.Split(',')[1];
                        byte[] imageBytes = Convert.FromBase64String(base64Data);

                        // Generate filename
                        string fileName = Guid.NewGuid().ToString() + ".jpg";
                        string path = Path.Combine(Server.MapPath("~/Content/img/facilities/"), fileName);
                        Directory.CreateDirectory(Server.MapPath("~/Content/img/facilities/")); // Ensure directory exists

                        // Save cropped image
                        System.IO.File.WriteAllBytes(path, imageBytes);
                        existingFacility.Banner = fileName;
                    }
                    catch (Exception ex)
                    {
                        // Log error and fall back to regular file upload
                        System.Diagnostics.Debug.WriteLine("Error processing cropped image: " + ex.Message);
                    }
                }
                // Fall back to regular file upload if no cropped data
                else if (bannerFile != null && bannerFile.ContentLength > 0)
                {
                    // Delete old banner if it exists (using the original filename)
                    if (!string.IsNullOrEmpty(originalBannerFileName))
                    {
                        string oldPath = Path.Combine(Server.MapPath("~/Content/img/facilities/"), originalBannerFileName);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    // Save new banner
                    string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(bannerFile.FileName);
                    string newPath = Path.Combine(Server.MapPath("~/Content/img/facilities/"), newFileName);
                    Directory.CreateDirectory(Server.MapPath("~/Content/img/facilities/")); // Ensure directory exists
                    bannerFile.SaveAs(newPath);
                    existingFacility.Banner = newFileName; // Update banner on existing entity
                }
                // If neither croppedImageData nor bannerFile is provided, existingFacility.Banner is not modified, thus retaining its old value.

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.tblDepartments, "DepartmentID", "Title", facility.DepartmentID);
            return View(facility);
        }

        // GET: DepartmentFacilities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFacilite facility = db.tblFacilites
                //.Include(f => f.tblDepartment) // Removed: no navigation property
                .FirstOrDefault(f => f.FacilityID == id);
            if (facility == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentTitle = facility.DepartmentID.HasValue ? db.tblDepartments.Where(d => d.DepartmentID == facility.DepartmentID).Select(d => d.Title).FirstOrDefault() : null;
            return View(facility);
        }

        // POST: DepartmentFacilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblFacilite facility = db.tblFacilites.Find(id);
            if (facility == null)
            {
                // Optionally return a not found status or error message
                return Json(new { success = false, message = "Facility not found." });
            }
            db.tblFacilites.Remove(facility);
            db.SaveChanges();
            return Json(new { success = true }); // Return JSON success for AJAX
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