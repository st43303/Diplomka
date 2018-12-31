using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace DiplomovaPrace.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class HomeController : Controller
    {
        private SDTEntities db = new SDTEntities();
        public ActionResult Index()
        {

            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            try
            {
                var projects = db.Projects.Where(p => p.ID_Author == userID).ToList().OrderByDescending(x => x.ID).AsQueryable();
                return View(projects);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        private void ControlTasks(int projectID)
        {
            if (db.TaskHistories.Where(t => t.ID_Project == projectID).Count()==0)
            {
                TaskHistory taskHistory = new TaskHistory();
                taskHistory.CreateCount = 0;
                taskHistory.FinishCount = 0;
                taskHistory.ProgressCount = 0;
                taskHistory.ID_Project = projectID;
                if (db.Projects.Find(projectID).DateCreated.HasValue)
                {
                    taskHistory.Date = db.Projects.Find(projectID).DateCreated.Value;
                }
                else
                {
                    taskHistory.Date = DateTime.Now;
                }
                try
                {
                    db.TaskHistories.Add(taskHistory);
                    db.SaveChanges();
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public ActionResult SetProject(int id)
        {
            Project project = db.Projects.Find(id);
            if (project != null)
            {
                var projectName = "";
                if (project.Name.Length<20)
                {
                    projectName = project.Name;
                }
                else
                {
                    projectName = project.Code;
                }
                
                Session["projectID"] = id;
                Session["projectName"] = projectName;
                ControlTasks(id);
            }
            else
            {
                Console.WriteLine("Projekt nenalezen.");
            }
            return RedirectToAction("Index");
        }

     

        public ActionResult Shared()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            try
            {
                var sharedProjects = db.ProjectUsers.Where(p => p.ID_User == userID && p.Project.ID_Author != userID).AsQueryable();
                return View(sharedProjects);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetUsers(string term)
        {
            int userID = (int)Session["userID"];
            object output = null;
            try
            {
                List<User> users = db.Users.Where(u => u.ID != userID).ToList();
                users = users.Where(u => u.Name.ToUpper().Contains(term.ToUpper())
                    || u.Surname.ToUpper().Contains(term.ToUpper())
                    || u.City.ToUpper().Contains(term.ToUpper())).ToList();

                output = users.Select(s => new { s.ID, Name = s.Name + " " + s.Surname, s.City });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Json(output,JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetNotifications()
        {
            var userID = (int)Session["userID"];

            try
            {
                NotificationComponent NC = new NotificationComponent();
                var notifications = NC.GetNotifications(userID);
                var output = notifications.Select(s => new { Message = s.Message + " " + s.DateNotification.Value.ToShortDateString() + " " + s.DateNotification.Value.ToShortTimeString(), s.URL, s.ID, s.Avatar });
                return new JsonResult { Data = output, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;

        }

        [HttpPost]
        public ActionResult DeleteNotification(int id, string url)
        {
            try
            {
                var notification = db.Notifications.Find(id);
                db.Notifications.Remove(notification);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Content("<script>location.href = "+url+";</script>");
        }


    }
}