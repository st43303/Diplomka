using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDT.Web.Models
{
    [MetadataType(typeof(FileAttributes))]
    public partial class File { }
    public class FileAttributes
    {
        public string Path { get; set; }

        [DisplayName("Typ souboru")]
        public int ID_File_Type { get; set; }

        [DisplayName("Datum vložení")]
        public System.DateTime Date_Uploaded { get; set; }

        [DisplayName("Vlastník souboru")]
        public int ID_User { get; set; }

        [DisplayName("Název souboru")]
        [StringLength(50,ErrorMessage ="Maximální délka textu je 50 znaků")]
        [Required(ErrorMessage ="Název souboru je povinná položka")]
        public string Name { get; set; }

        [DisplayName("Velikost souboru")]
        public string Length { get; set; }
    }
}