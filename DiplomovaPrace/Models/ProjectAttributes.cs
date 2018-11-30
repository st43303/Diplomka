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
        public string DescriptionPreview
        {
            get
            {
                return Description.Substring(0, Description.Length / 4) + "...";
            }
        }
    }
    public class ProjectAttributes
    {
        [DisplayName("Název projektu")]
        [Required(ErrorMessage ="Název projektu je povinná položka")]
        [StringLength(50,ErrorMessage ="Maximální délka je 50 znaků")]
        public string Name { get; set; }

        [DisplayName("Popis projektu")]
        [Required(ErrorMessage = "Popis projektu je povinná položka")]
        [StringLength(400, ErrorMessage = "Maximální délka je 400 znaků")]
        public string Description { get; set; }

        [DisplayName("Autor projektu")]
        public int ID_Author { get; set; }

        [DisplayName("Datum vytvoření projektu")]
        public Nullable<System.DateTime> DateCreated { get; set; }

   
    }
}