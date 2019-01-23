using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    public class UserProject
    {
        public int ID_Project { get; set; }
        public bool Remove { get; set; }
        public int New_Author { get; set; }
        public virtual Project Project { get; set; }
    }
}