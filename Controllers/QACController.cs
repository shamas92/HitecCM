using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class QACController : Controller
    {
        private readonly Model1 db = new Model1();

        // Storage paths updated: use Content/img/qac for images and Content/pdf/qac for PDFs
        private const string QacImageDir = "~/Content/img/qac/";
        private const string QacPdfDir = "~/Content/pdf/qac/";

        private string SaveFile(HttpPostedFileBase file, string rootDir)
        {
            if (file == null || file.ContentLength <= 0) return null;
            var dirPath = Server.MapPath(rootDir);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            var fileName = Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(dirPath, fileName);
            file.SaveAs(fullPath);
            return VirtualPathUtility.ToAbsolute(rootDir + fileName);
        }

        // GET/POST: /QAC/About (tblQAC)
        [HttpGet]
        public ActionResult About()
        {
            var model = db.tblQACs.FirstOrDefault() ?? new tblQAC();
            ViewBag.Title = "About QA&C";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult About(tblQAC input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "About QA&C";
                return View(input);
            }

            if (input.Id > 0)
            {
                var entity = db.tblQACs.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                input.UploadedDate = DateTime.Now;
                db.tblQACs.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("About");
        }

        // GET/POST: /QAC/Policy (tblQAPolicy)
        [HttpGet]
        public ActionResult Policy()
        {
            var model = db.tblQAPolicies.FirstOrDefault() ?? new tblQAPolicy();
            ViewBag.Title = "Quality Assurance Policy";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Policy(tblQAPolicy input, HttpPostedFileBase pdfFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Quality Assurance Policy";
                return View(input);
            }

            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQAPolicies.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQAPolicies.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("Policy");
        }

        // GET/POST: /QAC/AnnualActivityCalendar (tblQACAnnualActivityCalender)
        [HttpGet]
        public ActionResult AnnualActivityCalendar()
        {
            var model = db.tblQACAnnualActivityCalenders.FirstOrDefault() ?? new tblQACAnnualActivityCalender();
            ViewBag.Title = "Annual Activity Calendar";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AnnualActivityCalendar(tblQACAnnualActivityCalender input, HttpPostedFileBase pdfFile)
        {
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACAnnualActivityCalenders.Find(input.Id);
                if (entity == null) return HttpNotFound();
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACAnnualActivityCalenders.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("AnnualActivityCalendar");
        }

        // GET/POST: /QAC/Organogram (tblQACOrganogram)
        [HttpGet]
        public ActionResult Organogram()
        {
            var model = db.tblQACOrganograms.FirstOrDefault() ?? new tblQACOrganogram();
            ViewBag.Title = "Organogram";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Organogram(tblQACOrganogram input, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            string imgPath = SaveFile(imageFile, QacImageDir);
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACOrganograms.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (imgPath != null) entity.PageImage = imgPath;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (imgPath != null) input.PageImage = imgPath;
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACOrganograms.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("Organogram");
        }

        // GET/POST: /QAC/IPE (tblQACIPE)
        [HttpGet]
        public ActionResult IPE()
        {
            var model = db.tblQACIPEs.FirstOrDefault() ?? new tblQACIPE();
            ViewBag.Title = "IPE";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult IPE(tblQACIPE input, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            string imgPath = SaveFile(imageFile, QacImageDir);
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACIPEs.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (imgPath != null) entity.PageImage = imgPath;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (imgPath != null) input.PageImage = imgPath;
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACIPEs.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("IPE");
        }

        // GET/POST: /QAC/RIPE (tblQACRIPE)
        [HttpGet]
        public ActionResult RIPE()
        {
            var model = db.tblQACRIPEs.FirstOrDefault() ?? new tblQACRIPE();
            ViewBag.Title = "RIPE";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult RIPE(tblQACRIPE input, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            string imgPath = SaveFile(imageFile, QacImageDir);
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACRIPEs.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (imgPath != null) entity.PageImage = imgPath;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                // Assign a new Id manually since the table doesn't auto-generate Id
                int nextId = 1;
                if (db.tblQACRIPEs.Any())
                {
                    nextId = db.tblQACRIPEs.Max(x => x.Id) + 1;
                }
                input.Id = nextId;

                if (imgPath != null) input.PageImage = imgPath;
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACRIPEs.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("RIPE");
        }

        // GET/POST: /QAC/PGPR (tblQACPGPR)
        [HttpGet]
        public ActionResult PGPR()
        {
            var model = db.tblQACPGPRs.FirstOrDefault() ?? new tblQACPGPR();
            ViewBag.Title = "PGPR";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult PGPR(tblQACPGPR input, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            string imgPath = SaveFile(imageFile, QacImageDir);
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACPGPRs.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (imgPath != null) entity.PageImage = imgPath;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (imgPath != null) input.PageImage = imgPath;
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACPGPRs.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("PGPR");
        }

        // GET/POST: /QAC/SARs (tblQACSAR)
        [HttpGet]
        public ActionResult SARs()
        {
            var model = db.tblQACSARs.FirstOrDefault() ?? new tblQACSAR();
            ViewBag.Title = "SARs";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SARs(tblQACSAR input, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            string imgPath = SaveFile(imageFile, QacImageDir);
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACSARs.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (imgPath != null) entity.PageImage = imgPath;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (imgPath != null) input.PageImage = imgPath;
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACSARs.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("SARs");
        }

        // GET/POST: /QAC/Surveys (tblQACSurvay)
        [HttpGet]
        public ActionResult Surveys()
        {
            var model = db.tblQACSurvays.FirstOrDefault() ?? new tblQACSurvay();
            ViewBag.Title = "Surveys";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Surveys(tblQACSurvay input, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            string imgPath = SaveFile(imageFile, QacImageDir);
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACSurvays.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (imgPath != null) entity.PageImage = imgPath;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (imgPath != null) input.PageImage = imgPath;
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACSurvays.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("Surveys");
        }

        // GET/POST: /QAC/Accreditation (tblQACAccreditation)
        [HttpGet]
        public ActionResult Accreditation()
        {
            var model = db.tblQACAccreditations.FirstOrDefault() ?? new tblQACAccreditation();
            ViewBag.Title = "Accreditation";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Accreditation(tblQACAccreditation input, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            string imgPath = SaveFile(imageFile, QacImageDir);
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACAccreditations.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (imgPath != null) entity.PageImage = imgPath;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (imgPath != null) input.PageImage = imgPath;
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACAccreditations.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("Accreditation");
        }

        // GET/POST: /QAC/Contacts (tblQACContact)
        [HttpGet]
        public ActionResult Contacts()
        {
            var model = db.tblQACContacts.FirstOrDefault() ?? new tblQACContact();
            ViewBag.Title = "Contacts";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Contacts(tblQACContact input, HttpPostedFileBase imageFile, HttpPostedFileBase pdfFile)
        {
            string imgPath = SaveFile(imageFile, QacImageDir);
            string pdfPath = SaveFile(pdfFile, QacPdfDir);

            if (input.Id > 0)
            {
                var entity = db.tblQACContacts.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageText = input.PageText;
                if (imgPath != null) entity.PageImage = imgPath;
                if (pdfPath != null)
                {
                    entity.PdfFileName = Path.GetFileName(pdfPath);
                    entity.PdfFilePath = pdfPath;
                }
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                if (imgPath != null) input.PageImage = imgPath;
                if (pdfPath != null)
                {
                    input.PdfFileName = Path.GetFileName(pdfPath);
                    input.PdfFilePath = pdfPath;
                }
                input.UploadedDate = DateTime.Now;
                db.tblQACContacts.Add(input);
            }
            db.SaveChanges();
            TempData["Success"] = "Saved successfully.";
            return RedirectToAction("Contacts");
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
