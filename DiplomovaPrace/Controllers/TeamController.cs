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
            var users = db.ProjectUsers.Where(p => p.ID_Project == projectID);
            return View(users);
        }
    }
}