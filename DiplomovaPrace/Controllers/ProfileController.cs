using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class ProfileController : Controller
    {
        private SDTEntities db = Database.GetDatabase();
        // GET: Profile
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            try
            {
                var user = db.Users.Where(u => u.ID == userID).FirstOrDefault();
                return View(user);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult Index(User user, HttpPostedFileBase AvatarFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User old = db.Users.Find(user.ID);
                    bool changedImage = false;
                    if (AvatarFile != null)
                    {
                        old.Avatar = SaveFile(AvatarFile);
                        changedImage = true;
                    }

                    old.Name = user.Name;
                    old.Surname = user.Surname;
                    old.City = user.City;
                    old.BirthDate = user.BirthDate;
                    db.Entry(old).State = EntityState.Modified;
                    db.SaveChanges();
                    if (changedImage)
                    {
                        Session["avatar"] = old.Avatar;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.Error = "Něco se nepovedlo. Opakujte prosím akci.";
                }

            }

            return RedirectToAction("Index");
        }

        private string SaveFile(HttpPostedFileBase AvatarFile)
        {
            string filename = Path.GetFileNameWithoutExtension(AvatarFile.FileName);
            string extension = Path.GetExtension(AvatarFile.FileName);
            filename = filename + DateTime.Now.ToString("ddmmyyy") + extension;
            string filenameOutput = "/images/" + filename;
            filename = Path.Combine(Server.MapPath("~/images/"), filename);
            AvatarFile.SaveAs(filename);
            return filenameOutput;
        }

        public ActionResult Details(int id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            var user = db.Users.Find(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var contacts = db.Friendships.Where(f => f.ID_UserA == userID || f.ID_UserB == userID);
                ViewBag.Contact = 0;
                foreach (Friendship f in contacts)
                {
                    if (f.ID_UserA == userID)
                    {
                        if (f.ID_UserB == id)
                        {
                            if (f.Checked)
                            {
                                ViewBag.Contact = 1;
                            }
                            else
                            {
                                ViewBag.Contact = 2;
                            }
                        }
                    }
                    else
                    {
                        if (f.ID_UserA == id)
                        {
                            if (f.Checked)
                            {
                                ViewBag.Contact = 1;
                            }
                            else
                            {
                                ViewBag.Contact = 2;
                            }
                        }
                    }
                }
                List<Project> myProjects = db.Projects.Where(p => p.ID_Author == userID).ToList();
                List<Project> contactProjects = db.Projects.Where(p => p.ID_Author == id).ToList();
                List<Project> sharedProjects = new List<Project>();

                foreach (Project project in myProjects)
                {
                    if (db.ProjectUsers.Where(p => p.ID_Project == project.ID && p.ID_User == id).FirstOrDefault() != null)
                    {
                        sharedProjects.Add(project);
                    }
                }

                foreach (Project project in contactProjects)
                {
                    if (db.ProjectUsers.Where(p => p.ID_Project == project.ID && p.ID_User == userID).FirstOrDefault() != null)
                    {
                        sharedProjects.Add(project);
                    }
                }

                ViewBag.Projects = sharedProjects;


                return View(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        public ActionResult AddContact(int id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int) Session["userID"];
            
            try
            {
                User user = db.Users.Find(userID);
                Friendship f = new Friendship();
                f.ID_UserA = userID;
                f.ID_UserB = id;
                f.Checked = false;
                db.Friendships.Add(f);

                Notification notification = new Notification();
                notification.ID_User = id;
                notification.Message = "Uživatel " + user.Name + " " + user.Surname + " Vám poslal žádost o navázání kontaktu.";
                notification.Avatar = user.Avatar;
                notification.URL = "/Contacts/Requests";
                db.Notifications.Add(notification);
                db.SaveChanges();
            }catch(Exception ex)
            {
                ViewBag.Error = "Nastala chyba. Opakujte prosím akci.";
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Details", new { id });
        }

        public ActionResult CancelRequest(int id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            
            try
            {
                Friendship friendship = db.Friendships.Where(f => f.ID_UserA == userID && f.ID_UserB == id).FirstOrDefault();
                db.Friendships.Remove(friendship);
                db.SaveChanges();
            }catch(Exception ex)
            {
                ViewBag.Error = "Nastala chyba. Opakujte prosím akci.";
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Details", new { id });
        }
       
    }
}