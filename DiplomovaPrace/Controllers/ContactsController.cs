using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    public class ContactsController : Controller
    {
        private SDTEntities db = Database.GetDatabase();
        // GET: Contacts
        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            var contacts = db.Friendships.Where(f => f.ID_UserA == userID || f.ID_UserB == userID);
            contacts = contacts.Where(f => f.Checked == true);
          
            return View(contacts);
        }

        public ActionResult Delete(int id)
        {
            int userID = (int)Session["userID"];
            Friendship friendship = db.Friendships.Where(f => f.ID_UserA == id && f.ID_UserB == userID).FirstOrDefault();
            if (friendship == null)
            {
                friendship = db.Friendships.Where(f => f.ID_UserA == userID && f.ID_UserB == id).FirstOrDefault();
            }
            try
            {
                db.Friendships.Remove(friendship);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
         
            return RedirectToAction("Details","Profile",new { id});
        }

        public ActionResult Requests()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];

            var requests = db.Friendships.Where(u => u.ID_UserB==userID && u.Checked == false);

            return View(requests.ToList());
        }

        public ActionResult AcceptRequest(int id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["userID"];
            var friendship = db.Friendships.Find(id);
            Notification notification = new Notification();
            notification.ID_User = friendship.ID_UserA;
            notification.Message = "Uživatel " + friendship.User1.Name + " " + friendship.User1.Surname + " přijal Vaši žádost o navázání kontaktu.";
            notification.URL = "/Profile/Details/" + userID;
            notification.Avatar = friendship.User1.Avatar;
            friendship.Checked = true;
            if (friendship != null)
            {
                try
                {
                    db.Entry(friendship).State = EntityState.Modified;
                    db.Notifications.Add(notification);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    ViewBag.Error = "Nastala chyba. Opakujte prosím akci.";
                    Console.WriteLine(ex.Message);
                }
            }

            return RedirectToAction("Requests");
        }
    }
}