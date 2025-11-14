using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HitecCM.Models;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace HitecCM.Controllers
{
    public class AccountController : Controller
    {
        private Model1 db = new Model1();

        // GET: Login
        public ActionResult Login()
        {
            // If user is already logged in, redirect to home
            if (Session["UserID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Authenticate using UserName column (display name) instead of Login
                    var user = db.tblUsers
                        .FirstOrDefault(u => u.UserName == model.Username && u.Password == model.Password);

                    if (user != null)
                    {
                        if (user.Active == null || user.Active.Value == true)
                        {
                            Session["UserID"] = user.UserID;
                            Session["Username"] = user.UserName; // display name entered
                            Session["Login"] = user.Login;       // keep original login id for role checks

                            // Role assignment based on Login column value
                            if (string.Equals(user.Login, "admin", StringComparison.OrdinalIgnoreCase))
                                Session["Role"] = "Admin";
                            else if (string.Equals(user.Login, "DSA", StringComparison.OrdinalIgnoreCase))
                                Session["Role"] = "DSA";
                            else
                                Session["Role"] = "User";

                            // Role-based redirect
                            if (string.Equals(user.Login, "DSA", StringComparison.OrdinalIgnoreCase))
                                return RedirectToAction("Index", "Events");

                            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                                return Redirect(model.ReturnUrl);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "This account is inactive.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database connection error. Please check your connection string.");
                    System.Diagnostics.Debug.WriteLine($"Login Exception: {ex}");
                }
            }
            return View(model);
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        // GET: Register (only available when no users exist)
        public ActionResult Register()
        {
            // Check if any users exist
            var userCount = db.tblUsers.Count();
            
            // Only allow registration if no users exist (first time setup)
            if (userCount > 0)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            // Check if any users exist
            var userCount = db.tblUsers.Count();
            
            // Only allow registration if no users exist (first time setup)
            if (userCount > 0)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if user already exists
                    var existingUser = db.tblUsers.FirstOrDefault(u => u.Login == model.Username);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("", "Username already exists.");
                        return View(model);
                    }

                    // Create new user
                    var newUser = new tblUser
                    {
                        Login = model.Username,
                        Password = model.Password,
                        UserName = model.FullName,
                        Active = true
                    };

                    db.tblUsers.Add(newUser);
                    db.SaveChanges();

                    // Log the user in
                    Session["UserID"] = newUser.UserID;
                    Session["Username"] = newUser.UserName;
                    Session["Login"] = newUser.Login;

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                }
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helper Methods

        private string HashPassword(string password)
        {
            // Simple hashing for demonstration - in production, use a stronger method like bcrypt
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        #endregion
    }

    #region View Models

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    #endregion
}