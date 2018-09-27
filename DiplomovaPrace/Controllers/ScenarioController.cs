using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{

    public class ScenarioController : Controller
    {
        private SDTEntities db = Database.GetDatabase();
        // GET: Scenario
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
            var scenarious = db.Scenarios.Where(s => s.ID_Project == projectID&&s.Done).OrderByDescending(i=>i.ID);
            return View(scenarious);
        }

        private void DeleteDeadScenarious(int projectID)
        {
            var deadScenarious = db.Scenarios.Where(s => s.Done == false && s.ID_Project==projectID);
            if (deadScenarious != null)
            {
                foreach(Scenario scenario in deadScenarious)
                {
                    db.ScenarioActors.RemoveRange(scenario.ScenarioActors);
                    db.Scenarios.Remove(scenario);
                }
                db.SaveChanges();
            }
        }

        public ActionResult Create(int idUseCase)
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
            DeleteDeadScenarious(projectID);
            Scenario scenario;
            if (db.Scenarios.Where(s => s.ID_UseCase == idUseCase).Count()==0)
            {
                scenario = PrepareScenario(projectID, idUseCase,null);
            }
            else
            {
                int? ID_MainScenario = db.Scenarios.Where(s => s.ID_UseCase == idUseCase && s.Done == true && s.ID_MainScenario == null).FirstOrDefault().ID;
                scenario = PrepareScenario(projectID, idUseCase,ID_MainScenario);

            }
        
            List<Actor> mainActors = db.UseCaseActors.Where(u => u.ID_UseCase == idUseCase).Select(s=>s.Actor).ToList();
            ViewBag.mainActors = new MultiSelectList(mainActors, "ID", "Name");
            List<Actor> otherActors = db.Actors.Where(a => a.ID_Project == projectID).ToList();
            otherActors = otherActors.Except(mainActors).ToList();
            LinkedList<Actor> otherActorsList = new LinkedList<Actor>(otherActors);
            otherActorsList.AddFirst(new Actor() { ID = 0, Name = "Žádný" });
            ViewBag.otherActors = new MultiSelectList(otherActorsList, "ID", "Name");
            ViewBag.useCaseID = idUseCase;
            ViewBag.useCaseName = db.UseCases.Find(idUseCase).Name;
            return View(scenario);
        }

        [HttpPost]
        public ActionResult Create(Scenario scenario,List<Int32> otherActors,List<Int32> mainActors)
        {
            Scenario old = db.Scenarios.Find(scenario.ID);
            old.Description = scenario.Description;
            old.InCondition = scenario.InCondition;
            old.OutCondition = scenario.OutCondition;
            old.Scenario1 = scenario.Scenario1;
            old.Done = true;

            AddActors(mainActors, old.ID, 1);
            if (otherActors != null)
            {
                if (!((otherActors.Count == 1) && (otherActors.FirstOrDefault() == 0)))
                {
                    AddActors(otherActors, old.ID, 2);
                }
            }     

            try
            {
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.CREATE_SCENARIO, "/Scenario");

                return RedirectToAction("Index");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            List<Actor> mainActorsList = db.UseCaseActors.Where(u => u.ID_UseCase == old.ID_UseCase).Select(s => s.Actor).ToList();
            ViewBag.mainActors = new MultiSelectList(mainActorsList, "ID", "Name");
            List<Actor> otherActorsList = db.Actors.Where(a => a.ID_Project == old.ID_Project).ToList();
            otherActorsList = otherActorsList.Except(mainActorsList).ToList();
            LinkedList<Actor> otherActorsLinkedList = new LinkedList<Actor>(otherActorsList);
            otherActorsLinkedList.AddFirst(new Actor() { ID = 0, Name = "Žádný" });
            ViewBag.otherActors = new MultiSelectList(otherActorsList, "ID", "Name");
            ViewBag.useCaseID = old.ID_UseCase;
            ViewBag.useCaseName = db.UseCases.Find(old.ID_UseCase).Name;

            return View(scenario);
        }

        private void AddActors(List<Int32> listActors,int idScenario,int actorType)
        {
            if (listActors != null)
            {
                foreach (int index in listActors)
                {
                    ScenarioActor scenarioActor = new ScenarioActor();
                    scenarioActor.ID_Actor = index;
                    scenarioActor.ID_ActorType = actorType;
                    scenarioActor.ID_Scenario = idScenario;
                    db.ScenarioActors.Add(scenarioActor);
                    db.SaveChanges();
                }
            }
        
        }

        private Scenario PrepareScenario(int projectID,int idUseCase, int? ID_MainScenario)
        {
            Scenario scenario = new Scenario();
            scenario.ID_Project = projectID;
            scenario.ID_UseCase = idUseCase;
            scenario.Done = false;
            scenario.ID_Scenario = db.Scenarios.Where(s => s.ID_Project == projectID).Count() + 1;
            scenario.ID_MainScenario = ID_MainScenario;
            db.Scenarios.Add(scenario);
            db.SaveChanges();
            return scenario;
        }

        public ActionResult Details(int id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["projectID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var scenario = db.Scenarios.Find(id);
            return View(scenario);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var scenario = db.Scenarios.Find(id);
            try
            {
                db.ScenarioActors.RemoveRange(scenario.ScenarioActors);
                scenario.ScenarioActors.Clear();
                List<Scenario> alternativeScenarious = db.Scenarios.Where(s => s.ID_MainScenario == id).ToList();
                db.Scenarios.RemoveRange(alternativeScenarious);
                db.Scenarios.Remove(scenario);    
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.DELETE_SCENARIO, "/Scenario");

            }
            catch (Exception ex)
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
            var scenario = db.Scenarios.Find(id);
            List<Actor> mainActors = db.UseCaseActors.Where(u => u.ID_UseCase == scenario.ID_UseCase).Select(s => s.Actor).ToList();
            mainActors = mainActors.Except(scenario.ScenarioActors.Select(a => a.Actor)).ToList();
            ViewBag.mainActors = new MultiSelectList(mainActors, "ID", "Name");
            List<Actor> otherActors = db.Actors.Where(a => a.ID_Project == scenario.ID_Project).ToList();
            otherActors = otherActors.Except(mainActors).ToList();
            otherActors = otherActors.Except(scenario.ScenarioActors.Select(a => a.Actor)).ToList();
            LinkedList<Actor> otherActorsList = new LinkedList<Actor>(otherActors);
            otherActorsList.AddFirst(new Actor() { ID = 0, Name = "Žádný" });
            ViewBag.otherActors = new MultiSelectList(otherActorsList, "ID", "Name");
            return View(scenario);
        }

        [HttpPost]
        public ActionResult DeleteActor(int id, int idScenario)
        {
            var actorScenario = db.ScenarioActors.Where(s => s.ID_Scenario == idScenario && s.ID_Actor == id).FirstOrDefault();
            try
            {
                db.Scenarios.Find(idScenario).ScenarioActors.Remove(actorScenario);
                db.ScenarioActors.Remove(actorScenario);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
     
            return RedirectToAction("Edit", new { id = idScenario });
        }

        [HttpPost]
        public ActionResult Edit(Scenario scenario, List<Int32> mainActors, List<Int32> otherActors)
        {
            Scenario old = db.Scenarios.Find(scenario.ID);
            old.Description = scenario.Description;
            old.InCondition = scenario.InCondition;
            old.OutCondition = scenario.OutCondition;
            old.Scenario1 = scenario.Scenario1;
   
            AddActors(mainActors, old.ID, 1);
            if (otherActors != null)
            {
                if (!((otherActors.Count == 1) && (otherActors.FirstOrDefault() == 0)))
                {
                    AddActors(otherActors, old.ID, 2);
                }
            }
                      
            try
            {
                db.Entry(old).State = EntityState.Modified;
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.EDIT_SCENARIO, "/Scenario");

                return RedirectToAction("Index");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            List<Actor> mainActorsList = db.UseCaseActors.Where(u => u.ID_UseCase == scenario.ID_UseCase).Select(s => s.Actor).ToList();
            mainActorsList = mainActorsList.Except(scenario.ScenarioActors.Select(a => a.Actor)).ToList();
            ViewBag.mainActors = new MultiSelectList(mainActorsList, "ID", "Name");
            List<Actor> otherActorsList = db.Actors.Where(a => a.ID_Project == scenario.ID_Project).ToList();
            otherActorsList = otherActorsList.Except(mainActorsList).ToList();
            otherActorsList = otherActorsList.Except(scenario.ScenarioActors.Select(a => a.Actor)).ToList();
            LinkedList<Actor> otherActorsLinkedList = new LinkedList<Actor>(otherActorsList);
            otherActorsLinkedList.AddFirst(new Actor() { ID = 0, Name = "Žádný" });
            ViewBag.otherActors = new MultiSelectList(otherActorsLinkedList, "ID", "Name");

            return View(scenario);
        }
    }
}