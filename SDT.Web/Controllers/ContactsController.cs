using SDT.Web.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SDT.Web.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class ContactsController : Controller
    {
        private SDTEntities db = new SDTEntities();
        // GET: Contacts
        public ActionResult Index()
        {

            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            try
            {
                var contacts = db.Friendships.Where(f => f.ID_UserA == userID || f.ID_UserB == userID);
                contacts = contacts.Where(f => f.Checked == true).AsQueryable();
                return View(contacts);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        public ActionResult Delete(int id)
        {

            int userID = (int)Session["userID"];
       
            try
            {
                Friendship friendship = db.Friendships.Where(f => f.ID_UserA == id && f.ID_UserB == userID).FirstOrDefault();
                if (friendship == null)
                {
                    friendship = db.Friendships.Where(f => f.ID_UserA == userID && f.ID_UserB == id).FirstOrDefault();
                }
                db.Friendships.Remove(friendship);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Details", "Profile", new { id });

        }

        public ActionResult Requests()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            try
            {
                var requests = db.Friendships.Where(u => u.ID_UserB == userID && u.Checked == false).AsQueryable();
                return View(requests);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }

        public ActionResult AcceptRequest(int id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];

                try
                {
                    var friendship = db.Friendships.Find(id);
                    Notification notification = new Notification();
                    notification.ID_User = friendship.ID_UserA;
                    notification.Message = "Uživatel " + friendship.User1.Name + " " + friendship.User1.Surname + " přijal Vaši žádost o navázání kontaktu.";
                    notification.URL = "/Profile/Details/" + userID;
                    notification.Avatar = friendship.User1.Avatar;
                    notification.DateNotification = DateTime.Now;
                    friendship.Checked = true;
                    if(friendship != null)
                    {
                        db.Entry(friendship).State = EntityState.Modified;
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }
         
                }
                catch(Exception ex)
                {
                    ViewBag.Error = "Nastala chyba. Opakujte prosím akci.";
                    Console.WriteLine(ex.Message);
            }
         
            return RedirectToAction("Requests");
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