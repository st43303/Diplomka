using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDT.Web.Models
{
    [MetadataType(typeof(UseCaseAttributes))]
    public partial class UseCase
    {

    }
    public class UseCaseAttributes
    {
        [DisplayName("Název")]
        [Required(ErrorMessage = "Název je povinná položka")]
        [StringLength(50, ErrorMessage = "Maximální délka je 50 znaků")]
        public string Name { get; set; }

        [DisplayName("Popis")]
        [Required(ErrorMessage = "Popis je povinná položka")]
        [StringLength(150, ErrorMessage = "Maximální délka je 150 znaků")]
        public string Description { get; set; }
    }
}