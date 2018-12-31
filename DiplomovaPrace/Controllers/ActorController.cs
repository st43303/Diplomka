using DiplomovaPrace.Models;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class ActorController : Controller
    {
        private SDTEntities db = new SDTEntities();
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
            try
            {
                var actors = db.Actors.Where(a => a.ID_Project == projectID).OrderByDescending(d => d.ID).AsQueryable();
                return View(actors);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
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
            try
            {
                var actor = db.Actors.Find(id);
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
            try
            {
                var actor = db.Actors.Find(id);
                actor.Name = editActor;
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
            ViewBag.Count = int.Parse(count);
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


                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(csvTable);
                    ViewBag.JSONTable = JSONString;
                    return View(csvTable);
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

        public ActionResult ActorImport(string table)
        {
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(table);
            int projectID = (int)Session["projectID"];
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        if (col.ColumnName == "Name")
                        {
                            string name = row[col.ColumnName].ToString();
                            Actor actor = new Actor();
                            actor.Name = name;
                            actor.ID_Project = projectID;
                            db.Actors.Add(actor);
                            db.SaveChanges();
                        }
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
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