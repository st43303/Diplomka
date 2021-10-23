using SDT.Web.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SDT.Web
{
    public class NotificationComponent
    {
        public void RegisterNotification (DateTime currentTime)
        {

            string conStr = ConfigurationManager.ConnectionStrings["sqlConString"].ConnectionString;
            string sqlCommand = @"SELECT [ID], [ID_User], [Message], [URL], [Avatar], [ID_Project] FROM [dbo].[Notification] WHERE [DateNotification] <> @DateNotification";

            using(SqlConnection conn = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, conn);
                cmd.Parameters.AddWithValue("@DateNotification", currentTime);
                if(conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange;

                using(SqlDataReader reader = cmd.ExecuteReader()) { }
            }
        }

        void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if(e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;

                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");
                RegisterNotification(DateTime.Now);
            }
        }

        public List<Notification> GetNotifications(int userID)
        {
            using (SDTEntities db = new SDTEntities())
            {
                return db.Notifications.Where(n => n.ID_User == userID).OrderByDescending(i => i.ID).ToList();
            }
        }
    }
}