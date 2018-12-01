using DiplomovaPrace.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DiplomovaPrace.Controllers
{
    public class HomeController : Controller
    {
       private SDTEntities db = Database.GetDatabase();
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            var projects = db.Projects.Where(p => p.ID_Author == userID).ToList().OrderByDescending(x => x.ID);

            return View(projects);
        }

        private void controlTasks(int projectID)
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
                controlTasks(id);
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

            var sharedProjects = db.ProjectUsers.Where(p => p.ID_User == userID && p.Project.ID_Author!=userID);
     

            return View(sharedProjects);
        }

        [HttpPost]
        public ActionResult GetUsers(string term)
        {
            
            int userID = (int)Session["userID"];
            List<User> users = db.Users.Where(u => u.ID != userID).ToList();
            users = users.Where(u => u.Name.ToUpper().Contains(term.ToUpper())
                ||u.Surname.ToUpper().Contains(term.ToUpper())
                ||u.City.ToUpper().Contains(term.ToUpper())).ToList();

            var output = users.Select(s => new {s.ID, Name = s.Name + " " + s.Surname, s.City });
            return Json(output,JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetNotifications()
        {
            var userID = (int)Session["userID"];
            List<Notification> notifications = db.Notifications.Where(n => n.ID_User == userID).OrderByDescending(i=>i.ID).ToList();

            var output = notifications.Select(s => new { Message=s.Message+" "+s.DateNotification.Value.ToShortDateString() + " "+s.DateNotification.Value.ToShortTimeString(), s.URL, s.ID, s.Avatar });
            return new JsonResult { Data = output, JsonRequestBehavior= JsonRequestBehavior.AllowGet };

        }

        [HttpPost]
        public ActionResult DeleteNotification(int id, string url)
        {
            var notification = db.Notifications.Find(id);
            try
            {
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