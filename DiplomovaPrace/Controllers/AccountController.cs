using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class AccountController : Controller
    {
        private SDTEntities db = new SDTEntities();
        // GET: Login
        public ActionResult Login()
        {

            if (Session["userID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            try
            {
                var password = GetHashString(user.Password);
                var userDetails = db.Users.Where(u => u.Username == user.Username && u.Password == password).FirstOrDefault();
                if (userDetails == null)
                {
                    ViewBag.ErrorMessage = "Špatné uživatelské jméno či heslo.";
                    return View();
                }
                else
                {
                    Session["userID"] = userDetails.ID;
                    Session["userName"] = userDetails.Username;
                    Session["avatar"] = userDetails.Avatar;

                    userDetails.LastActive = DateTime.Now;
                    db.Entry(userDetails).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.ErrorMessage = "Něco se pokazilo. Opakujte prosím akci.";
                return View();
            }

                return RedirectToAction("Index", "Home");

        }

        public ActionResult Register()
        {
            if (Session["userID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user, String ConfirmPassword)
        {
            try
            {
                ViewBag.ErrorMessage = "";
                if (user.Password.Equals(ConfirmPassword))
                {
                    foreach (User u in db.Users)
                    {
                        if (u.Username.ToUpper().Equals(user.Username.ToUpper()))
                        {
                            ViewBag.ErrorMessage = "Zadané uživatelské jméno již existuje. Zadejte prosím jiné.";
                            return View();
                        }
                    }
                    string newPassword = GetHashString(ConfirmPassword);
                    user.Password = newPassword;
                    user.Avatar = "/images/admin.jpg";
                    user.RegistrationDate = DateTime.Now;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.ErrorMessage = "Hesla se neshodují. Opakujte prosím akci.";
                    return View();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }


            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }

        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
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
