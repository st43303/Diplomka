using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class RequirementsController : Controller
    {
        private SDTEntities db = new SDTEntities();
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
            try
            {
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

                LinkedList<String> sources = new LinkedList<string>(requirements.Select(s => s.Source).Distinct().ToList());
                sources = new LinkedList<string>(sources.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList());
                sources.AddFirst("Zdroj");
                ViewBag.Source = new SelectList(sources.Select(x => new { Value = x, Text = x }), "Value", "Text");
                return View(requirements);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Functional(int ID_Category, int ID_Priority, int ID_Status, int Count, string Source)
        {
            int projectID = (int)Session["projectID"];
            try
            {
                var requirements = db.Requirements.Where(r => r.ID_Project == projectID && r.ID_ReqType == 1);
                LinkedList<String> sources = new LinkedList<string>(requirements.Select(s => s.Source).Distinct().ToList());

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

                if (Source != "Zdroj")
                {
                    requirements = requirements.Where(r => r.Source.Equals(Source));
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

                sources = new LinkedList<string>(sources.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList());
                sources.AddFirst("Zdroj");
                ViewBag.Source = new SelectList(sources.Select(x => new { Value = x, Text = x }), "Value", "Text");

                return View(requirements.OrderByDescending(r => r.ID_Requirement));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();

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
            try
            {
                ViewBag.ID_ReqType = new SelectList(db.ReqTypes, "ID", "Type");
                ViewBag.ID_Priority = db.PriorityRequirements.ToList();
                ViewBag.ID_Category = new SelectList(db.CategoryRequirements.Where(c => c.ID_Project == projectID), "ID", "Name");
                ViewBag.ID_Status = db.StatusRequirements.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
            try
            {
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

                LinkedList<String> sources = new LinkedList<string>(requirements.Select(s => s.Source).Distinct().ToList());
                sources = new LinkedList<string>(sources.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList());
                sources.AddFirst("Zdroj");
                ViewBag.Source = new SelectList(sources.Select(x => new { Value = x, Text = x }), "Value", "Text");

                return View(requirements);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Nonfunctional(int ID_Category, int ID_Priority, int ID_Status, int Count, string Source)
        {
            int projectID = (int)Session["projectID"];
            try
            {
                var requirements = db.Requirements.Where(r => r.ID_Project == projectID && r.ID_ReqType == 2);
                LinkedList<string> sources = new LinkedList<string>(requirements.Select(s => s.Source).Distinct().ToList());

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

                if (Source != "Zdroj")
                {
                    requirements = requirements.Where(r => r.Source.Equals(Source));
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

                sources = new LinkedList<string>(sources.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList());
                sources.AddFirst("Zdroj");
                ViewBag.Source = new SelectList(sources.Select(x => new { Value = x, Text = x }), "Value", "Text");

                return View(requirements.OrderByDescending(r => r.ID_Requirement));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        private string createID(int projectID)
        {
            try
            {
                string projectName = (string)Session["projectName"];
                int count = db.Requirements.Where(r => r.ID_Project == projectID).Count() + 1;
                string ID = projectName.First() + "" + count;
                return ID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }


        [HttpPost]
        public ActionResult Create(Requirement requirement)
        {
            int projectID = (int)Session["projectID"];
            requirement.ID_Project = projectID;
            requirement.ID_Requirement = createID(projectID);

            if (db.Requirements.Where(r => r.ID_Project == projectID && r.ID_Requirement.Equals(requirement.ID_Requirement)).FirstOrDefault() != null)
            {
                ViewBag.Error = "Zadané ID požadavku se již v projektu vyskytuje. Zvolte prosím jiné.";
            }
            else
            {
                try
                {
                    db.Requirements.Add(requirement);
                    db.SaveChanges();

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
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.Error = "Vyskytla se chyba. Opakujte prosím akci.";
                }

            }
            ViewBag.ID_ReqType = new SelectList(db.ReqTypes, "ID", "Type");
            ViewBag.ID_Priority = db.PriorityRequirements.ToList();
            ViewBag.ID_Category = new SelectList(db.CategoryRequirements.Where(c => c.ID_Project == projectID), "ID", "Name");
            ViewBag.ID_Status = db.StatusRequirements.ToList();

            return View(requirement);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Requirement requirement = null;
            try
            {
                requirement = db.Requirements.Find(id);
                db.Requirements.Remove(requirement);
                db.SaveChanges();
            }
            catch (Exception ex)
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
            try
            {
                var requirement = db.Requirements.Find(id);
                if (requirement == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.ID_Priority = db.PriorityRequirements.ToList();
                ViewBag.ID_Category = new SelectList(db.CategoryRequirements.Where(c => c.ID_Project == projectID), "ID", "Name", requirement.ID_Requirement);
                ViewBag.ID_Status = db.StatusRequirements.ToList();
                ViewBag.ID_ReqType = new SelectList(db.ReqTypes, "ID", "Type", requirement.ID_ReqType);

                return View(requirement);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Edit(Requirement requirement)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var old = db.Requirements.Find(requirement.ID);
                    old.ID_ReqType = requirement.ID_ReqType;
                    old.Text = requirement.Text;
                    old.ID_Requirement = requirement.ID_Requirement;
                    old.ID_Category = requirement.ID_Category;
                    old.ID_Priority = requirement.ID_Priority;
                    old.ID_Status = requirement.ID_Status;
                    old.Source = requirement.Source;
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
            try
            {
                var requirements = db.Requirements.Where(r => r.ID_Project == projectID && r.ID_ReqType == 1).OrderBy(r => r.ID_Requirement).ToList();
                var usecases = db.UseCases.Where(u => u.ID_Project == projectID).OrderByDescending(i => i.ID).ToList();
                var matrix = new Matrix(requirements, usecases);

                ViewBag.ReqCount = requirements.Count;
                ViewBag.UseCount = usecases.Count;

                var output = matrix.GetMatrix();
                return View(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
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