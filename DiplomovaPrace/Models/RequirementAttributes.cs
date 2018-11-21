using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    [MetadataType(typeof(RequirementAttributes))]
    public partial class Requirement { }
    public class RequirementAttributes
    {
        [DisplayName("ID")]
        [Required(ErrorMessage = "ID je povinná položka")]
        [StringLength(50, ErrorMessage = "Maximální délka je 50 znaků")]
        public string ID_Requirement { get; set; }

        [DisplayName("Text")]
        [Required(ErrorMessage = "Text je povinná položka")]
        [StringLength(50, ErrorMessage = "Maximální délka je 50 znaků")]
        public string Text { get; set; }

        [DisplayName("Typ")]
        [Required(ErrorMessage = "Typ je povinná položka")]
        public int ID_ReqType { get; set; }

        [DisplayName("Kategorie")]
        public Nullable<int> ID_Category { get; set; }

        [DisplayName("Priorita")]
        public Nullable<int> ID_Priority { get; set; }

        [DisplayName("Status")]
        public Nullable<int> ID_Status { get; set; }

        [DisplayName("Zdroj")]
        public string Source { get; set; }
    }
}