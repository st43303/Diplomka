using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class TeamController : Controller
    {
        private SDTEntities db = Database.GetDatabase();

        // GET: Team
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
            int userID = (int)Session["userID"];
            var users = db.ProjectUsers.Where(p => p.ID_Project == projectID);
            List<User> listContacts = GetContacts(userID, projectID);
            ViewBag.Contacts = new SelectList((from s in listContacts select new { s.ID, FullName = s.Name + " " + s.Surname }), "ID", "FullName");
            return View(users);
        }

        [HttpPost]
        public ActionResult Create(int Contacts)
        {
            int projectID = (int)Session["projectID"];
            ProjectUser projectUser = new ProjectUser();
            projectUser.ID_Project = projectID;
            projectUser.ID_User = Contacts;
            try
            {
                db.ProjectUsers.Add(projectUser);
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
            ProjectUser projectUser = db.ProjectUsers.Find(id);
            try
            {
                db.ProjectUsers.Remove(projectUser);
                db.SaveChanges();
            }catch(Exception ex)
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

        private Boolean isProjectUser(int userID, IQueryable<ProjectUser> projectUsers)
        {
            Boolean answer = false;
            foreach (ProjectUser projectUser in projectUsers)
            {
                if (projectUser.ID_User == userID)
                {
                    answer = true;
                }

            }
            return answer;
        }



    }
}