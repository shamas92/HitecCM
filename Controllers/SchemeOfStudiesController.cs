using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class SchemeOfStudiesController : BaseController
    {
        private readonly Model1 db = new Model1();

        // GET: SchemeOfStudies
        public ActionResult Index(string program, string semester, string code, int? departmentId, int? programId)
        {
            var query = db.tblSchemeOfStudies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(program))
                query = query.Where(s => s.Program.Contains(program));
            if (!string.IsNullOrWhiteSpace(semester))
                query = query.Where(s => s.Semester.Contains(semester));
            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(s => s.Code.Contains(code));
            if (departmentId.HasValue)
                query = query.Where(s => s.DepartmentID == departmentId.Value);
            if (programId.HasValue)
                query = query.Where(s => s.ProgramID == programId.Value);

            var list = query
                .OrderBy(s => s.Program)
                .ThenBy(s => s.Semester)
                .ThenBy(s => s.Code)
                .ToList();

            ViewBag.FilterProgram = program;
            ViewBag.FilterSemester = semester;
            ViewBag.FilterCode = code;
            ViewBag.FilterDepartmentId = departmentId;
            ViewBag.FilterProgramId = programId;
            ViewBag.Departments = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", departmentId);
            ViewBag.Programs = new SelectList(db.tblPrograms.OrderBy(p => p.Title), "ID", "Title", programId);

            return View(list);
        }

        // GET: SchemeOfStudies/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title");
            ViewBag.ProgramID = new SelectList(db.tblPrograms.OrderBy(p => p.Title), "ID", "Title");
            return View(new tblSchemeOfStudy());
        }

        // POST: SchemeOfStudies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentID,ProgramID,Program,Semester,Code,Title,CH")] tblSchemeOfStudy model)
        {
            if (ModelState.IsValid)
            {
                db.tblSchemeOfStudies.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Scheme added successfully.";
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", model.DepartmentID);
            ViewBag.ProgramID = new SelectList(db.tblPrograms.OrderBy(p => p.Title), "ID", "Title", model.ProgramID);
            return View(model);
        }

        // GET: SchemeOfStudies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var entity = db.tblSchemeOfStudies.Find(id);
            if (entity == null)
                return HttpNotFound();

            // Create lists with proper selected values
            var departments = db.tblDepartments.OrderBy(d => d.Title).ToList();
            var programs = db.tblPrograms.OrderBy(p => p.Title).ToList();

            ViewBag.Departments = departments;
            ViewBag.Programs = programs;
            ViewBag.SelectedDepartmentID = entity.DepartmentID;
            ViewBag.SelectedProgramID = entity.ProgramID;

            return View(entity);
        }

        // POST: SchemeOfStudies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DepartmentID,ProgramID,Program,Semester,Code,Title,CH")] tblSchemeOfStudy model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Scheme updated successfully.";
                return RedirectToAction("Index");
            }

            // If validation fails, repopulate dropdowns
            var departments = db.tblDepartments.OrderBy(d => d.Title).ToList();
            var programs = db.tblPrograms.OrderBy(p => p.Title).ToList();

            ViewBag.Departments = departments;
            ViewBag.Programs = programs;
            ViewBag.SelectedDepartmentID = model.DepartmentID;
            ViewBag.SelectedProgramID = model.ProgramID;

            return View(model);
        }

        // POST: SchemeOfStudies/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var entity = db.tblSchemeOfStudies.Find(id);
            if (entity == null)
                return Json(new { success = false, message = "Scheme entry not found." });

            db.tblSchemeOfStudies.Remove(entity);
            db.SaveChanges();
            return Json(new { success = true, message = "Scheme deleted successfully." });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}