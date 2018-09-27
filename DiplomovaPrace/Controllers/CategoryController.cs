using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class CategoryController : Controller
    {
        private SDTEntities db = Database.GetDatabase();

        // GET: Category
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
            var categories = db.CategoryRequirements.Where(c => c.ID_Project == projectID);
            return View(categories);
        }

        [HttpPost]
        public ActionResult Create(String name, int create,int id)
        {
            CategoryRequirement category = new CategoryRequirement();
            int projectID = (int)Session["projectID"];
            category.ID_Project = projectID;
            category.Name = name;

            try
            {
                db.CategoryRequirements.Add(category);
                db.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (create == 1)
            {
                return RedirectToAction("Create", "Requirements");
            }
            else if (create == 2)
            {
                return RedirectToAction("Edit", "Requirements", new { id });
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var category = db.CategoryRequirements.Find(id);
            try
            {
                db.CategoryRequirements.Remove(category);
                db.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(int id, string editCategory)
        {
            var category = db.CategoryRequirements.Find(id);
            category.Name = editCategory;
            try
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");
        }
    }
}