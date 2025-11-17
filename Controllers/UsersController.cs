using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class UsersController : BaseController
    {
        private readonly Model1 db = new Model1();

        // GET: Users
        public ActionResult Index()
        {
            try
            {
                var users = db.tblUsers
                    .OrderBy(u => u.UserName)
                    .ToList();

                return View(users);
            }
            catch (Exception)
            {
                return View(new List<tblUser>());
            }
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            var model = new tblUser
            {
                Active = true
            };
            return View(model);
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Login,Password,UserName,Active")] tblUser model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if login already exists
                    var existingUser = db.tblUsers.FirstOrDefault(u => u.Login == model.Login);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Login", "This login ID already exists.");
                        return View(model);
                    }

                    db.tblUsers.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating user: " + ex.Message);
                }
            }
            return View(model);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.tblUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Login,Password,UserName,Active")] tblUser model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if login already exists for another user
                    var existingUser = db.tblUsers
                        .FirstOrDefault(u => u.Login == model.Login && u.UserID != model.UserID);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Login", "This login ID already exists.");
                        return View(model);
                    }

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating user: " + ex.Message);
                }
            }
            return View(model);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.tblUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var user = db.tblUsers.Find(id);
                if (user == null)
                {
                    if (Request.IsAjaxRequest())
                        return Json(new { success = false, message = "User not found" });
                    return RedirectToAction("Index");
                }

                // Prevent deleting the last admin user
                if (string.Equals(user.Login, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    var adminCount = db.tblUsers.Count(u => string.Equals(u.Login, "admin", StringComparison.OrdinalIgnoreCase));
                    if (adminCount <= 1)
                    {
                        if (Request.IsAjaxRequest())
                            return Json(new { success = false, message = "Cannot delete the last admin user" });
                        
                        ModelState.AddModelError("", "Cannot delete the last admin user.");
                        return View(user);
                    }
                }

                db.tblUsers.Remove(user);
                db.SaveChanges();

                if (Request.IsAjaxRequest())
                    return Json(new { success = true, message = "User deleted successfully" });
                
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Error deleting user: " + ex.Message });
                
                ModelState.AddModelError("", "Error deleting user: " + ex.Message);
                return RedirectToAction("Index");
            }
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
