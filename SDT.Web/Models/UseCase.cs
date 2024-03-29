//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SDT.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UseCase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UseCase()
        {
            this.Scenarios = new HashSet<Scenario>();
            this.UseCaseActors = new HashSet<UseCaseActor>();
            this.UseCaseRequirements = new HashSet<UseCaseRequirement>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ID_Project { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scenario> Scenarios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UseCaseActor> UseCaseActors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UseCaseRequirement> UseCaseRequirements { get; set; }
        public virtual Project Project { get; set; }
        public virtual UseCase UseCase1 { get; set; }
        public virtual UseCase UseCase2 { get; set; }
    }
}
