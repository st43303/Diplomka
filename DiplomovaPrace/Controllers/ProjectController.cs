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
                try
                {
                    project.ID_Author = (int)Session["userID"];
                    project.DateCreated = DateTime.Now;
                    project.WIP = 2;
                    db.Projects.Add(project);
                    db.SaveChanges();
                if (myTechnologies != null)
                {
                    AddTechnologies(myTechnologies, project.ID);
                }
                  
                    ProjectUser projectUser = new ProjectUser();
                    projectUser.ID_User = project.ID_Author;
                    projectUser.ID_Project = project.ID;
                    db.ProjectUsers.Add(projectUser);
                    db.SaveChanges();                
                    return RedirectToAction("SetProject", "Home",new { id=project.ID});
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.CreateError = "Došlo k chybě. Opakujte prosím akci.";
                }
            ViewBag.technologies = new MultiSelectList(db.Technologies.ToList(), "ID", "Name");
            ViewBag.myTechnologies = new MultiSelectList(db.Technologies.Where(s => s.ID == 0), "ID", "Name");

            return View(project);
        }

        private void AddTechnologies(List<int> technologies, int projectID)
        {
            for (int i = 0; i < technologies.Count; i++)
            {
                ProjectTechnology projectTechnology = new ProjectTechnology();
                projectTechnology.ID_Project = projectID;
                projectTechnology.ID_Technology = technologies[i];
                db.ProjectTechnologies.Add(projectTechnology);
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            try
            {
                RemoveScenarios(id);
                RemoveUseCases(id);
                RemoveActors(id);
                RemoveRequirements(id);
                RemoveCategoryRequirements(id);
                RemoveFiles(id);
                RemoveNotifications(id);
                RemoveTaskHistories(id);
                RemoveTasks(id);
                RemoveProjectUsers(id);
                RemoveTechnologies(id);
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

        private void RemoveTechnologies(int projectID)
        {
            var technologies = db.ProjectTechnologies.Where(t => t.ID_Project == projectID).AsQueryable();
            try
            {
                db.ProjectTechnologies.RemoveRange(technologies);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveProjectUsers(int projectID)
        {
            var users = db.ProjectUsers.Where(p => p.ID_Project == projectID).AsQueryable();
            try
            {
                db.ProjectUsers.RemoveRange(users);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveTasks(int projectID)
        {
            var tasks = db.Tasks.Where(t => t.ID_Project == projectID).AsQueryable();
            try
            {
                db.Tasks.RemoveRange(tasks);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveNotifications(int projectID)
        {
            var notifications = db.Notifications.Where(n => n.ID_Project == projectID).AsQueryable();
            try
            {
                db.Notifications.RemoveRange(notifications);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveTaskHistories(int projectID)
        {
            var tasksH = db.TaskHistories.Where(t => t.ID_Project == projectID).AsQueryable();
            try
            {
                db.TaskHistories.RemoveRange(tasksH);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveFiles(int projectID)
        {
            var files = db.Files.Where(r => r.ID_Project == projectID);
            try
            {
               foreach(var file in files)
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

        private void RemoveCategoryRequirements(int projectID)
        {
            var creqs = db.CategoryRequirements.Where(r => r.ID_Project == projectID).AsQueryable();
            try
            {
                db.CategoryRequirements.RemoveRange(creqs);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveRequirements(int projectID)
        {
            var reqs = db.Requirements.Where(r => r.ID_Project == projectID).AsQueryable();
            try
            {
                db.Requirements.RemoveRange(reqs);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveActors(int projectID)
        {
            var actors = db.Actors.Where(a => a.ID_Project == projectID).AsQueryable();
            try
            {
                db.Actors.RemoveRange(actors);
                db.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RemoveUseCases(int projectID)
        {
            var useCases = db.UseCases.Where(u => u.ID_Project == projectID).AsQueryable();

            foreach(var useCase in useCases)
            {
                try
                {
                    db.UseCaseActors.RemoveRange(useCase.UseCaseActors);
                    useCase.UseCaseActors.Clear();
                    db.UseCases.Remove(useCase);
                    db.SaveChanges();
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
      
        }

        private void RemoveScenarios(int projectID)
        {
            var scenarios = db.Scenarios.Where(s => s.ID_Project == projectID).AsQueryable();

            foreach(var scenario in scenarios)
            {
                try
                {
                    db.ScenarioActors.RemoveRange(scenario.ScenarioActors);
                    scenario.ScenarioActors.Clear();
                    var alternativeScenarious = db.Scenarios.Where(s => s.ID_MainScenario == scenario.ID).AsQueryable();
                    db.Scenarios.RemoveRange(alternativeScenarious);
                    db.Scenarios.Remove(scenario);
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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