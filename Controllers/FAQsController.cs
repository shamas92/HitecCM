using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class FAQViewModel
    {
        public int FAQID { get; set; }
        public int? DepartmentID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string DepartmentName { get; set; }
    }

    public class FAQsController : BaseController
    {
        private readonly Model1 db = new Model1();

        // GET: FAQs
        public ActionResult Index(string question, int? departmentId)
        {
            var query = from f in db.tblFAQs
                       join d in db.tblDepartments on f.DepartmentID equals d.DepartmentID into dept
                       from department in dept.DefaultIfEmpty()
                       select new
                       {
                           FAQ = f,
                           DepartmentName = department != null ? department.Title : "All Departments"
                       };

            if (!string.IsNullOrWhiteSpace(question))
                query = query.Where(f => f.FAQ.Question.Contains(question));
            if (departmentId.HasValue)
                query = query.Where(f => f.FAQ.DepartmentID == departmentId.Value);

            var results = query
                .OrderBy(f => f.FAQ.Question)
                .ToList();

            // Create a combined model for the view
            var faqsWithDept = results.Select(r => new FAQViewModel
            {
                FAQID = r.FAQ.FAQID,
                DepartmentID = r.FAQ.DepartmentID,
                Question = r.FAQ.Question,
                Answer = r.FAQ.Answer,
                DepartmentName = r.DepartmentName
            }).ToList();

            ViewBag.FilterQuestion = question;
            ViewBag.FilterDepartmentId = departmentId;
            ViewBag.Departments = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", departmentId);

            return View(faqsWithDept);
        }

        // GET: FAQs/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title");
            return View();
        }

        // POST: FAQs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentID,Question,Answer")] tblFAQ model)
        {
            if (ModelState.IsValid)
            {
                db.tblFAQs.Add(model);
                db.SaveChanges();
                TempData["Success"] = "FAQ added successfully.";
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", model.DepartmentID);
            return View(model);
        }

        // GET: FAQs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var entity = db.tblFAQs.Find(id);
            if (entity == null)
                return HttpNotFound();

            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", entity.DepartmentID);
            return View(entity);
        }

        // POST: FAQs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FAQID,DepartmentID,Question,Answer")] tblFAQ model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "FAQ updated successfully.";
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", model.DepartmentID);
            return View(model);
        }

        // POST: FAQs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var entity = db.tblFAQs.Find(id);
            if (entity == null)
                return Json(new { success = false, message = "FAQ not found." });

            db.tblFAQs.Remove(entity);
            db.SaveChanges();
            return Json(new { success = true, message = "FAQ deleted successfully." });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}