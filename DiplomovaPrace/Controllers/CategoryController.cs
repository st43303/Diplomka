using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
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
            return View();
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
    }
}