using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    [MetadataType(typeof(ActorAttributes))]
    public partial class Actor
    {

    }
    public class ActorAttributes
    {
        [DisplayName("Jméno aktéra")]
        [Required(ErrorMessage ="Jméno aktéra je povinná položka")]
        [StringLength(50,ErrorMessage ="Maximální délka jména je 50 znaků")]
        public string Name { get; set; }
    }
}