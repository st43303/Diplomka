//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiplomovaPrace.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ScenarioView
    {
        public string ActorName { get; set; }
        public string ActorType { get; set; }
        public int ID_Project { get; set; }
        public int ID_Scenario { get; set; }
        public string Description { get; set; }
        public string InCondition { get; set; }
        public string OutCondition { get; set; }
        public string Text { get; set; }
        public bool Done { get; set; }
        public Nullable<int> ID_MainScenario { get; set; }
        public string UseCaseName { get; set; }
    }
}
