using DiplomovaPrace.Models;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class ActorController : Controller
    {
        private SDTEntities db = Database.GetDatabase();
        // GET: Actor
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
            var actors = db.Actors.Where(a => a.ID_Project == projectID).OrderByDescending(d=>d.ID);
            return View(actors);
        }

        [HttpPost]

        public ActionResult Create(string name)
        {
            int projectID = (int)Session["projectID"];
            Actor actor = new Actor();
            actor.Name = name;
            actor.ID_Project = projectID;
            try
            {
                db.Actors.Add(actor);
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.CREATE_ACTOR, "/Actor");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var actor = db.Actors.Find(id);
            try
            {
                db.Actors.Remove(actor);
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.DELETE_ACTOR, "/Actor");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(int id, string editActor)
        {
            var actor = db.Actors.Find(id);
            actor.Name = editActor;

            try
            {
                db.Entry(actor).State = EntityState.Modified;
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.EDIT_ACTOR, "/Actor");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Import()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (Session["projectID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Count = 5;
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase upload, string Header, string Encoding, string Delimiter, string count)
        {
            bool hasHeaders = Header.Equals("true");
            char delimiter = ';';
            ViewBag.Count = Int32.Parse(count);
            switch (Delimiter)
            {
                case ",":
                    delimiter = ',';
                    break;
                case ";":
                    delimiter = ';';
                    break;
                case "|":
                    delimiter = '|';
                    break;
                case ":":
                    delimiter = ':';
                    break;
                case " ":
                    delimiter = ' ';
                    break;
            }

            if (upload != null && upload.ContentLength > 0)
            {

                if (upload.FileName.EndsWith(".csv"))
                {
                    Stream stream = upload.InputStream;
                    DataTable csvTable = new DataTable();
                    CsvReader csvReader = new CsvReader(new StreamReader(stream), hasHeaders, delimiter);
                    csvTable.Load(csvReader);

                    ImportData import = new ImportData();
                    import.DataTable = csvTable;
                    ViewBag.Table = import;
                    return View(import);
                }
                else
                {
                    ModelState.AddModelError("File", "This file format is not supported");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("File", "Please Upload Your file");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ActorImport(ImportData DataTable)
        {
            return RedirectToAction("Index");
        }
    }
}