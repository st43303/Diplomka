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
    //[OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
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

                ViewBag.technologies = new MultiSelectList(db.Technologies.Where(t=>t.UserTechnologies.Select(s=>s.ID_User).Contains(user.ID)==false), "ID", "Name");
                ViewBag.myTechnologies = new MultiSelectList(db.Technologies.Where(t => t.UserTechnologies.Select(s => s.ID_User).Contains(user.ID)), "ID", "Name");
                return View(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(User user, HttpPostedFileBase AvatarFile, List<int> myTechnologies)
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
                if (myTechnologies != null)
                {
                    db.UserTechnologies.RemoveRange(old.UserTechnologies);
                    old.UserTechnologies.Clear();
                    for (int i = 0; i < myTechnologies.Count; i++)
                    {
                        UserTechnology userTechnology = new UserTechnology();
                        userTechnology.ID_User = old.ID;
                        userTechnology.ID_Technology = myTechnologies[i];
                        db.UserTechnologies.Add(userTechnology);
                    }
                }
       
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
                if (changedImage)
                {
                    Session["avatar"] = old.Avatar;
                }
            }
            catch (Exception ex)
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
            int userID = (int)Session["userID"];

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
                notification.DateNotification = DateTime.Now;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
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

        public ActionResult CancelAccount() {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
 
                var userProjects = db.Projects.Where(p => p.ID_Author == userID);
                var sharedProjects = userProjects.Where(u => u.ProjectUsers.Count > 1);
            if (sharedProjects.Count()!=0)
            {
                UserProject userProject = new UserProject();
                userProject.Project = sharedProjects.FirstOrDefault();
                userProject.ID_Project = sharedProjects.FirstOrDefault().ID;
                userProject.Remove = false;
                var projectUsers = sharedProjects.FirstOrDefault().ProjectUsers.Where(s => s.ID_User != userID);
                ViewBag.New_Author = new SelectList(from s in projectUsers select new { ID = s.ID_User, FullName = s.User.Name + " " + s.User.Surname }, "ID", "FullName");
                return View(userProject);
            }
            else
            {
                foreach(var item in userProjects)
                {
                    RemoveProject(item);
                }
                RemoveUser(userID);
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
   
         
        }

        [HttpPost]
        public ActionResult CancelAccount(UserProject userProject)
        {
            if (userProject.Remove)
            {
                RemoveProject(db.Projects.Find(userProject.ID_Project));
            }
            else
            {
                Project project = db.Projects.Find(userProject.ID_Project);
                project.ID_Author = userProject.New_Author;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
            }
            int userID = (int)Session["userID"];
            var userProjects = db.Projects.Where(p => p.ID_Author == userID);
            var sharedProjects = userProjects.Where(u => u.ProjectUsers.Count > 1);
            if (sharedProjects.Count()!=0)
            {
                UserProject userPROJECT = new UserProject();
                userPROJECT.Project = sharedProjects.FirstOrDefault();
                userPROJECT.ID_Project = sharedProjects.FirstOrDefault().ID;
                userPROJECT.Remove = false;
                var projectUsers = sharedProjects.FirstOrDefault().ProjectUsers.Where(s => s.ID_User != userID);
                ViewBag.New_Author = new SelectList(from s in projectUsers select new { ID = s.ID_User, FullName = s.User.Name + " " + s.User.Surname }, "ID", "FullName");
                return View(userPROJECT);
            }
            else
            {
                foreach (var item in userProjects)
                {
                    RemoveProject(item);
                }
                RemoveUser(userID);
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
        }


        private void RemoveUser(int ID)
        {

            User user = db.Users.Find(ID);
        
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            


        }

        private void RemoveProject(Project project)
        {
            try
            {
                RemoveScenarios(project.Scenarios);
                db.Scenarios.RemoveRange(project.Scenarios);
                db.Actors.RemoveRange(project.Actors);
                project.Actors.Clear();

                db.CategoryRequirements.RemoveRange(project.CategoryRequirements);
                project.CategoryRequirements.Clear();

                RemoveFiles(project.Files);
                db.Files.RemoveRange(project.Files);

                db.ProjectUsers.RemoveRange(project.ProjectUsers);
                project.ProjectUsers.Clear();

                RemoveRequirements(project.Requirements);
                db.Requirements.RemoveRange(project.Requirements);

                db.TaskHistories.RemoveRange(project.TaskHistories);
                project.TaskHistories.Clear();

                db.Tasks.RemoveRange(project.Tasks);
                project.Tasks.Clear();
                RemoveUseCases(project.UseCases);
                db.UseCases.RemoveRange(project.UseCases);

                db.Projects.Remove(project);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveUseCases(ICollection<UseCase> useCases)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void RemoveRequirements(ICollection<Requirement> requirements)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void RemoveScenarios(ICollection<Scenario> scenarios)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void RemoveFiles(ICollection<Models.File> files)
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
            catch (Exception ex)
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