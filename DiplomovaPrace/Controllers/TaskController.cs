using DiplomovaPrace.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                updateTaskHistory(projectID, 1,0,true);
                NotificationSystem.SendNotification(EnumNotification.CREATE_TASK, "/Task");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");

        }

        private void updateTaskHistory(int projectID, int ID_State, int old_State, Boolean increment)
        {
            double createCount = 0;
            double progressCount = 0;
            double finishCount = 0;
            if (db.TaskHistories.Where(t => t.ID_Project == projectID).FirstOrDefault()!=null)
            {
                createCount = db.TaskHistories.Where(t => t.ID_Project == projectID).OrderBy(s=>s.ID).ToList().LastOrDefault().CreateCount;
                progressCount = db.TaskHistories.Where(t => t.ID_Project == projectID).OrderBy(s => s.ID).ToList().LastOrDefault().ProgressCount;
                finishCount = db.TaskHistories.Where(t => t.ID_Project == projectID).OrderBy(s => s.ID).ToList().LastOrDefault().FinishCount;
            }

            if (increment)
            {
                if (old_State == 0)
                {
                    if (ID_State == 1)
                    {
                        createCount++;
                    }
                }else if (old_State == 1)
                {
                    if (ID_State == 2)
                    {
                        createCount--;
                        progressCount++;
                    }
                    if (ID_State == 3)
                    {
                        createCount--;
                        finishCount++;
                    }
                }else if (old_State == 2)
                {
                    if (ID_State == 1)
                    {
                        progressCount--;
                        createCount++;
                    }
    
                    if (ID_State == 3)
                    {
                        progressCount--;
                        finishCount++;
                    }
                }else if (old_State == 3)
                {
                    if (ID_State == 1)
                    {
                        finishCount--;
                        createCount++;
                    }
                    if (ID_State == 2)
                    {
                        finishCount--;
                        progressCount++;
                    }
     
                }

            }
            else
            {
                if (ID_State == 1)
                {
                    createCount--;
                }
                else if (ID_State == 2)
                {
                    progressCount--;
                }
                else
                {
                    finishCount--;
                }
            }
           

            TaskHistory taskHistory = new TaskHistory();
            taskHistory.CreateCount = createCount;
            taskHistory.ProgressCount = progressCount;
            taskHistory.FinishCount = finishCount;
            taskHistory.ID_Project = projectID;
            taskHistory.Date = DateTime.Now;

            try
            {
                db.TaskHistories.Add(taskHistory);
                db.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }


        [HttpPost]
        public ActionResult ChangeState(int id, int ID_State, int old_State)
        {
            int projectID = (int)Session["projectID"];
            var task = db.Tasks.Find(id);
            task.ID_State = ID_State;
            if (ID_State == 3)
            {
                task.DateFinished = DateTime.Now;
                updateTaskHistory(projectID, 3,old_State,true);
            }
            else if(ID_State==2)
            {
                task.DateFinished = null;
                updateTaskHistory(projectID, 2,old_State,true);

            }
            else
            {
                updateTaskHistory(projectID, 1,old_State,true);
                task.DateFinished = null;

            }

            try
            {
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.CHANGE_TASK, "/Task");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["projectID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var task = db.Tasks.Find(id);

            int projectID = (int)Session["projectID"];
            ViewBag.ID_Priority = new SelectList(db.PriorityTasks, "ID", "Priority",task.ID_Priority);
            List<User> users = db.ProjectUsers.Where(p => p.ID_Project == projectID).Select(s => s.User).ToList();
            ViewBag.ID_User_Executor = new SelectList((from s in users select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName",task.ID_User_Executor);
            return View(task);
        }

        [HttpPost]
        public ActionResult Edit(Task task)
        {
            var old = db.Tasks.Find(task.ID);
            old.Deadline = task.Deadline;
            old.ID_Priority = task.ID_Priority;
            old.ID_User_Executor = task.ID_User_Executor;
            old.Text = task.Text;

            try
            {
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            int projectID = (int)Session["projectID"];
            ViewBag.ID_Priority = new SelectList(db.PriorityTasks, "ID", "Priority", task.ID_Priority);
            List<User> users = db.ProjectUsers.Where(p => p.ID_Project == projectID).Select(s => s.User).ToList();
            ViewBag.ID_User_Executor = new SelectList((from s in users select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName", task.ID_User_Executor);

            return View(old);
        }

        public ActionResult Chart()
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
            var taskHistory = db.TaskHistories.Where(t => t.ID_Project == projectID).OrderBy(s => s.ID);
            List<TaskHistory> list = new List<TaskHistory>();
            var date = taskHistory.FirstOrDefault().Date;
            var last = taskHistory.Where(t => t.Date.Year == date.Year && t.Date.Month == date.Month && t.Date.Day == date.Day).ToList().LastOrDefault();
            list.Add(last);
            foreach (TaskHistory task in taskHistory)
            {
                if (task.Date.ToShortDateString() != date.ToShortDateString())
                {
                    date = task.Date;
                    last = taskHistory.Where(t => t.Date.Year == date.Year && t.Date.Month == date.Month && t.Date.Day == date.Day).ToList().LastOrDefault();
                    list.Add(last);
                }

            }

            return View(list);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var task = db.Tasks.Find(id);
            int projectID = (int)Session["projectID"];
            if (task.ID_State == 3)
            {
                updateTaskHistory(projectID, 3,0, false);
            }
            else if (task.ID_State == 2)
            {
                updateTaskHistory(projectID, 2,0, false);

            }
            else
            {
                updateTaskHistory(projectID, 1,0, false);
            }
            try
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.DELETE_TASK, "/Task");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }
    }
}