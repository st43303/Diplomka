using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDT.Web.Models
{
    [MetadataType(typeof(TaskAttributes))]
    public partial class Task { }
    public class TaskAttributes
    {
        [DisplayName("Zadání úkolu")]
        [StringLength(200,ErrorMessage ="Maximální délka textu je 200 znaků")]
        [Required(ErrorMessage ="Zadání úkolu je povinná položka")]
        public string Text { get; set; }

        [DisplayName("Očekávaný termín dokončení")]
        public Nullable<System.DateTime> Deadline { get; set; }

        [DisplayName("Priorita")]
        [Required(ErrorMessage = "Priorita je povinná položka")]
        public int ID_Priority { get; set; }

        [DisplayName("Stav úkolu")]
        public int ID_State { get; set; }

        [DisplayName("Zadavatel")]
        public int ID_User_Creator { get; set; }
        public int ID_Project { get; set; }


        [DisplayName("Vyřizovatel úkolu")]
        [Required(ErrorMessage = "Vyřizovatel je povinná položka")]
        public int ID_User_Executor { get; set; }

        [DisplayName("Dokončeno")]
        public Nullable<System.DateTime> DateFinished { get; set; }
    }
}