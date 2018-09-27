using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Controllers
{
    

    public class NotificationSystem
    {
        public static void SendNotification(EnumNotification notificationType, string url)
        {
            int projectID = (int)HttpContext.Current.Session["projectID"];
            int userID = (int)HttpContext.Current.Session["userID"];
            User sender = Database.GetDatabase().Users.Find(userID);

            List<ProjectUser> receivers = Database.GetDatabase().ProjectUsers.Where(p => p.ID_Project == projectID && p.ID_User != userID).ToList();

            string projectName = Database.GetDatabase().Projects.Find(projectID).Name;
            string message = "Uživatel "+sender.Name+" "+sender.Surname;
            switch (notificationType)
            {
                case EnumNotification.CREATE_ACTOR:
                    message += CreateActor();
                    message += projectName + ".";
                    break;
                case EnumNotification.EDIT_ACTOR:
                    message += EditActor();
                    message += projectName + ".";
                    break;
                case EnumNotification.DELETE_ACTOR:
                    message += DeleteActor();
                    message += projectName + ".";
                    break;
                case EnumNotification.CREATE_REQUIREMENT:
                    message += CreateRequirement();
                    message += projectName + ".";
                    break;
                case EnumNotification.EDIT_REQUIREMENT:
                    message += EditRequirement();
                    message += projectName + ".";
                    break;
                case EnumNotification.DELETE_REQUIREMENT:
                    message += DeleteRequirement();
                    message += projectName + ".";
                    break;
                case EnumNotification.CREATE_USECASE:
                    message += CreateUseCase();
                    message += projectName + ".";
                    break;
                case EnumNotification.EDIT_USECASE:
                    message += EditUseCase();
                    message += projectName + ".";
                    break;
                case EnumNotification.DELETE_USECASE:
                    message += DeleteUseCase();
                    message += projectName + ".";
                    break;
                case EnumNotification.CREATE_SCENARIO:
                    message += CreateScenario();
                    message += projectName + ".";
                    break;
                case EnumNotification.EDIT_SCENARIO:
                    message += EditScenario();
                    message += projectName + ".";
                    break;
                case EnumNotification.DELETE_SCENARIO:
                    message += DeleteScenario();
                    message += projectName + ".";
                    break;
                case EnumNotification.CREATE_FILE:
                    message += CreateFile();
                    message += projectName + ".";
                    break;
                case EnumNotification.DELETE_FILE:
                    message += DeleteFile();
                    message += projectName + ".";
                    break;
            }

            foreach(ProjectUser projectUser in receivers)
            {
                Notification notification = new Notification();
                notification.Avatar = sender.Avatar;
                notification.ID_User = projectUser.ID_User;
                notification.Message = message;
                notification.URL = url;
                Database.GetDatabase().Notifications.Add(notification);
                Database.GetDatabase().SaveChanges();
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

    }
}