using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;
using System.Web;
using System.Data.Entity;

namespace HitecCM.Controllers
{
    public class AdmissionsController : Controller
    {
        // Advertisement action (corresponds to the Advertisement view)
        public ActionResult Advertisement()
        {
            using (var db = new Model1())
            {
                var ad = db.AdverQsements.FirstOrDefault();
                if (ad == null)
                {
                    ad = new AdverQsement();
                }
                return View(ad);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Advertisement(AdverQsement model, HttpPostedFileBase image_file)
        {
            // Remove ImagePath from ModelState validation since we handle it manually
            ModelState.Remove("ImagePath");
            
            try
            {
                using (var db = new Model1())
                {
                    var ad = db.AdverQsements.FirstOrDefault();
                    bool isNewRecord = (ad == null);
                    
                    if (isNewRecord)
                    {
                        ad = new AdverQsement
                        {
                            CreatedAt = DateTime.Now,
                            ImagePath = "" // Initialize to empty string to satisfy Required constraint
                        };
                    }

                    // Handle image upload
                    if (image_file != null && image_file.ContentLength > 0)
                    {
                        // Ensure the upload directory exists
                        var uploadDir = Server.MapPath("~/Content/img/admissions/");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        var fileName = Path.GetFileName(image_file.FileName);
                        var path = Path.Combine(uploadDir, fileName);
                        image_file.SaveAs(path);
                        ad.ImagePath = fileName;
                    }
                    else if (!string.IsNullOrEmpty(model.ImagePath))
                    {
                        ad.ImagePath = model.ImagePath;
                    }
                    else if (isNewRecord)
                    {
                        // For new records, we need at least an empty string due to Required constraint
                        ad.ImagePath = "";
                    }

                    // Copy other properties if they exist
                    if (!string.IsNullOrEmpty(model.DescripQon))
                    {
                        ad.DescripQon = model.DescripQon;
                    }

                    ad.UpdatedAt = DateTime.Now;

                    if (isNewRecord)
                    {
                        db.AdverQsements.Add(ad);
                    }

                    db.SaveChanges();
                    TempData["Success"] = "Advertisement updated successfully.";
                    return RedirectToAction("Advertisement");
                }
            }
            catch (Exception ex)
            {
                // Log the error and show user-friendly message
                TempData["Error"] = "An error occurred while saving the advertisement: " + ex.Message;
                
                // Return the model back to the view with the uploaded file information
                using (var db = new Model1())
                {
                    var ad = db.AdverQsements.FirstOrDefault() ?? new AdverQsement();
                    return View(ad);
                }
            }
        }

        // Academic Calendar action (corresponds to the AcademicCalendar view)
        public ActionResult AcademicCalendar()
        {
            using (var db = new Model1())
            {
                var calendar = db.AcademicCalendars.FirstOrDefault();
                if (calendar == null)
                {
                    calendar = new AcademicCalendar();
                }
                return View(calendar);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AcademicCalendar(AcademicCalendar model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Model1())
                {
                    var calendar = db.AcademicCalendars.FirstOrDefault();
                    if (calendar == null)
                    {
                        calendar = new AcademicCalendar
                        {
                            CreatedAt = DateTime.Now
                        };
                        db.AcademicCalendars.Add(calendar);
                    }
                    calendar.Title = model.Title;
                    calendar.DescripQon = model.DescripQon;
                    calendar.UpdatedAt = DateTime.Now;
                    db.SaveChanges();
                }
                TempData["Success"] = "Academic Calendar updated successfully.";
                return RedirectToAction("AcademicCalendar");
            }
            using (var db = new Model1())
            {
                var calendar = db.AcademicCalendars.FirstOrDefault() ?? new AcademicCalendar();
                return View(calendar);
            }
        }

        // ====== Admissions: Fee Structure ======
        [HttpGet]
        public ActionResult FeeStructure()
        {
            using (var db = new Model1())
            {
                var model = db.tblFeeStructures.FirstOrDefault() ?? new tblFeeStructure
                {
                    Title = "Fee Structure",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Fee Structure";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult FeeStructure(tblFeeStructure input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Fee Structure";
                return View(input);
            }
            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblFeeStructures.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblFeeStructures.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Fee Structure saved successfully.";
            return RedirectToAction("FeeStructure");
        }

        // ====== Admissions: Academic Regulations (single page content) ======
        [HttpGet]
        public ActionResult AcademicRegulations()
        {
            using (var db = new Model1())
            {
                var model = db.tblAcademicRegulations.FirstOrDefault() ?? new tblAcademicRegulation
                {
                    Title = "Academic Regulations",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Academic Regulations";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AcademicRegulations(tblAcademicRegulation input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Academic Regulations";
                return View(input);
            }

            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblAcademicRegulations.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.ID = db.tblAcademicRegulations.Any() ? db.tblAcademicRegulations.Max(x => x.ID) + 1 : 1;
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblAcademicRegulations.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Academic Regulations saved successfully.";
            return RedirectToAction("AcademicRegulations");
        }

        // ====== Admissions: Academic Deficiencies (single page content) ======
        [HttpGet]
        public ActionResult AcademicDeficiencies()
        {
            using (var db = new Model1())
            {
                var model = db.tblAcademicDeficiencies.FirstOrDefault() ?? new tblAcademicDeficiency
                {
                    Title = "Academic Deficiencies",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Academic Deficiencies";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AcademicDeficiencies(tblAcademicDeficiency input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Academic Deficiencies";
                return View(input);
            }
            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblAcademicDeficiencies.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.ID = db.tblAcademicDeficiencies.Any() ? db.tblAcademicDeficiencies.Max(x => x.ID) + 1 : 1;
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblAcademicDeficiencies.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Academic Deficiencies saved successfully.";
            return RedirectToAction("AcademicDeficiencies");
        }

        // ====== Rules & Regulations (MS) ======
        [HttpGet]
        public ActionResult RulesRegulationsMS()
        {
            using (var db = new Model1())
            {
                var model = db.tblRulesRegulationsMS.FirstOrDefault() ?? new tblRulesRegulationsMS
                {
                    Title = "Rules & Regulations (MS)",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Rules & Regulations (MS)";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult RulesRegulationsMS(tblRulesRegulationsMS input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Rules & Regulations (MS)";
                return View(input);
            }
            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblRulesRegulationsMS.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.ID = db.tblRulesRegulationsMS.Any() ? db.tblRulesRegulationsMS.Max(x => x.ID) + 1 : 1;
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblRulesRegulationsMS.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Rules & Regulations (MS) saved successfully.";
            return RedirectToAction("RulesRegulationsMS");
        }

        // ====== Rules & Regulations (PhD) ======
        [HttpGet]
        public ActionResult RulesRegulationsPhD()
        {
            using (var db = new Model1())
            {
                var model = db.tblRulesRegulationsPhds.FirstOrDefault() ?? new tblRulesRegulationsPhd
                {
                    Title = "Rules & Regulations (PhD)",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Rules & Regulations (PhD)";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult RulesRegulationsPhD(tblRulesRegulationsPhd input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Rules & Regulations (PhD)";
                return View(input);
            }
            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblRulesRegulationsPhds.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.ID = db.tblRulesRegulationsPhds.Any() ? db.tblRulesRegulationsPhds.Max(x => x.ID) + 1 : 1;
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblRulesRegulationsPhds.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Rules & Regulations (PhD) saved successfully.";
            return RedirectToAction("RulesRegulationsPhD");
        }

        // ====== Other submenu stubs ======
        [HttpGet]
        public ActionResult FinancialAssistance()
        {
            using (var db = new Model1())
            {
                var model = db.tblFinancialAssistances.FirstOrDefault() ?? new tblFinancialAssistance
                {
                    Title = "Financial Assistance",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Financial Assistance";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult FinancialAssistance(tblFinancialAssistance input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Financial Assistance";
                return View(input);
            }
            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblFinancialAssistances.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.ID = db.tblFinancialAssistances.Any() ? db.tblFinancialAssistances.Max(x => x.ID) + 1 : 1;
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblFinancialAssistances.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Financial Assistance saved successfully.";
            return RedirectToAction("FinancialAssistance");
        }

        [HttpGet]
        public ActionResult RefundPolicy()
        {
            using (var db = new Model1())
            {
                var model = db.tblRefundPolicies.FirstOrDefault() ?? new tblRefundPolicy
                {
                    Title = "Refund Policy",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Refund Policy";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult RefundPolicy(tblRefundPolicy input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Refund Policy";
                return View(input);
            }
            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblRefundPolicies.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.ID = db.tblRefundPolicies.Any() ? db.tblRefundPolicies.Max(x => x.ID) + 1 : 1;
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblRefundPolicies.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Refund Policy saved successfully.";
            return RedirectToAction("RefundPolicy");
        }

        [HttpGet]
        public ActionResult LatePayments()
        {
            using (var db = new Model1())
            {
                var model = db.tblLatePaymentPenalties.FirstOrDefault() ?? new tblLatePaymentPenalty();
                ViewBag.Title = "Fine/Penalties On Late Payments";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult LatePayments(tblLatePaymentPenalty input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Fine/Penalties On Late Payments";
                return View(input);
            }

            using (var db = new Model1())
            {
                if (input.PenaltyID > 0)
                {
                    var entity = db.tblLatePaymentPenalties.Find(input.PenaltyID);
                    if (entity == null) return HttpNotFound();
                    entity.PenaltyDetail = input.PenaltyDetail;
                    entity.ModifiedDate = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.CreatedDate = DateTime.Now;
                    input.ModifiedDate = DateTime.Now;
                    db.tblLatePaymentPenalties.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Fine/Penalties On Late Payments saved successfully.";
            return RedirectToAction("LatePayments");
        }

        [HttpGet]
        public ActionResult Accommodation()
        {
            using (var db = new Model1())
            {
                var model = db.tblAccomodations.FirstOrDefault() ?? new tblAccomodation
                {
                    Title = "Accommodation",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Accommodation";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Accommodation(tblAccomodation input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Accommodation";
                return View(input);
            }
            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblAccomodations.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.ID = db.tblAccomodations.Any() ? db.tblAccomodations.Max(x => x.ID) + 1 : 1;
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblAccomodations.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Accommodation page saved successfully.";
            return RedirectToAction("Accommodation");
        }

        [HttpGet]
        public ActionResult Transport()
        {
            using (var db = new Model1())
            {
                var model = db.tblAdmTransports.FirstOrDefault() ?? new tblAdmTransport
                {
                    Title = "Transport",
                    CreatedAt = DateTime.Now
                };
                ViewBag.Title = "Transport";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Transport(tblAdmTransport input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Transport";
                return View(input);
            }
            using (var db = new Model1())
            {
                if (input.ID > 0)
                {
                    var entity = db.tblAdmTransports.Find(input.ID);
                    if (entity == null) return HttpNotFound();
                    entity.Title = input.Title;
                    entity.PageData = input.PageData;
                    entity.UpdatedAt = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.ID = db.tblAdmTransports.Any() ? db.tblAdmTransports.Max(x => x.ID) + 1 : 1;
                    input.CreatedAt = DateTime.Now;
                    input.UpdatedAt = DateTime.Now;
                    db.tblAdmTransports.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Transport page saved successfully.";
            return RedirectToAction("Transport");
        }

        [HttpGet]
        public ActionResult MiscellaneousCharges()
        {
            using (var db = new Model1())
            {
                var model = db.tblMiscellaneousCharges.FirstOrDefault() ?? new tblMiscellaneousCharge();
                ViewBag.Title = "Miscellaneous Charges";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MiscellaneousCharges(tblMiscellaneousCharge input)
        {
            // Remove AmountRates from validation as page does not include it
            ModelState.Remove("AmountRates");
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Miscellaneous Charges";
                return View(input);
            }

            using (var db = new Model1())
            {
                if (input.ChargeID > 0)
                {
                    var entity = db.tblMiscellaneousCharges.Find(input.ChargeID);
                    if (entity == null) return HttpNotFound();
                    entity.Description = input.Description;
                    entity.ModifiedDate = DateTime.Now;
                    db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    input.CreatedDate = DateTime.Now;
                    input.ModifiedDate = DateTime.Now;
                    db.tblMiscellaneousCharges.Add(input);
                }
                db.SaveChanges();
            }
            TempData["Success"] = "Miscellaneous Charges saved successfully.";
            return RedirectToAction("MiscellaneousCharges");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMiscellaneousCharge(int id)
        {
            using (var db = new Model1())
            {
                var entity = db.tblMiscellaneousCharges.Find(id);
                if (entity == null) return HttpNotFound();
                db.tblMiscellaneousCharges.Remove(entity);
                db.SaveChanges();
            }
            TempData["Success"] = "Miscellaneous charge deleted.";
            return RedirectToAction("MiscellaneousCharges");
        }
    }
}