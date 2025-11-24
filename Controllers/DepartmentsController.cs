using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;
using System.Web;

namespace HitecCM.Controllers
{
    public class DepartmentsController : BaseController
    {
        private Model1 db = new Model1();

        // GET: Departments
        public ActionResult Index()
        {
            try
            {
                var departments = db.tblDepartments
                    .OrderBy(d => d.C_Order)
                    .ThenBy(d => d.Title)
                    .ToList();
                return View(departments);
            }
            catch (Exception ex)
            {
                return View(new System.Collections.Generic.List<tblDepartment>());
            }
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View(new tblDepartment());
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Code,Title,Banner,About,HODPhoto,HODMessage,Programs,Contacts,C_Order")] tblDepartment department, HttpPostedFileBase banner_file, HttpPostedFileBase hod_photo_file)
        {
            bool created = false;
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(department.Title))
                {
                    ModelState.AddModelError("Title", "Department title is required.");
                    return View(department);
                }

                var uploadDir = Server.MapPath("~/Content/img/departments/");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                // Save department first to get ID (no filenames yet)
                db.tblDepartments.Add(department);
                db.SaveChanges();
                created = true;

                // Process banner upload (temp save then move to final name)
                if (banner_file != null && banner_file.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var ext = Path.GetExtension(banner_file.FileName)?.ToLower() ?? string.Empty;
                    if (!allowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("", "Banner image must be a valid image file (jpg, jpeg, png, gif, bmp).");
                        // cleanup created record
                        if (created) { try { db.tblDepartments.Remove(department); db.SaveChanges(); } catch { } }
                        return View(department);
                    }
                    if (banner_file.ContentLength > 10 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "Banner image size must be less than 10MB.");
                        if (created) { try { db.tblDepartments.Remove(department); db.SaveChanges(); } catch { } }
                        return View(department);
                    }

                    var tempName = Guid.NewGuid().ToString() + ext;
                    var tempPath = Path.Combine(uploadDir, tempName);
                    banner_file.SaveAs(tempPath);

                    var finalName = $"departmentBanner{department.DepartmentID}" + ext;
                    var finalPath = Path.Combine(uploadDir, finalName);

                    // Remove existing final file if present
                    if (System.IO.File.Exists(finalPath)) System.IO.File.Delete(finalPath);
                    System.IO.File.Move(tempPath, finalPath);

                    department.Banner = finalName;
                }

                // Process HOD photo
                if (hod_photo_file != null && hod_photo_file.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var ext = Path.GetExtension(hod_photo_file.FileName)?.ToLower() ?? string.Empty;
                    if (!allowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("", "HOD photo must be a valid image file (jpg, jpeg, png, gif, bmp).");
                        if (created) { try { db.tblDepartments.Remove(department); db.SaveChanges(); } catch { } }
                        return View(department);
                    }
                    if (hod_photo_file.ContentLength > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "HOD photo size must be less than 5MB.");
                        if (created) { try { db.tblDepartments.Remove(department); db.SaveChanges(); } catch { } }
                        return View(department);
                    }

                    var tempName = Guid.NewGuid().ToString() + ext;
                    var tempPath = Path.Combine(uploadDir, tempName);
                    hod_photo_file.SaveAs(tempPath);

                    var finalName = $"departmentHOD_{department.DepartmentID}" + ext;
                    var finalPath = Path.Combine(uploadDir, finalName);

                    if (System.IO.File.Exists(finalPath)) System.IO.File.Delete(finalPath);
                    System.IO.File.Move(tempPath, finalPath);

                    department.HODPhoto = finalName;
                }

                // Save final filenames
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Department created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // If failure and department record was created, attempt cleanup (do not throw)
                if (created)
                {
                    try { var d = db.tblDepartments.Find(department.DepartmentID); if (d != null) { db.tblDepartments.Remove(d); db.SaveChanges(); } } catch { }
                }
                ModelState.AddModelError("", "Error saving department: " + ex.Message);
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDepartment department = db.tblDepartments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "DepartmentID,Code,Title,Banner,About,HODPhoto,HODMessage,Programs,Contacts,C_Order")] tblDepartment department, HttpPostedFileBase banner_file, HttpPostedFileBase hod_photo_file)
        {
            try
            {
                var existingDepartment = db.tblDepartments.Find(department.DepartmentID);
                if (existingDepartment == null) return HttpNotFound();

                var uploadDir = Server.MapPath("~/Content/img/departments/");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                // Update scalar fields first
                existingDepartment.Code = department.Code;
                existingDepartment.Title = department.Title;
                existingDepartment.About = department.About;
                existingDepartment.HODMessage = department.HODMessage;
                existingDepartment.Programs = department.Programs;
                existingDepartment.Contacts = department.Contacts;
                existingDepartment.C_Order = department.C_Order;

                // Handle banner replacement
                if (banner_file != null && banner_file.ContentLength > 0)
                {
                    var ext = Path.GetExtension(banner_file.FileName)?.ToLower() ?? string.Empty;
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    if (!allowedExtensions.Contains(ext)) { ModelState.AddModelError("", "Banner image must be a valid image file."); return View(existingDepartment); }
                    if (banner_file.ContentLength > 10 * 1024 * 1024) { ModelState.AddModelError("", "Banner image size must be less than 10MB."); return View(existingDepartment); }

                    var tempName = Guid.NewGuid().ToString() + ext;
                    var tempPath = Path.Combine(uploadDir, tempName);
                    banner_file.SaveAs(tempPath);

                    var finalName = $"departmentBanner{existingDepartment.DepartmentID}" + ext;
                    var finalPath = Path.Combine(uploadDir, finalName);

                    if (System.IO.File.Exists(finalPath)) System.IO.File.Delete(finalPath);
                    System.IO.File.Move(tempPath, finalPath);

                    // delete old banner if it had different name
                    if (!string.IsNullOrEmpty(existingDepartment.Banner) && !existingDepartment.Banner.Equals(finalName, StringComparison.OrdinalIgnoreCase))
                    {
                        var oldPath = Path.Combine(uploadDir, existingDepartment.Banner);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    existingDepartment.Banner = finalName;
                }

                // Handle HOD photo replacement
                if (hod_photo_file != null && hod_photo_file.ContentLength > 0)
                {
                    var ext = Path.GetExtension(hod_photo_file.FileName)?.ToLower() ?? string.Empty;
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    if (!allowedExtensions.Contains(ext)) { ModelState.AddModelError("", "HOD photo must be a valid image file."); return View(existingDepartment); }
                    if (hod_photo_file.ContentLength > 5 * 1024 * 1024) { ModelState.AddModelError("", "HOD photo size must be less than 5MB."); return View(existingDepartment); }

                    var tempName = Guid.NewGuid().ToString() + ext;
                    var tempPath = Path.Combine(uploadDir, tempName);
                    hod_photo_file.SaveAs(tempPath);

                    var finalName = $"departmentHOD_{existingDepartment.DepartmentID}" + ext;
                    var finalPath = Path.Combine(uploadDir, finalName);

                    if (System.IO.File.Exists(finalPath)) System.IO.File.Delete(finalPath);
                    System.IO.File.Move(tempPath, finalPath);

                    // delete old hod photo if different
                    if (!string.IsNullOrEmpty(existingDepartment.HODPhoto) && !existingDepartment.HODPhoto.Equals(finalName, StringComparison.OrdinalIgnoreCase))
                    {
                        var oldPath = Path.Combine(uploadDir, existingDepartment.HODPhoto);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    existingDepartment.HODPhoto = finalName;
                }

                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    TempData["Success"] = "Department updated successfully.";
                    return RedirectToAction("Index");
                }

                return View(existingDepartment);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating department: " + ex.Message);
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDepartment department = db.tblDepartments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDepartment department = db.tblDepartments.Find(id);
            if (department != null)
            {
                db.tblDepartments.Remove(department);
                db.SaveChanges();
                TempData["Success"] = "Department deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Helper method to rename uploaded files with proper naming convention
        private void RenameUploadedFiles(tblDepartment department, string uploadDir)
        {
            try
            {
                // Rename banner image if it exists and doesn't already follow the naming convention
                if (!string.IsNullOrEmpty(department.Banner))
                {
                    var bannerPath = Path.Combine(uploadDir, department.Banner);
                    if (System.IO.File.Exists(bannerPath))
                    {
                        var expectedBannerName = $"departmentBanner{department.DepartmentID}";
                        var fileExtension = Path.GetExtension(department.Banner);
                        var newBannerName = expectedBannerName + fileExtension;

                        System.Diagnostics.Debug.WriteLine($"Banner: Current={department.Banner}, Expected={expectedBannerName}, StartsWith={department.Banner.StartsWith(expectedBannerName)}");

                        // Only rename if the current name doesn't match the expected name
                        if (!department.Banner.StartsWith(expectedBannerName))
                        {
                            var newBannerPath = Path.Combine(uploadDir, newBannerName);

                            // Delete any existing file with the target name
                            if (System.IO.File.Exists(newBannerPath))
                            {
                                System.IO.File.Delete(newBannerPath);
                            }

                            // Rename the file
                            System.IO.File.Move(bannerPath, newBannerPath);

                            // Update the database record - reload to avoid conflicts
                            var deptToUpdate = db.tblDepartments.Find(department.DepartmentID);
                            if (deptToUpdate != null)
                            {
                                deptToUpdate.Banner = newBannerName;
                                try
                                {
                                    db.SaveChanges();
                                    System.Diagnostics.Debug.WriteLine($"Banner database updated to: {newBannerName}");
                                }
                                catch (Exception dbEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Banner database update failed: {dbEx.Message}");
                                }
                            }
                        }
                    }
                }

                // Rename HOD photo if it exists and doesn't already follow the naming convention
                if (!string.IsNullOrEmpty(department.HODPhoto))
                {
                    var hodPhotoPath = Path.Combine(uploadDir, department.HODPhoto);
                    System.Diagnostics.Debug.WriteLine($"Checking HOD photo: {hodPhotoPath}, Exists: {System.IO.File.Exists(hodPhotoPath)}");

                    if (System.IO.File.Exists(hodPhotoPath))
                    {
                        var expectedHodName = $"departmentHOD_{department.DepartmentID}";
                        var fileExtension = Path.GetExtension(department.HODPhoto);
                        var newHodName = expectedHodName + fileExtension;

                        System.Diagnostics.Debug.WriteLine($"HOD Photo: Current={department.HODPhoto}, Expected={expectedHodName}, Extension={fileExtension}, NewName={newHodName}");
                        System.Diagnostics.Debug.WriteLine($"StartsWith check: {department.HODPhoto.StartsWith(expectedHodName)}");

                        // Only rename if the current name doesn't match the expected name
                        if (!department.HODPhoto.StartsWith(expectedHodName))
                        {
                            var newHodPath = Path.Combine(uploadDir, newHodName);

                            System.Diagnostics.Debug.WriteLine($"Renaming HOD photo from {hodPhotoPath} to {newHodPath}");

                            // Delete any existing file with the target name
                            if (System.IO.File.Exists(newHodPath))
                            {
                                System.IO.File.Delete(newHodPath);
                                System.Diagnostics.Debug.WriteLine("Deleted existing file with target name");
                            }

                            // Rename the file
                            System.IO.File.Move(hodPhotoPath, newHodPath);
                            System.Diagnostics.Debug.WriteLine("File moved successfully");

                            // Update the database record - reload to avoid conflicts
                            var deptToUpdate = db.tblDepartments.Find(department.DepartmentID);
                            if (deptToUpdate != null)
                            {
                                deptToUpdate.HODPhoto = newHodName;
                                try
                                {
                                    db.SaveChanges();
                                    System.Diagnostics.Debug.WriteLine($"HOD Photo database updated to: {newHodName}");
                                }
                                catch (Exception dbEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Database update failed: {dbEx.Message}");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Could not find department to update in database");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("HOD Photo already has correct name, skipping rename");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"HOD Photo file not found: {hodPhotoPath}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No HOD Photo to rename");
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw it to avoid breaking the main operation
                // In a production app, you would log this to a logging framework
                System.Diagnostics.Debug.WriteLine("Error renaming department files: " + ex.Message);
            }
        }

        // Helper method to rename existing files that don't follow the naming convention
        private void RenameExistingFiles(tblDepartment department, string uploadDir)
        {
            try
            {
                // Check banner image naming
                if (!string.IsNullOrEmpty(department.Banner))
                {
                    var expectedBannerName = $"departmentBanner{department.DepartmentID}";
                    if (!department.Banner.StartsWith(expectedBannerName))
                    {
                        var bannerPath = Path.Combine(uploadDir, department.Banner);
                        if (System.IO.File.Exists(bannerPath))
                        {
                            var fileExtension = Path.GetExtension(department.Banner);
                            var newBannerName = expectedBannerName + fileExtension;
                            var newBannerPath = Path.Combine(uploadDir, newBannerName);

                            // Delete any existing file with the target name
                            if (System.IO.File.Exists(newBannerPath))
                            {
                                System.IO.File.Delete(newBannerPath);
                            }

                            // Rename the file
                            System.IO.File.Move(bannerPath, newBannerPath);

                            // Update the database record
                            var deptToUpdate = db.tblDepartments.Find(department.DepartmentID);
                            if (deptToUpdate != null)
                            {
                                deptToUpdate.Banner = newBannerName;
                                try
                                {
                                    db.SaveChanges();
                                    System.Diagnostics.Debug.WriteLine($"Existing banner database updated to: {newBannerName}");
                                }
                                catch (Exception dbEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Existing banner database update failed: {dbEx.Message}");
                                }
                            }
                        }
                    }
                }

                // Check HOD photo naming
                if (!string.IsNullOrEmpty(department.HODPhoto))
                {
                    var expectedHodName = $"departmentHOD_{department.DepartmentID}";
                    if (!department.HODPhoto.StartsWith(expectedHodName))
                    {
                        var hodPhotoPath = Path.Combine(uploadDir, department.HODPhoto);
                        if (System.IO.File.Exists(hodPhotoPath))
                        {
                            var fileExtension = Path.GetExtension(department.HODPhoto);
                            var newHodName = expectedHodName + fileExtension;
                            var newHodPath = Path.Combine(uploadDir, newHodName);

                            // Delete any existing file with the target name
                            if (System.IO.File.Exists(newHodPath))
                            {
                                System.IO.File.Delete(newHodPath);
                            }

                            // Rename the file
                            System.IO.File.Move(hodPhotoPath, newHodPath);

                            // Update the database record
                            var deptToUpdate = db.tblDepartments.Find(department.DepartmentID);
                            if (deptToUpdate != null)
                            {
                                deptToUpdate.HODPhoto = newHodName;
                                try
                                {
                                    db.SaveChanges();
                                    System.Diagnostics.Debug.WriteLine($"Existing HOD photo database updated to: {newHodName}");
                                }
                                catch (Exception dbEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Existing HOD photo database update failed: {dbEx.Message}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw it to avoid breaking the main operation
                // In a production app, you would log this to a logging framework
                System.Diagnostics.Debug.WriteLine("Error renaming existing department files: " + ex.Message);
            }
        }
    }
}