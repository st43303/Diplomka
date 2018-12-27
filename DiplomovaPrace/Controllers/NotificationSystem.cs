using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Controllers
{
    

    public class NotificationSystem
    {
        private SDTEntities db = new SDTEntities();
        public static void SendNotification(EnumNotification notificationType, string url)
        {
            SDTEntities db = new SDTEntities();
            int projectID = (int)HttpContext.Current.Session["projectID"];
            int userID = (int)HttpContext.Current.Session["userID"];
            User sender = db.Users.Find(userID);

            List<ProjectUser> receivers = db.ProjectUsers.Where(p => p.ID_Project == projectID && p.ID_User != userID).ToList();

            string projectName = db.Projects.Find(projectID).Name;
            string message = "Uživatel "+sender.Name+" "+sender.Surname;
            switch (notificationType)
            {
                case EnumNotification.CREATE_ACTOR:
                    message += CreateActor();
                    break;
                case EnumNotification.EDIT_ACTOR:
                    message += EditActor();
                    break;
                case EnumNotification.DELETE_ACTOR:
                    message += DeleteActor();
                    break;
                case EnumNotification.CREATE_REQUIREMENT:
                    message += CreateRequirement();
                    break;
                case EnumNotification.EDIT_REQUIREMENT:
                    message += EditRequirement();
                    break;
                case EnumNotification.DELETE_REQUIREMENT:
                    message += DeleteRequirement();
                    break;
                case EnumNotification.CREATE_USECASE:
                    message += CreateUseCase();
                    break;
                case EnumNotification.EDIT_USECASE:
                    message += EditUseCase();
                    break;
                case EnumNotification.DELETE_USECASE:
                    message += DeleteUseCase();
                    break;
                case EnumNotification.CREATE_SCENARIO:
                    message += CreateScenario();
                    break;
                case EnumNotification.EDIT_SCENARIO:
                    message += EditScenario();
                    break;
                case EnumNotification.DELETE_SCENARIO:
                    message += DeleteScenario();
                    break;
                case EnumNotification.CREATE_FILE:
                    message += CreateFile();
                    break;
                case EnumNotification.DELETE_FILE:
                    message += DeleteFile();
                    break;
                case EnumNotification.CREATE_TASK:
                    message += CreateTask();
                    break;
                case EnumNotification.CHANGE_TASK:
                    message += ChangeTask();
                    break;
                case EnumNotification.DELETE_TASK:
                    message += DeleteTask();
                    break;
            }
            message += projectName + ".";

            foreach(ProjectUser projectUser in receivers)
            {
                Notification notification = new Notification();
                notification.Avatar = sender.Avatar;
                notification.ID_User = projectUser.ID_User;
                notification.Message = message;
                notification.URL = url;
                notification.DateNotification = DateTime.Now;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }


        }

        private static string CreateActor()
        {
            return " vytvořil nového aktéra v projektu ";
        }

        private static string EditActor()
        {
            return " upravil aktéra v projektu ";
        }

        private static string DeleteActor()
        {
            return " odstranil aktéra z projektu ";
        }

        private static string CreateRequirement()
        {
            return " vytvořil nový požadavek v projektu ";
        }

        private static string EditRequirement()
        {
            return " upravil požadavek v projektu ";
        }

        private static string DeleteRequirement()
        {
            return " odstranil požadavek z projektu ";
        }

        private static string CreateUseCase()
        {
            return " vytvořil nový případ užití v projektu ";
        } 

        private static string EditUseCase()
        {
            return " upravil případ užití v projektu ";
        }

        private static string DeleteUseCase()
        {
            return " odstranil případ užití z projektu ";
        }

        private static string CreateScenario()
        {
            return " vytvořil nový scénář v projektu ";
        }

        private static string EditScenario()
        {
            return " upravil scénář v projektu ";
        }

        private static string DeleteScenario()
        {
            return " odstranil scénář z projektu ";
        }

        private static string CreateFile()
        {
            return " přidal nový soubor do projektu ";
        }

        private static string DeleteFile()
        {
            return " odstranil svůj soubor z projektu ";
        }

        private static string CreateTask()
        {
            return " vytvořil nový úkol v projektu ";
        }

        private static string ChangeTask()
        {
            return " změnil stav úkolu v projektu ";
        }

        private static string DeleteTask()
        {
            return " odstranil úkol z projektu ";
        }

    }
}