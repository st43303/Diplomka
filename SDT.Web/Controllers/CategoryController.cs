using SDT.Web.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SDT.Web.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class CategoryController : Controller
    {
        private SDTEntities db = new SDTEntities();
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

            try
            {
                var categories = db.CategoryRequirements.Where(c => c.ID_Project == projectID).AsQueryable();
                return View(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(string name, int create,int id)
        {
            CategoryRequirement category = new CategoryRequirement();
            int projectID = (int)Session["projectID"];
            category.ID_Project = projectID;
            category.Name = name;

            try
            {
                db.CategoryRequirements.Add(category);
                db.SaveChanges();
                if (create == 1)
                {
                    return RedirectToAction("Create", "Requirements");
                }
                else if (create == 2)
                {
                    return RedirectToAction("Edit", "Requirements", new { id });
                }
            }
            catch(Exception ex)
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
                var category = db.CategoryRequirements.Find(id);
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
            try
            {
                var category = db.CategoryRequirements.Find(id);
                category.Name = editCategory;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
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