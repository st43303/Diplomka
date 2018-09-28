using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class RequirementsController : Controller
    {
        private SDTEntities db = Database.GetDatabase();
        // GET: Requirements
        public ActionResult Functional()
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

            var requirements = db.Requirements.Where(r => r.ID_Project == projectID && r.ID_ReqType == 1).OrderByDescending(r => r.ID_Requirement);

            LinkedList<CategoryRequirement> categories = new LinkedList<CategoryRequirement>(db.CategoryRequirements.Where(c => c.ID_Project == projectID).ToList());
            categories.AddFirst(new CategoryRequirement() { ID = 0, Name = "Kategorie" });
            ViewBag.ID_Category = new SelectList(categories, "ID", "Name");

            LinkedList<PriorityRequirement> priorities = new LinkedList<PriorityRequirement>(db.PriorityRequirements.ToList());
            priorities.AddFirst(new PriorityRequirement() { ID = 0, Priority = "Priorita" });
            ViewBag.ID_Priority = new SelectList(priorities, "ID", "Priority");

            LinkedList<StatusRequirement> statuses = new LinkedList<StatusRequirement>(db.StatusRequirements.ToList());
            statuses.AddFirst(new StatusRequirement() { ID = 0, Status = "Status" });
            ViewBag.ID_Status = new SelectList(statuses, "ID", "Status");


            return View(requirements);
        }

        [HttpPost]
        public ActionResult Functional(int ID_Category, int ID_Priority, int ID_Status, int Count)
        {
            int projectID = (int)Session["projectID"];
            var requirements = db.Requirements.Where(r => r.ID_Project == projectID);
            if (ID_Category != 0)
            {
                requirements = requirements.Where(r => r.ID_Category == ID_Category);
            }

            if (ID_Priority != 0)
            {
                requirements = requirements.Where(r => r.ID_Priority == ID_Priority);
            }

            if (ID_Status != 0)
            {
                requirements = requirements.Where(r => r.ID_Status == ID_Status);
            }
            if (Count != 0)
            {
                requirements = requirements.Take(Count);
            }


            LinkedList<CategoryRequirement> categories = new LinkedList<CategoryRequirement>(db.CategoryRequirements.Where(c => c.ID_Project == projectID).ToList());
            categories.AddFirst(new CategoryRequirement() { ID = 0, Name = "Kategorie" });
            ViewBag.ID_Category = new SelectList(categories, "ID", "Name");

            LinkedList<PriorityRequirement> priorities = new LinkedList<PriorityRequirement>(db.PriorityRequirements.ToList());
            priorities.AddFirst(new PriorityRequirement() { ID = 0, Priority = "Priorita" });
            ViewBag.ID_Priority = new SelectList(priorities, "ID", "Priority");

            LinkedList<StatusRequirement> statuses = new LinkedList<StatusRequirement>(db.StatusRequirements.ToList());
            statuses.AddFirst(new StatusRequirement() { ID = 0, Status = "Status" });
            ViewBag.ID_Status = new SelectList(statuses, "ID", "Status");

            return View(requirements);
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

            ViewBag.ID_ReqType = new SelectList(db.ReqTypes, "ID", "Type");
            ViewBag.ID_Priority = db.PriorityRequirements.ToList();
            ViewBag.ID_Category = new SelectList(db.CategoryRequirements.Where(c => c.ID_Project == projectID), "ID", "Name");
            ViewBag.ID_Status = db.StatusRequirements.ToList();
            return View();
        }

        public ActionResult Nonfunctional()
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

            var requirements = db.Requirements.Where(r => r.ID_Project == projectID && r.ID_ReqType == 2).OrderByDescending(r => r.ID_Requirement);


            LinkedList<CategoryRequirement> categories = new LinkedList<CategoryRequirement>(db.CategoryRequirements.Where(c => c.ID_Project == projectID).ToList());
            categories.AddFirst(new CategoryRequirement() { ID = 0, Name = "Kategorie" });
            ViewBag.ID_Category = new SelectList(categories, "ID", "Name");

            LinkedList<PriorityRequirement> priorities = new LinkedList<PriorityRequirement>(db.PriorityRequirements.ToList());
            priorities.AddFirst(new PriorityRequirement() { ID = 0, Priority = "Priorita" });
            ViewBag.ID_Priority = new SelectList(priorities, "ID", "Priority");

            LinkedList<StatusRequirement> statuses = new LinkedList<StatusRequirement>(db.StatusRequirements.ToList());
            statuses.AddFirst(new StatusRequirement() { ID = 0, Status = "Status" });
            ViewBag.ID_Status = new SelectList(statuses, "ID", "Status");
            return View(requirements);
        }

        [HttpPost]
        public ActionResult Nonfunctional(int ID_Category, int ID_Priority, int ID_Status, int Count)
        {
            int projectID = (int)Session["projectID"];
            var requirements = db.Requirements.Where(r => r.ID_Project == projectID);
            if (ID_Category != 0)
            {
                requirements = requirements.Where(r => r.ID_Category == ID_Category);
            }

            if (ID_Priority != 0)
            {
                requirements = requirements.Where(r => r.ID_Priority == ID_Priority);
            }

            if (ID_Status != 0)
            {
                requirements = requirements.Where(r => r.ID_Status == ID_Status);
            }
            if (Count != 0)
            {
                requirements = requirements.Take(Count);
            }


            LinkedList<CategoryRequirement> categories = new LinkedList<CategoryRequirement>(db.CategoryRequirements.Where(c => c.ID_Project == projectID).ToList());
            categories.AddFirst(new CategoryRequirement() { ID = 0, Name = "Kategorie" });
            ViewBag.ID_Category = new SelectList(categories, "ID", "Name");

            LinkedList<PriorityRequirement> priorities = new LinkedList<PriorityRequirement>(db.PriorityRequirements.ToList());
            priorities.AddFirst(new PriorityRequirement() { ID = 0, Priority = "Priorita" });
            ViewBag.ID_Priority = new SelectList(priorities, "ID", "Priority");

            LinkedList<StatusRequirement> statuses = new LinkedList<StatusRequirement>(db.StatusRequirements.ToList());
            statuses.AddFirst(new StatusRequirement() { ID = 0, Status = "Status" });
            ViewBag.ID_Status = new SelectList(statuses, "ID", "Status");

            return View(requirements);
        }


        [HttpPost]
        public ActionResult Create(Requirement requirement )
        {
            int projectID = (int)Session["projectID"];
            requirement.ID_Project = projectID;

            try
            {
                db.Requirements.Add(requirement);
                db.SaveChanges();

                LinkedList<CategoryRequirement> categories = new LinkedList<CategoryRequirement>(db.CategoryRequirements.Where(c => c.ID_Project == projectID).ToList());
                categories.AddFirst(new CategoryRequirement() { ID = 0, Name = "Kategorie" });
                ViewBag.ID_Category = new SelectList(categories, "ID", "Name");

                LinkedList<PriorityRequirement> priorities = new LinkedList<PriorityRequirement>(db.PriorityRequirements.ToList());
                priorities.AddFirst(new PriorityRequirement() { ID = 0, Priority = "Priorita" });
                ViewBag.ID_Priority = new SelectList(priorities, "ID", "Priority");

                LinkedList<StatusRequirement> statuses = new LinkedList<StatusRequirement>(db.StatusRequirements.ToList());
                statuses.AddFirst(new StatusRequirement() { ID = 0, Status = "Status" });
                ViewBag.ID_Status = new SelectList(statuses, "ID", "Status");

                if (requirement.ID_ReqType == 1)
                {
                    NotificationSystem.SendNotification(EnumNotification.CREATE_REQUIREMENT, "/Requirements/Functional");
                    return RedirectToAction("Functional");
                }
                else
                {
                    NotificationSystem.SendNotification(EnumNotification.CREATE_REQUIREMENT, "/Requirements/Nonfunctional");
                    return RedirectToAction("Nonfunctional");
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Error = "Vyskytla se chyba. Opakujte prosím akci.";
                ViewBag.ID_ReqType = new SelectList(db.ReqTypes, "ID", "Type");
            }
            return View(requirement);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var requirement = db.Requirements.Find(id);
            try
            {
                db.Requirements.Remove(requirement);
                db.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (requirement.ID_ReqType == 1)
            {
                NotificationSystem.SendNotification(EnumNotification.DELETE_REQUIREMENT, "/Requirements/Functional");
                return RedirectToAction("Functional");
            }
            else
            {
                NotificationSystem.SendNotification(EnumNotification.DELETE_REQUIREMENT, "/Requirements/Nonfunctional");
                return RedirectToAction("Nonfunctional");
            }
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
            int projectID = (int)Session["projectID"];
            var requirement = db.Requirements.Find(id);
            if (requirement == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ID_Priority = db.PriorityRequirements.ToList();
            ViewBag.ID_Category = new SelectList(db.CategoryRequirements.Where(c => c.ID_Project == projectID), "ID", "Name",requirement.ID_Requirement);
            ViewBag.ID_Status = db.StatusRequirements.ToList();
            ViewBag.ID_ReqType = new SelectList(db.ReqTypes, "ID", "Type",requirement.ID_ReqType);

            return View(requirement);
        }

        [HttpPost]
        public ActionResult Edit(Requirement requirement)
        {
            var old = db.Requirements.Find(requirement.ID);
            old.ID_ReqType = requirement.ID_ReqType;
            old.Text = requirement.Text;
            old.ID_Requirement = requirement.ID_Requirement;
            old.ID_Category = requirement.ID_Category;
            old.ID_Priority = requirement.ID_Priority;
            old.ID_Status = requirement.ID_Status;
            old.Source = requirement.Source;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(old).State = EntityState.Modified;
                    db.SaveChanges();

                    LinkedList<CategoryRequirement> categories = new LinkedList<CategoryRequirement>(db.CategoryRequirements.Where(c => c.ID_Project == old.ID_Project).ToList());
                    categories.AddFirst(new CategoryRequirement() { ID = 0, Name = "Kategorie" });
                    ViewBag.ID_Category = new SelectList(categories, "ID", "Name");

                    LinkedList<PriorityRequirement> priorities = new LinkedList<PriorityRequirement>(db.PriorityRequirements.ToList());
                    priorities.AddFirst(new PriorityRequirement() { ID = 0, Priority = "Priorita" });
                    ViewBag.ID_Priority = new SelectList(priorities, "ID", "Priority");

                    LinkedList<StatusRequirement> statuses = new LinkedList<StatusRequirement>(db.StatusRequirements.ToList());
                    statuses.AddFirst(new StatusRequirement() { ID = 0, Status = "Status" });
                    ViewBag.ID_Status = new SelectList(statuses, "ID", "Status");

                    if (requirement.ID_ReqType == 1)
                    {
                        NotificationSystem.SendNotification(EnumNotification.EDIT_REQUIREMENT, "/Requirements/Functional");
                        return RedirectToAction("Functional");
                    }
                    else
                    {
                        NotificationSystem.SendNotification(EnumNotification.EDIT_REQUIREMENT, "/Requirements/Nonfunctional");
                        return RedirectToAction("Nonfunctional");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.Error = "Vyskytla se chyba. Opakujte prosím akci.";
                    ViewBag.ID_Priority = db.PriorityRequirements.ToList();
                    ViewBag.ID_Category = new SelectList(db.CategoryRequirements.Where(c => c.ID_Project == requirement.ID_Project), "ID", "Name", requirement.ID_Requirement);
                    ViewBag.ID_Status = db.StatusRequirements.ToList();
                    ViewBag.ID_ReqType = new SelectList(db.ReqTypes, "ID", "Type", requirement.ID_ReqType);
                }
            }

            return View(requirement);
         
        }


        public ActionResult Matrix()
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

            ViewBag.Reqs = db.Requirements.Where(r => r.ID_Project == projectID && r.ID_ReqType==1).ToList();
            ViewBag.Uses = db.UseCases.Where(u => u.ID_Project == projectID).ToList();
            return View();
        }
    }
}