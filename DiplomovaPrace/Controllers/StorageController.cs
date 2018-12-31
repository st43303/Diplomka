using DiplomovaPrace.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace DiplomovaPrace.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class StorageController : Controller
    {
        private SDTEntities db = new SDTEntities();
        // GET: Storage
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
            var files = db.Files.Where(f => f.ID_Project == projectID).AsQueryable();

            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            HttpRuntimeSection section = config.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
            double maxFileSize = section.MaxRequestLength;
            ViewBag.FileSize = "Maximální velikost souboru je " + GetLength(maxFileSize);
            return View(files);
        }

        [HttpPost]
        public ActionResult Create(string Name, HttpPostedFileBase File)
        {
            int projectID = (int)Session["projectID"];
            int userID = (int)Session["userID"];
            Models.File file = new Models.File();
            file.Date_Uploaded = DateTime.Now;
            file.Name = Name;
            file.ID_Project = projectID;
            file.ID_User = userID;
            file.ID_File_Type = GetTypeID(File);
            file.TypeFile = Path.GetExtension(File.FileName).Substring(1);
            file.Path = SaveFile(File);
            file.Length = GetLength(File);

            try
            {
                db.Files.Add(file);
                db.SaveChanges();
                NotificationSystem.SendNotification(EnumNotification.CREATE_FILE, "/Storage");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }

        public FileResult Download(int id)
        {
            Models.File file = db.Files.Find(id);
            string extension = file.TypeFile;
            byte[] fileBytes = System.IO.File.ReadAllBytes(file.Path);
            string fileName = file.Name + "."+extension;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        private String GetLength(double length)
        {
            string result;
            if ((length / 1024) >= 1)
            {
                //kilobytes
                length = length / 1024;
                result = length + " MB";
                if ((length / 1024) >= 1)
                {
                    //megabytes
                    length = length / 1024;
                    result = length + " GB";
                }
            }
            else
            {
                result = length + " kB";
            }
            return result;
        }

        private string GetLength(HttpPostedFileBase file)
        {
            int length = file.ContentLength;
            string result;
            if ((length / 1024) >= 1)
            {
                //kilobytes
                length = length / 1024;
                result = length + " kB";
                if ((length / 1024) >= 1)
                {
                    //megabytes
                    length = length / 1024;
                    result = length + " MB";
                }
            }
            else
            {
                result = length + " B";
            }
            return result;
        }

    
        private int GetTypeID(HttpPostedFileBase file)
        {
            string extension = Path.GetExtension(file.FileName);
            extension = extension.Substring(1);
            FileType fileType = db.FileTypes.Where(f => f.Extension.Equals(extension)).FirstOrDefault();
            if (fileType != null)
            {
                return fileType.ID;
            }
            else
            {
                return db.FileTypes.Where(f => f.Extension.Equals("txt")).FirstOrDefault().ID;
            }
        }
        private string SaveFile(HttpPostedFileBase AvatarFile)
        {
            string filename = Path.GetFileNameWithoutExtension(AvatarFile.FileName);
            string extension = Path.GetExtension(AvatarFile.FileName);
            filename = filename + DateTime.Now.ToString("ddmmyyy") + extension;
            filename = Path.Combine(Server.MapPath("~/files/"), filename);
            AvatarFile.SaveAs(filename);
            return filename;
        }

        [HttpPost]
        public ActionResult Delete (int id)
        {
            var file = db.Files.Find(id);
            try
            {
                if (System.IO.File.Exists(file.Path))
                {
                    System.IO.File.Delete(file.Path);
                    db.Files.Remove(file);
                    db.SaveChanges();
                    NotificationSystem.SendNotification(EnumNotification.DELETE_FILE, "/Storage");

                }

            }
            catch(Exception ex)
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