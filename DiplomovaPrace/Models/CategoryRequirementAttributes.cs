using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    [MetadataType(typeof(CategoryRequirementAttributes))]
    public partial class CategoryRequirement
    {

    }
    public class CategoryRequirementAttributes
    {
        [DisplayName("Název kategorie")]
        [Required(ErrorMessage ="Název kategorie je povinná položka")]
        [StringLength(50,ErrorMessage ="Maximální délka názvu je 50 znaků")]
        public string Name { get; set; }
    }
}