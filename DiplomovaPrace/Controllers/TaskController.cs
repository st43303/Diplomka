using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class TaskController : Controller
    {
        private SDTEntities db = Database.GetDatabase();

        // GET: Task
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["projectID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int projectID = (int)Session["projectID"];

            var tasks = db.Tasks.Where(t => t.ID_Project == projectID);
            LinkedList<PriorityTask> priority = new LinkedList<PriorityTask>(db.PriorityTasks);
            priority.AddFirst(new PriorityTask(){ ID=0, Priority="Priorita"});
            ViewBag.ID_Priority = new SelectList(priority, "ID", "Priority");

            LinkedList<StateTask> state = new LinkedList<StateTask>(db.StateTasks);
            state.AddFirst(new StateTask(){ ID=0, State="Stav"});
            ViewBag.ID_State = new SelectList(state, "ID", "State");

            LinkedList<User> user = new LinkedList<User>(db.ProjectUsers.Where(p => p.ID_Project == projectID).Select(s => s.User));
            user.AddFirst(new User() { ID = 0, Name = "Zadavatel", Surname="" });
            ViewBag.ID_User_Creator = new SelectList((from s in user select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName");

            LinkedList<User> user2 = new LinkedList<User>(db.ProjectUsers.Where(p => p.ID_Project == projectID).Select(s => s.User));
            user2.AddFirst(new User() { ID = 0, Name = "Vyřizovatel", Surname = "" });
            ViewBag.ID_User_Executor = new SelectList((from s in user2 select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName");

            ViewBag.MyTask = false;
            return View(tasks);
        }

        [HttpPost]
        public ActionResult Index(int ID_Priority, int ID_State, int ID_User_Creator, int ID_User_Executor, Boolean? MyTask)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["projectID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int projectID = (int)Session["projectID"];
            int userID = (int)Session["userID"];
            var tasks = db.Tasks.Where(t => t.ID_Project == projectID);
            if (ID_Priority != 0)
            {
                tasks = tasks.Where(t => t.ID_Priority == ID_Priority);
            }

            if (ID_State != 0)
            {
                tasks = tasks.Where(t => t.ID_State == ID_State);
            }

            if (ID_User_Creator != 0)
            {
                tasks = tasks.Where(t => t.ID_User_Creator == ID_User_Creator);
            }

            if (ID_User_Executor != 0)
            {
                tasks = tasks.Where(t => t.ID_User_Executor == ID_User_Executor);
            }

            if (MyTask!=null)
            {
                if (MyTask == true)
                {
                    tasks = tasks.Where(t => t.ID_User_Executor == userID);
                    ViewBag.MyTask = true;
                }
                else
                {
                    ViewBag.MyTask = false;
                }

            }
            else
            {
                ViewBag.MyTask = false;
            }

            LinkedList<PriorityTask> priority = new LinkedList<PriorityTask>(db.PriorityTasks);
            priority.AddFirst(new PriorityTask() { ID = 0, Priority = "Priorita" });
            ViewBag.ID_Priority = new SelectList(priority, "ID", "Priority");

            LinkedList<StateTask> state = new LinkedList<StateTask>(db.StateTasks);
            state.AddFirst(new StateTask() { ID = 0, State = "Stav" });
            ViewBag.ID_State = new SelectList(state, "ID", "State");

            LinkedList<User> user = new LinkedList<User>(db.ProjectUsers.Where(p => p.ID_Project == projectID).Select(s => s.User));
            user.AddFirst(new User() { ID = 0, Name = "Zadavatel", Surname = "" });
            ViewBag.ID_User_Creator = new SelectList((from s in user select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName");

            LinkedList<User> user2 = new LinkedList<User>(db.ProjectUsers.Where(p => p.ID_Project == projectID).Select(s => s.User));
            user2.AddFirst(new User() { ID = 0, Name = "Vyřizovatel", Surname = "" });
            ViewBag.ID_User_Executor = new SelectList((from s in user2 select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName");

            return View(tasks);
        }

        public ActionResult Create()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["projectID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int projectID = (int)Session["projectID"];
            ViewBag.ID_Priority = new SelectList(db.PriorityTasks, "ID", "Priority");
            List<User> users = db.ProjectUsers.Where(p => p.ID_Project == projectID).Select(s => s.User).ToList();
            ViewBag.ID_User_Executor = new SelectList((from s in users select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Task task)
        {
            int userID = (int)Session["userID"];
            int projectID = (int)Session["projectID"];

            task.ID_User_Creator = userID;
            task.ID_Project = projectID;
            task.ID_State = 1;

            try
            {
                db.Tasks.Add(task);
                db.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var task = db.Tasks.Find(id);
            try
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }
    }
}