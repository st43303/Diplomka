using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDT.Web.Models
{
    [MetadataType(typeof(ScenarioAttributes))]
    public partial class Scenario
    {
    }

    public class ScenarioAttributes
    {
        [DisplayName("Jedinečný identifikátor")]
        [Required(ErrorMessage = "Identifikátor je povinná položka")]
        [Range(0,Int32.MaxValue,ErrorMessage ="Identifikátor musí být celočíselná hodnota")]
        public int ID_Scenario { get; set; }

        [DisplayName("Popis")]
        [StringLength(100, ErrorMessage = "Maximální délka je 100 znaků")]
        public string Description { get; set; }

        [DisplayName("Vstupní podmínky")]
        [StringLength(100, ErrorMessage = "Maximální délka je 100 znaků")]
        public string InCondition { get; set; }

        [DisplayName("Výstupní podmínky")]
        [StringLength(100, ErrorMessage = "Maximální délka je 100 znaků")]
        public string OutCondition { get; set; }


        public int ID_UseCase { get; set; }

        [DisplayName("Hlavní scénář")]
        [StringLength(1000,ErrorMessage ="Maximální délka je 1000 znaků")]
        [AllowHtml]
        public string Scenario1 { get; set; }


    }


}