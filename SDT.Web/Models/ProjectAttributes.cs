﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDT.Web.Models
{
    [MetadataType(typeof(ProjectAttributes))]
    public partial class Project
    {
        public string DescriptionPreview
        {
            get
            {
                if (Description.Length <= 50)
                {
                    return Description;
                }

                return Description.Substring(0, 50) + "...";
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

        [DisplayName("Zkratka projektu")]
        [Required(ErrorMessage ="Zkratka projektu je povinná položka")]
        [StringLength(20,ErrorMessage ="Maximální délka je 20 znaků")]
        public string Code { get; set; }

        [DisplayName("Hodnota WIP")]
        [Required(ErrorMessage ="Hodnota WIP je povinná položka")]
        [Range(1,int.MaxValue,ErrorMessage ="Minimální možná hodnota je 1")]
        public int WIP { get; set; }


    }
}