using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class ProfileController : Controller
    {
        private SDTEntities db = new SDTEntities();
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

        [HttpPost]
        public ActionResult CancelAccount(int ID, string pswd)
        {

            User user = db.Users.Find(ID);
            if(user.Password == GetHashString(pswd))
            {
                try
                {
                    // odstranění nahraných souborů
                    RemoveFiles(db.Files.ToList());
                    // odstranění navázaných kontaktů
                    db.Friendships.RemoveRange(user.Friendships);
                    user.Friendships.Clear();
                    db.Friendships.RemoveRange(user.Friendships1);
                    user.Friendships1.Clear();
                    // odstranění nepřečtených upozornění
                    db.Notifications.RemoveRange(user.Notifications);
                    user.Notifications.Clear();
                    // odstranění projektů, či jejich předání jinému členovi týmu
                    List<Project> projects = user.Projects.ToList();
                    foreach (Project project in projects)
                    {
                        ChangeProjectAuthor(project, ID);
                    }
                    user.Projects.Clear();
                    // odstranění vazeb uživatele na jiné projekty
                    db.ProjectUsers.RemoveRange(user.ProjectUsers);
                    user.ProjectUsers.Clear();
                    // odstranění úkolů uživatele
                    db.Tasks.RemoveRange(user.Tasks);
                    user.Tasks.Clear();
                    db.Tasks.RemoveRange(user.Tasks1);
                    user.Tasks1.Clear();
                    // odstranění uživatele
                    db.Users.Remove(user);
                    db.SaveChanges();
                    Session.Abandon();
                    return RedirectToAction("Login", "Account");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.Error = "Něco se nepovedlo. Opakujte prosím akci.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.Error = "Heslo nebylo zadané správně. Opakujte prosím akci.";
                return RedirectToAction("Index");
            }

        }

        private void ChangeProjectAuthor(Project project, int userID)
        {
            try
            {
                // pokud měl projekt jen jednoho uživatele, rovnou ho smaž
                if (project.ProjectUsers.Count <=1)
                {
                    RemoveProject(project);
                }
                else
                {
                    // jinak nalezení jiného člena týmu a přiřazení vedení projektu
                    List<ProjectUser> projectUsers = project.ProjectUsers.ToList();
                    ProjectUser user = projectUsers.Where(p => p.ID_User != userID).FirstOrDefault();
                    project.ID_Author = user.ID_User;
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void RemoveProject(Project project)
        {
            try
            {
                RemoveScenarios(project.Scenarios.ToList());
                db.Actors.RemoveRange(project.Actors);
                project.Actors.Clear();

                db.CategoryRequirements.RemoveRange(project.CategoryRequirements);
                project.CategoryRequirements.Clear();

                RemoveFiles(project.Files.ToList());

                db.ProjectUsers.RemoveRange(project.ProjectUsers);
                project.ProjectUsers.Clear();

                RemoveRequirements(project.Requirements.ToList());
                

                db.TaskHistories.RemoveRange(project.TaskHistories);
                project.TaskHistories.Clear();

                db.Tasks.RemoveRange(project.Tasks);
                project.Tasks.Clear();
                RemoveUseCases(project.UseCases.ToList());

                db.Projects.Remove(project);
                db.SaveChanges();

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveUseCases(List<UseCase> useCases)
        {
            try
            {
                foreach (UseCase useCase in useCases)
                {
                    db.Scenarios.RemoveRange(useCase.Scenarios);
                    useCase.Scenarios.Clear();
                    db.UseCaseActors.RemoveRange(useCase.UseCaseActors);
                    useCase.UseCaseActors.Clear();
                    db.UseCaseRequirements.RemoveRange(useCase.UseCaseRequirements);
                    useCase.UseCaseRequirements.Clear();
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void RemoveRequirements(List<Requirement> requirements)
        {
            try
            {
                foreach (Requirement requirement in requirements)
                {
                    db.UseCaseRequirements.RemoveRange(requirement.UseCaseRequirements);
                    requirement.UseCaseRequirements.Clear();
                    db.Requirements.Remove(requirement);
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void RemoveScenarios(List<Scenario> scenarios)
        {
            try
            {
                foreach (Scenario scenario in scenarios)
                {
                    db.ScenarioActors.RemoveRange(scenario.ScenarioActors);
                    scenario.ScenarioActors.Clear();

                    db.Scenarios.Remove(scenario);
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void RemoveFiles(List<Models.File> files)
        {
            try
            {
                foreach (Models.File file in files)
                {
                    if (System.IO.File.Exists(file.Path))
                    {
                        System.IO.File.Delete(file.Path);
                        db.Files.Remove(file);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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