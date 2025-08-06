using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class MenuTypeController : Controller
    {
        private Model1 db = new Model1();

        // GET: MenuType
        public ActionResult Index()
        {
            var menuTypes = db.tblMenuTypes
                .OrderBy(m => m.MenuTypeID)
                .ToList();
            return View(menuTypes);
        }

        // GET: MenuType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMenuType menuType = db.tblMenuTypes.Find(id);
            if (menuType == null)
            {
                return HttpNotFound();
            }
            return View(menuType);
        }

        // GET: MenuType/Create
        public ActionResult Create()
        {
            return View(new tblMenuType());
        }

        // POST: MenuType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MenuTypeID,MenuTypeName")] tblMenuType menuType)
        {
            if (ModelState.IsValid)
            {
                db.tblMenuTypes.Add(menuType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuType);
        }

        // GET: MenuType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMenuType menuType = db.tblMenuTypes.Find(id);
            if (menuType == null)
            {
                return HttpNotFound();
            }
            return View(menuType);
        }

        // POST: MenuType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MenuTypeID,MenuTypeName")] tblMenuType menuType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuType);
        }

        // GET: MenuType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMenuType menuType = db.tblMenuTypes.Find(id);
            if (menuType == null)
            {
                return HttpNotFound();
            }
            return View(menuType);
        }

        // POST: MenuType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblMenuType menuType = db.tblMenuTypes.Find(id);
            if (menuType == null)
            {
                return Json(new { success = false, message = "Menu Type not found." });
            }
            db.tblMenuTypes.Remove(menuType);
            db.SaveChanges();
            return Json(new { success = true });
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