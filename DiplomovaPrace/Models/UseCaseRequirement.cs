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
    
    public partial class UseCaseRequirement
    {
        public int ID { get; set; }
        public int ID_UseCase { get; set; }
        public int ID_Requirement { get; set; }
    
        public virtual Requirement Requirement { get; set; }
        public virtual UseCase UseCase { get; set; }
    }
}
