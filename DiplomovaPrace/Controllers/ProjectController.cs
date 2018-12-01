using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class ProjectController : Controller
    {
        private SDTEntities db = Database.GetDatabase();
        // GET: Project
        public ActionResult Create()
        {
            
            if (Session["userID"] == null)
            {               
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    project.ID_Author = (int)Session["userID"];
                    project.DateCreated = DateTime.Now;
                    db.Projects.Add(project);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.CreateError = "Došlo k chybě. Opakujte prosím akci.";
                }
         
            }
            return View(project);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            try
            {
                
                db.Requirements.RemoveRange(project.Requirements);
                project.Requirements.Clear();

                db.Actors.RemoveRange(project.Actors);
                project.Actors.Clear();

                List<UseCaseActor> list = new List<UseCaseActor>();
                foreach(UseCase useCase in project.UseCases)
                {
                    list.AddRange(useCase.UseCaseActors);
                    useCase.UseCaseActors.Clear();
                }

                db.UseCaseActors.RemoveRange(list);
             

                db.UseCases.RemoveRange(project.UseCases);
                project.UseCases.Clear();

                db.ProjectUsers.RemoveRange(project.ProjectUsers);
                project.ProjectUsers.Clear();

                db.Projects.Remove(project);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }

        private List<User> GetContacts(int userID, int projectID)
        {
            List<User> list = new List<User>();
            var projectUsers = db.ProjectUsers.Where(p => p.ID_Project == projectID);

            var contacts = db.Friendships.Where(f => f.ID_UserA == userID || f.ID_UserB == userID);
            contacts = contacts.Where(f => f.Checked == true);

            foreach (Friendship f in contacts)
            {
                if (f.ID_UserA == userID)
                {
                    if(!isProjectUser(f.ID_UserB, projectUsers))
                    {
                        list.Add(f.User1);
                    }
                    
                }
                else if(!isProjectUser(f.ID_UserA,projectUsers))
                {
                    list.Add(f.User);
                }
            }

            return list;
        }

        private Boolean isProjectUser(int userID,IQueryable<ProjectUser> projectUsers)
        {
            Boolean answer = false;
            foreach(ProjectUser projectUser in projectUsers)
            {
                if (projectUser.ID_User == userID)
                {
                    answer = true;
                }
              
            }
            return answer;
        }

        public ActionResult Edit(int id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return RedirectToAction("Index","Home");
            }
            LinkedList<User> listContacts = new LinkedList<User>(GetContacts(userID,id));
            listContacts.AddFirst(new Models.User() { ID = 0, Name = "Vyberte", Surname = "" });
            ViewBag.Contacts = new SelectList((from s in listContacts select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName");
            ViewBag.Team = GetTeam(id, userID);

            return View(project);
        }

        [HttpPost]
        public ActionResult Edit(Project project,int? Contacts)
        {
            Project old = db.Projects.Find(project.ID);
            old.Name = project.Name;
            old.Description = project.Description;
            old.Code = project.Code;
          
            if (Contacts != 0)
            {
                ProjectUser projectUser = new ProjectUser();
                projectUser.ID_Project = project.ID;
                projectUser.ID_User = (Int32)Contacts;
                db.ProjectUsers.Add(projectUser);

                var user = db.Users.Find((int)Session["userID"]);
                Notification notification = new Notification();
                notification.ID_User = projectUser.ID_User;
                notification.Avatar = user.Avatar;
                notification.Message = "Uživatel " + user.Name + " " + user.Surname + " Vás připojil ke svému projektu „" + project.Name + "“.";
                notification.URL = "/Home/Shared";
                db.Notifications.Add(notification);

            }
            

            if (ModelState.IsValid)
            {
                try
                {                
                    db.Entry(old).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.Error = "Nastala chyba. Opakujte prosím akci.";
                }
            }
            List<User> listContacts = GetContacts(old.ID_Author,old.ID);
            ViewBag.Contacts = new SelectList((from s in listContacts select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName");
            ViewBag.Team = GetTeam(old.ID, old.ID_Author);
            return RedirectToAction("Edit", new { id = old.ID });
        }

        public List<User> GetTeam(int projectID,int userID)
        {
            List<User> list = new List<User>();
            var projectUsers = db.ProjectUsers.Where(p => p.ID_Project == projectID);
            foreach(ProjectUser projectUser in projectUsers)
            {
                if (projectUser.ID_User != userID)
                {
                    list.Add(projectUser.User);
                }
            }
            return list;
        }

        public ActionResult RemoveFromProject(int projectID, int userID)
        {
            ProjectUser projectUser = db.ProjectUsers.Where(p => p.ID_Project == projectID && p.ID_User == userID).FirstOrDefault();
            try
            {
                db.ProjectUsers.Remove(projectUser);
                db.SaveChanges();
            }catch(Exception ex)
            {
                ViewBag.Error = "Nastala chyba. Opakujte prosím akci.";
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Edit", new { id = projectID });
        }
    }
}