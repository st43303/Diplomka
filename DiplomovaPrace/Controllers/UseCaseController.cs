using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class UseCaseController : Controller
    {
        private SDTEntities db = new SDTEntities();
        // GET: UseCase
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

            var useCases = db.UseCases.Where(u => u.ID_Project == projectID).OrderByDescending(i => i.ID).AsQueryable();
            return View(useCases);
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
            ViewBag.actors = new MultiSelectList(db.Actors.Where(a => a.ID_Project == projectID), "ID", "Name");
            ViewBag.requirements = new MultiSelectList((from s in db.Requirements.Where(c => c.ID_Project == projectID && c.ID_ReqType == 1) select new { s.ID, FullReq = s.ID_Requirement + " " + s.Text }), "ID", "FullReq");
            return View();
        }

        [HttpPost]
        public ActionResult Create(UseCase useCase, List<int> actors, List<Int32> requirements)
        {
            int projectID = (int)Session["projectID"];
            useCase.ID_Project = projectID;

            try
            {
                db.UseCases.Add(useCase);
                db.SaveChanges();
                Session["actors"] = actors;
                Session["requirements"] = requirements;
                NotificationSystem.SendNotification(EnumNotification.CREATE_USECASE, "/UseCase");
                return RedirectToAction("AddActors");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            ViewBag.actors = new MultiSelectList(db.Actors.Where(a => a.ID_Project == projectID), "ID", "Name");
            ViewBag.requirements = new MultiSelectList((from s in db.Requirements.Where(c => c.ID_Project == projectID && c.ID_ReqType == 1) select new { s.ID, FullReq = s.ID_Requirement + " " + s.Text }), "ID", "FullReq");
            return View(useCase);
        }


        public ActionResult AddActors()
        {
            List<int> actors = (List<Int32>)Session["actors"];
            int useCaseID = db.UseCases.OrderByDescending(i => i.ID).First().ID;
            foreach (int i in actors)
            {
                UseCaseActor caseActor = new UseCaseActor();
                caseActor.ID_Actor = i;
                caseActor.ID_UseCase = useCaseID;
                db.UseCaseActors.Add(caseActor);
            }

            db.SaveChanges();

            return RedirectToAction("AddRequirements");
        }

        public ActionResult AddRequirements()
        {
            List<int> requirements = (List<int>)Session["requirements"];
            int useCaseID = db.UseCases.OrderByDescending(i => i.ID).First().ID;
            foreach (int i in requirements)
            {
                UseCaseRequirement caseRequirement = new UseCaseRequirement();
                caseRequirement.ID_Requirement = i;
                caseRequirement.ID_UseCase = useCaseID;
                db.UseCaseRequirements.Add(caseRequirement);

            }
            db.SaveChanges();
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
            int projectID = (int)Session["projectID"];

            var useCase = db.UseCases.Find(id);
            List<int> actors = new List<int>();
            foreach (UseCaseActor useCaseActor in useCase.UseCaseActors)
            {
                actors.Add(useCaseActor.ID);
            }

            ViewBag.actors = new MultiSelectList(GetActors(useCase), "ID", "Name");
            ViewBag.requirements = new MultiSelectList((from s in GetRequirements(useCase) select new { s.ID, FullReq = s.ID_Requirement + " " + s.Text }), "ID", "FullReq");

            return View(useCase);
        }


        [HttpPost]
        public ActionResult Edit(UseCase useCase, List<int> actors, List<int> requirements)
        {
            var old = db.UseCases.Find(useCase.ID);
            old.Name = useCase.Name;
            old.Description = useCase.Description;

            if (actors != null)
            {
                for (int i = 0; i < actors.Count; i++)
                {
                    UseCaseActor useCaseActor = new UseCaseActor();
                    useCaseActor.ID_Actor = actors[i];
                    useCaseActor.ID_UseCase = old.ID;
                    db.UseCaseActors.Add(useCaseActor);
                }
            }
            if (requirements != null)
            {
                for (int i = 0; i < requirements.Count; i++)
                {
                    UseCaseRequirement useCaseRequirement = new UseCaseRequirement();
                    useCaseRequirement.ID_Requirement = requirements[i];
                    useCaseRequirement.ID_UseCase = old.ID;
                    db.UseCaseRequirements.Add(useCaseRequirement);
                }
            }

            try
            {
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.EDIT_USECASE, "/UseCase");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            List<int> actorsList = new List<int>();
            foreach (UseCaseActor useCaseActor in useCase.UseCaseActors)
            {
                actorsList.Add(useCaseActor.ID);
            }

            ViewBag.actors = new MultiSelectList(GetActors(old), "ID", "Name");
            ViewBag.requirements = new MultiSelectList(GetRequirements(old), "ID", "Text");

            return View(useCase);
        }

        private List<Requirement> GetRequirements(UseCase useCase)
        {
            List<Requirement> list = new List<Requirement>();
            int projectID = (int)Session["projectID"];
            foreach (Requirement req in db.Requirements)
            {
                if (req.ID_Project == projectID)
                {
                    bool exist = false;
                    foreach (UseCaseRequirement useCaseReq in useCase.UseCaseRequirements)
                    {
                        if (req.ID == useCaseReq.ID_Requirement)
                        {
                            exist = true;
                        }
                    }
                    if (!exist)
                    {
                        list.Add(req);
                    }
                }
            }
            return list;
        }

        private List<Actor> GetActors(UseCase useCase)
        {
            List<Actor> list = new List<Actor>();
            int projectID = (int)Session["projectID"];
            foreach (Actor actor in db.Actors)
            {
                if (actor.ID_Project == projectID)
                {
                    bool exist = false;
                    foreach (UseCaseActor useCaseActor in useCase.UseCaseActors)
                    {
                        if (actor.ID == useCaseActor.ID_Actor)
                        {
                            exist = true;
                        }
                    }
                    if (!exist)
                    {
                        list.Add(actor);
                    }

                }
            }
            return list;
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var useCase = db.UseCases.Find(id);
            try
            {
                db.Scenarios.RemoveRange(useCase.Scenarios);
                useCase.Scenarios.Clear();
                db.UseCaseActors.RemoveRange(useCase.UseCaseActors);
                useCase.UseCaseActors.Clear();
                db.UseCases.Remove(useCase);
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.DELETE_USECASE, "/UseCase");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteActor(int id, int idUseCase)
        {
            var useCaseActor = db.UseCaseActors.Find(id);
            try
            {
                db.UseCaseActors.Remove(useCaseActor);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Edit", new { id = idUseCase });
        }

        [HttpPost]
        public ActionResult DeleteRequirement(int id, int idUseCase)
        {
            var useCaseRequirement = db.UseCaseRequirements.Find(id);
            try
            {
                db.UseCaseRequirements.Remove(useCaseRequirement);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Edit", new { id = idUseCase });
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