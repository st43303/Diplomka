using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DiplomovaPrace.Models;

namespace DiplomovaPrace.Controllers
{
    public class Database
    {
        private static SDTEntities db;
        public static SDTEntities GetDatabase()
        {
            if (db == null)
            {
                db = new SDTEntities();
            }
            return db;
        }
    }
}