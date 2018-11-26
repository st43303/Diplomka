using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    [MetadataType(typeof(UserAttributes))]
    public partial class User { }
    public class UserAttributes
    {
        [DisplayName("Uživatelské jméno")]
        [StringLength(10,ErrorMessage ="Maximální délka textu je 10 znaků")]
        [Required(ErrorMessage ="Uživatelské jméno je povinná položka")]
        public string Username { get; set; }

        [DisplayName("Heslo")]
        [StringLength(100,ErrorMessage ="Maximální délka textu je 100 znaků")]
        [Required(ErrorMessage="Heslo je povinná položka")]
        public string Password { get; set; }

        [DisplayName("Jméno")]
        [StringLength(50, ErrorMessage = "Maximální délka textu je 50 znaků")]
        public string Name { get; set; }

        [DisplayName("Příjmení")]
        [StringLength(50, ErrorMessage = "Maximální délka textu je 50 znaků")]
        public string Surname { get; set; }

        [DisplayName("Město")]
        [StringLength(50, ErrorMessage = "Maximální délka textu je 50 znaků")]
        public string City { get; set; }
        public string Avatar { get; set; }

        [DisplayName("Datum narození")]
        public Nullable<System.DateTime> BirthDate { get; set; }

        [DisplayName("Datum registrace")]
        public System.DateTime RegistrationDate { get; set; }

        [DisplayName("Poslední přihlášení")]
        public Nullable<System.DateTime> LastActive { get; set; }
    }
}