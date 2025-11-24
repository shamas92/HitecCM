using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;
using System.Collections.Generic; // added

namespace HitecCM.Controllers
{
    public class DepartmentFacilitiesController : BaseController
    {
        private Model1 db = new Model1();
        private const string FacilityImageDir = "~/Content/img/facilities/";
        private static readonly string[] AllowedImageExts = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const int MaxImageBytes = 5 * 1024 * 1024; // 5MB

        private IEnumerable<SelectListItem> GetDepartmentItems(int? selectedId = null)
        {
            try
            {
                var list = db.tblDepartments
                    .OrderBy(d => d.Title)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentID.ToString(),
                        Text = string.IsNullOrWhiteSpace(d.Title) ? ("Department " + d.DepartmentID) : d.Title,
                        Selected = selectedId.HasValue && d.DepartmentID == selectedId.Value
                    })
                    .ToList();
                return list;
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        // GET: DepartmentFacilities
        public ActionResult Index()
        {
            var facilities = db.tblFacilites
                .OrderBy(f => f.C_Order)
                .ToList();
            ViewBag.Departments = db.tblDepartments.ToDictionary(d => d.DepartmentID, d => d.Title);
            return View(facilities);
        }

        // GET: DepartmentFacilities/Create
        public ActionResult Create()
        {
            try
            {
                var departments = db.tblDepartments
                    .OrderBy(d => d.Title)
                    .ToList(); // Force immediate execution

                if (departments == null || !departments.Any())
                {
                    ViewBag.DepartmentItems = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "No departments available" }
                    };
                }
                else
                {
                    ViewBag.DepartmentItems = departments
                        .Select(d => new SelectListItem
                        {
                            Value = d.DepartmentID.ToString(),
                            Text = !string.IsNullOrWhiteSpace(d.Title) ? d.Title : ("Department " + d.DepartmentID)
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading departments: " + ex.Message);
                ViewBag.DepartmentItems = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Error loading departments" }
                };
            }
            
            return View(new tblFacilite { Active = true });
        }

        // POST: DepartmentFacilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "FacilityID,DepartmentID,Title,Details,Active,C_Order")] tblFacilite facility, HttpPostedFileBase bannerFile, string croppedImageData)
        {
            if (ModelState.IsValid)
            {
                // Validate DepartmentID selection explicitly
                if (!facility.DepartmentID.HasValue || !db.tblDepartments.Any(d => d.DepartmentID == facility.DepartmentID.Value))
                {
                    ModelState.AddModelError("DepartmentID", "Please select a valid department.");
                    goto ReturnView; // repopulate dropdown
                }
                try
                {
                    db.tblFacilites.Add(facility);
                    db.SaveChanges();

                    // Process image (cropped has priority)
                    string finalFileName = null;
                    string physicalDir = Server.MapPath(FacilityImageDir);
                    if (!Directory.Exists(physicalDir)) Directory.CreateDirectory(physicalDir);

                    if (!string.IsNullOrEmpty(croppedImageData) && croppedImageData.StartsWith("data:image"))
                    {
                        try
                        {
                            var base64Data = croppedImageData.Split(',')[1];
                            var imageBytes = Convert.FromBase64String(base64Data);
                            if (imageBytes.Length > MaxImageBytes)
                            {
                                ModelState.AddModelError("Banner", "Cropped image exceeds 5MB size limit.");
                                goto ReturnView;
                            }
                            finalFileName = facility.FacilityID + "_FacilityPhoto.jpg"; // force jpg for cropped
                            System.IO.File.WriteAllBytes(Path.Combine(physicalDir, finalFileName), imageBytes);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("Banner", "Error processing cropped image: " + ex.Message);
                            goto ReturnView;
                        }
                    }
                    else if (bannerFile != null && bannerFile.ContentLength > 0)
                    {
                        var ext = Path.GetExtension(bannerFile.FileName).ToLowerInvariant();
                        if (!AllowedImageExts.Contains(ext))
                        {
                            ModelState.AddModelError("Banner", "Invalid image type. Allowed: jpg, jpeg, png, gif, webp.");
                            goto ReturnView;
                        }
                        if (bannerFile.ContentLength > MaxImageBytes)
                        {
                            ModelState.AddModelError("Banner", "Image exceeds 5MB size limit.");
                            goto ReturnView;
                        }
                        finalFileName = facility.FacilityID + "_FacilityPhoto" + ext;
                        bannerFile.SaveAs(Path.Combine(physicalDir, finalFileName));
                    }

                    if (finalFileName != null)
                    {
                        facility.Banner = finalFileName;
                        db.Entry(facility).State = EntityState.Modified; // update banner field
                        db.SaveChanges();
                    }

                    TempData["Success"] = "Facility created successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating facility: " + ex.Message);
                }
            }
ReturnView:
            // Repopulate dropdown on error
            try
            {
                var departments = db.tblDepartments.OrderBy(d => d.Title).ToList();
                ViewBag.DepartmentItems = departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentID.ToString(),
                        Text = !string.IsNullOrWhiteSpace(d.Title) ? d.Title : ("Department " + d.DepartmentID),
                        Selected = facility.DepartmentID.HasValue && d.DepartmentID == facility.DepartmentID.Value
                    })
                    .ToList();
            }
            catch
            {
                ViewBag.DepartmentItems = new List<SelectListItem>();
            }
            return View(facility);
        }

        // GET: DepartmentFacilities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var facility = db.tblFacilites.Find(id);
            if (facility == null) return HttpNotFound();
            
            try
            {
                var departments = db.tblDepartments
                    .OrderBy(d => d.Title)
                    .ToList();

                if (departments == null || !departments.Any())
                {
                    ViewBag.DepartmentItems = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "No departments available" }
                    };
                }
                else
                {
                    ViewBag.DepartmentItems = departments
                        .Select(d => new SelectListItem
                        {
                            Value = d.DepartmentID.ToString(),
                            Text = !string.IsNullOrWhiteSpace(d.Title) ? d.Title : ("Department " + d.DepartmentID),
                            Selected = facility.DepartmentID.HasValue && d.DepartmentID == facility.DepartmentID.Value
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading departments: " + ex.Message);
                ViewBag.DepartmentItems = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Error loading departments" }
                };
            }

            ViewBag.DepartmentTitle = facility.DepartmentID.HasValue ? 
                db.tblDepartments.Where(d => d.DepartmentID == facility.DepartmentID).Select(d => d.Title).FirstOrDefault() : null;
            
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
                if (!facility.DepartmentID.HasValue || !db.tblDepartments.Any(d => d.DepartmentID == facility.DepartmentID.Value))
                {
                    ModelState.AddModelError("DepartmentID", "Please select a valid department.");
                    goto ReturnEdit;
                }
                
                var existing = db.tblFacilites.Find(facility.FacilityID);
                if (existing == null) return HttpNotFound();

                // Update scalar fields
                existing.DepartmentID = facility.DepartmentID;
                existing.Title = facility.Title;
                existing.Details = facility.Details;
                existing.Active = facility.Active;
                existing.C_Order = facility.C_Order;

                string physicalDir = Server.MapPath(FacilityImageDir);
                if (!Directory.Exists(physicalDir)) Directory.CreateDirectory(physicalDir);

                bool newImageProvided = false;
                string newFileName = null;

                if (!string.IsNullOrEmpty(croppedImageData) && croppedImageData.StartsWith("data:image"))
                {
                    try
                    {
                        var base64Data = croppedImageData.Split(',')[1];
                        var imageBytes = Convert.FromBase64String(base64Data);
                        if (imageBytes.Length > MaxImageBytes)
                        {
                            ModelState.AddModelError("Banner", "Cropped image exceeds 5MB size limit.");
                            goto ReturnEdit;
                        }
                        newFileName = existing.FacilityID + "_FacilityPhoto.jpg";
                        newImageProvided = true;
                        DeleteOldFacilityImage(physicalDir, existing.Banner);
                        System.IO.File.WriteAllBytes(Path.Combine(physicalDir, newFileName), imageBytes);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Banner", "Error processing cropped image: " + ex.Message);
                        goto ReturnEdit;
                    }
                }
                else if (bannerFile != null && bannerFile.ContentLength > 0)
                {
                    var ext = Path.GetExtension(bannerFile.FileName).ToLowerInvariant();
                    if (!AllowedImageExts.Contains(ext)) { ModelState.AddModelError("Banner", "Invalid image type. Allowed: jpg, jpeg, png, gif, webp."); goto ReturnEdit; }
                    if (bannerFile.ContentLength > MaxImageBytes) { ModelState.AddModelError("Banner", "Image exceeds 5MB size limit."); goto ReturnEdit; }
                    newFileName = existing.FacilityID + "_FacilityPhoto" + ext;
                    newImageProvided = true;
                    DeleteOldFacilityImage(physicalDir, existing.Banner);
                    bannerFile.SaveAs(Path.Combine(physicalDir, newFileName));
                }

                if (newImageProvided)
                {
                    existing.Banner = newFileName;
                }

                db.SaveChanges();
                TempData["Success"] = "Facility updated successfully.";
                return RedirectToAction("Index");
            }
ReturnEdit:
            // Repopulate dropdown on error
            try
            {
                var departments = db.tblDepartments.OrderBy(d => d.Title).ToList();
                ViewBag.DepartmentItems = departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentID.ToString(),
                        Text = !string.IsNullOrWhiteSpace(d.Title) ? d.Title : ("Department " + d.DepartmentID),
                        Selected = facility.DepartmentID.HasValue && d.DepartmentID == facility.DepartmentID.Value
                    })
                    .ToList();
            }
            catch
            {
                ViewBag.DepartmentItems = new List<SelectListItem>();
            }
            return View(facility);
        }

        // Helper: delete old image
        private void DeleteOldFacilityImage(string physicalDir, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            var oldPath = Path.Combine(physicalDir, fileName);
            if (System.IO.File.Exists(oldPath))
            {
                try { System.IO.File.Delete(oldPath); } catch { }
            }
        }

        // GET: DepartmentFacilities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var facility = db.tblFacilites.FirstOrDefault(f => f.FacilityID == id);
            if (facility == null) return HttpNotFound();
            ViewBag.DepartmentTitle = facility.DepartmentID.HasValue ? db.tblDepartments.Where(d => d.DepartmentID == facility.DepartmentID).Select(d => d.Title).FirstOrDefault() : null;
            return View(facility);
        }

        // POST: DepartmentFacilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var facility = db.tblFacilites.Find(id);
            if (facility == null)
            {
                return Json(new { success = false, message = "Facility not found." });
            }

            // Delete associated image
            var physicalDir = Server.MapPath(FacilityImageDir);
            DeleteOldFacilityImage(physicalDir, facility.Banner);

            db.tblFacilites.Remove(facility);
            db.SaveChanges();
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}