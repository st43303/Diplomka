using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    [MetadataType(typeof(ProjectAttributes))]
    public partial class Project
    {

    }
    public class ProjectAttributes
    {
        [DisplayName("Název projektu")]
        [Required(ErrorMessage ="Název projektu je povinná položka")]
        [StringLength(20,ErrorMessage ="Maximální délka je 20 znaků")]
        public string Name { get; set; }

        [DisplayName("Popis projektu")]
        [Required(ErrorMessage = "Popis projektu je povinná položka")]
        [StringLength(50, ErrorMessage = "Maximální délka je 50 znaků")]
        public string Description { get; set; }

        [DisplayName("Autor projektu")]
        public int ID_Author { get; set; }

        [DisplayName("Datum vytvoření projektu")]
        public Nullable<System.DateTime> DateCreated { get; set; }
    }
}