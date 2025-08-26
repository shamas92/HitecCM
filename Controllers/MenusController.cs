using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HitecCM.Models;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace HitecCM.Controllers
{
    public class MenusController : Controller
    {
        private Model1 db = new Model1();
        private const string IconDirectory = "~/Content/img/menuicon/";

        // GET: Menus
        public ActionResult Index()
        {
            try
            {
                // First, try a simple query to see if the DbSet is accessible
                var menus = db.tblMenus.ToList();
                
                // If that works, then try with includes
                if (menus != null)
                {
                    var menusWithIncludes = db.tblMenus
                        .Include(m => m.MenuType)
                        .Include(m => m.ParentMenu)
                        .OrderBy(m => m.SortingOrder)
                        .ThenBy(m => m.MenuName)
                        .ToList();
                    return View(menusWithIncludes);
                }
                
                return View(new List<tblMenus>());
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return View(new List<tblMenus>());
            }
        }

        
        // GET: Menus/Create
        public ActionResult Create()
        {
            try
            {
                PopulateDropdowns();
                return View(new tblMenus());
            }
            catch (Exception ex)
            {
                // If there's an error, return a simple view without dropdowns
                ViewBag.ParentMenus = new SelectList(new List<SelectListItem>(), "Value", "Text");
                ViewBag.MenuTypes = new SelectList(new List<SelectListItem>(), "Value", "Text");
                return View(new tblMenus());
            }
        }

        // POST: Menus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MenuName,MenuLink,SortingOrder,ParentMenuID,MenuTypeID")] tblMenus menu, HttpPostedFileBase svgIconFile)
        {
            if (ModelState.IsValid)
            {
                // Handle SVG icon upload if provided
                if (svgIconFile != null && svgIconFile.ContentLength > 0)
                {
                    if (Path.GetExtension(svgIconFile.FileName).ToLower() == ".svg")
                    {
                        string fileName = Path.GetFileName(svgIconFile.FileName);
                        string filePath = Server.MapPath(IconDirectory + fileName);
                        
                        // Save the file to the menuicon directory
                        svgIconFile.SaveAs(filePath);
                        
                        // Store the filename in the SvgIcon property
                        menu.SvgIcon = fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("svgIconFile", "Only SVG files are allowed.");
                        PopulateDropdowns(menu);
                        return View(menu);
                    }
                }

                db.tblMenus.Add(menu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDropdowns(menu);
            return View(menu);
        }

        // GET: Menus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                tblMenus menu = db.tblMenus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                PopulateDropdowns(menu);
                return View(menu);
            }
            catch (Exception ex)
            {
                // If there's an error, return a simple view without dropdowns
                ViewBag.ParentMenus = new SelectList(new List<SelectListItem>(), "Value", "Text");
                ViewBag.MenuTypes = new SelectList(new List<SelectListItem>(), "Value", "Text");
                return View(new tblMenus());
            }
        }

        // POST: Menus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MenuID,MenuName,MenuLink,SortingOrder,ParentMenuID,MenuTypeID,SvgIcon")] tblMenus menu, HttpPostedFileBase svgIconFile)
        {
            if (ModelState.IsValid)
            {
                // Handle SVG icon upload if provided
                if (svgIconFile != null && svgIconFile.ContentLength > 0)
                {
                    if (Path.GetExtension(svgIconFile.FileName).ToLower() == ".svg")
                    {
                        string fileName = Path.GetFileName(svgIconFile.FileName);
                        string filePath = Server.MapPath(IconDirectory + fileName);
                        
                        // Save the file to the menuicon directory
                        svgIconFile.SaveAs(filePath);
                        
                        // Store the filename in the SvgIcon property
                        menu.SvgIcon = fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("svgIconFile", "Only SVG files are allowed.");
                        PopulateDropdowns(menu);
                        return View(menu);
                    }
                }

                db.Entry(menu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDropdowns(menu);
            return View(menu);
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblMenus menu = db.tblMenus
                .Include(m => m.MenuType)
                .Include(m => m.ParentMenu)
                .FirstOrDefault(m => m.MenuID == id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblMenus menu = db.tblMenus.Find(id);
            if (menu == null)
            {
                return Json(new { success = false, message = "Menu not found." });
            }

            // Check if this menu has child menus
            var childMenus = db.tblMenus.Where(m => m.ParentMenuID == id).ToList();
            if (childMenus.Any())
            {
                return Json(new { success = false, message = "Cannot delete menu because it has child menus. Please delete child menus first." });
            }

            // Delete associated SVG icon file if it exists
            if (!string.IsNullOrEmpty(menu.SvgIcon))
            {
                string filePath = Server.MapPath(IconDirectory + menu.SvgIcon);
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception but continue with deletion of menu record
                    }
                }
            }

            db.tblMenus.Remove(menu);
            db.SaveChanges();
            return Json(new { success = true });
        }

        private void PopulateDropdowns(tblMenus menu = null)
        {
            try
            {
                // Find the "main menu" type
                var mainMenuType = db.tblMenuTypes.FirstOrDefault(mt => mt.MenuTypeName.ToLower() == "main menu");
                
                // Populate Parent Menus dropdown (excluding current menu if editing)
                // Only show menus that are of "main menu" type
                var allMenus = db.tblMenus.ToList();
                var parentMenus = allMenus
                    .Where(m => (menu == null || m.MenuID != menu.MenuID) && // Exclude current menu from parent options
                               (mainMenuType != null && m.MenuTypeID == mainMenuType.MenuTypeID)) // Only include "main menu" type
                    .OrderBy(m => m.MenuName)
                    .ToList();

                var parentMenuList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "-- Select Parent Menu (Optional) --", Value = "" }
                };

                foreach (var parentMenu in parentMenus)
                {
                    parentMenuList.Add(new SelectListItem
                    {
                        Text = parentMenu.MenuName,
                        Value = parentMenu.MenuID.ToString(),
                        Selected = menu != null && menu.ParentMenuID == parentMenu.MenuID
                    });
                }

                ViewBag.ParentMenus = new SelectList(parentMenuList, "Value", "Text");

                // Populate Menu Types dropdown
                var menuTypes = db.tblMenuTypes.ToList();

                var menuTypeList = new List<SelectListItem>();

                foreach (var menuType in menuTypes)
                {
                    menuTypeList.Add(new SelectListItem
                    {
                        Text = menuType.MenuTypeName,
                        Value = menuType.MenuTypeID.ToString(),
                        Selected = menu != null && menu.MenuTypeID == menuType.MenuTypeID
                    });
                }

                ViewBag.MenuTypes = new SelectList(menuTypeList, "Value", "Text");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                ViewBag.ParentMenus = new SelectList(new List<SelectListItem>(), "Value", "Text");
                ViewBag.MenuTypes = new SelectList(new List<SelectListItem>(), "Value", "Text");
            }
        }

        // Test action to check database connectivity
        public ActionResult Test()
        {
            try
            {
                // Test if the DbSet is accessible
                var count = db.tblMenus.Count();
                return Content($"Database connection successful. tblMenus count: {count}");
            }
            catch (Exception ex)
            {
                return Content($"Database error: {ex.Message}");
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