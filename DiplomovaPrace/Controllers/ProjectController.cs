using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class ProjectController : Controller
    {
        private SDTEntities db = new SDTEntities();
        // GET: Project
        public ActionResult Create()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.technologies = new MultiSelectList(db.Technologies.ToList(), "ID", "Name");
            ViewBag.myTechnologies = new MultiSelectList(db.Technologies.Where(s => s.ID == 0), "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Project project, List<int> myTechnologies)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    project.ID_Author = (int)Session["userID"];
                    project.DateCreated = DateTime.Now;
                    project.WIP = 2;

                    db.Projects.Add(project);
                    db.SaveChanges();
                    for (int i = 0; i < myTechnologies.Count; i++)
                    {
                        ProjectTechnology projectTechnology = new ProjectTechnology();
                        projectTechnology.ID_Project = project.ID;
                        projectTechnology.ID_Technology = myTechnologies[i];
                        db.ProjectTechnologies.Add(projectTechnology);
                    }
                    ProjectUser projectUser = new ProjectUser();
                    projectUser.ID_User = project.ID_Author;
                    projectUser.ID_Project = project.ID;
                    db.ProjectUsers.Add(projectUser);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
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
                foreach (UseCase useCase in project.UseCases)
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
                if (project.ID == (int)Session["projectID"])
                {
                    Session["projectID"] = null;
                }

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
                    if (!isProjectUser(f.ID_UserB, projectUsers))
                    {
                        list.Add(f.User1);
                    }

                }
                else if (!isProjectUser(f.ID_UserA, projectUsers))
                {
                    list.Add(f.User);
                }
            }

            return list;
        }

        private bool isProjectUser(int userID, IQueryable<ProjectUser> projectUsers)
        {
            bool answer = false;
            foreach (ProjectUser projectUser in projectUsers)
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
            try
            {
                Project project = db.Projects.Find(id);
                ViewBag.technologies = new MultiSelectList(db.Technologies.Where(t => t.ProjectTechnologies.Select(s => s.ID_Project).Contains(project.ID) == false), "ID", "Name");
                ViewBag.myTechnologies = new MultiSelectList(db.Technologies.Where(t => t.ProjectTechnologies.Select(s => s.ID_Project).Contains(project.ID)), "ID", "Name");
                if (project == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Edit(Project project, List<int> myTechnologies)
        {
            Project old = db.Projects.Find(project.ID);
            old.Name = project.Name;
            old.Description = project.Description;
            old.Code = project.Code;
            db.ProjectTechnologies.RemoveRange(old.ProjectTechnologies);
            old.ProjectTechnologies.Clear();

            for (int i = 0; i < myTechnologies.Count; i++)
            {
                ProjectTechnology projectTechnology = new ProjectTechnology();
                projectTechnology.ID_Project = old.ID;
                projectTechnology.ID_Technology = myTechnologies[i];
                db.ProjectTechnologies.Add(projectTechnology);
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
            ViewBag.technologies = new MultiSelectList(db.Technologies.Where(t => t.ProjectTechnologies.Select(s => s.ID_Project).Contains(project.ID) == false), "ID", "Name");
            ViewBag.myTechnologies = new MultiSelectList(db.Technologies.Where(t => t.ProjectTechnologies.Select(s => s.ID_Project).Contains(project.ID)), "ID", "Name");
            return RedirectToAction("Edit", new { id = old.ID });
        }

        public List<User> GetTeam(int projectID, int userID)
        {
            List<User> list = new List<User>();
            var projectUsers = db.ProjectUsers.Where(p => p.ID_Project == projectID);
            foreach (ProjectUser projectUser in projectUsers)
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
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Nastala chyba. Opakujte prosím akci.";
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Edit", new { id = projectID });
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