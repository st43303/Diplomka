﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SDTEntities : DbContext
    {
        public SDTEntities()
            : base("name=SDTEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<ActorType> ActorTypes { get; set; }
        public virtual DbSet<CategoryRequirement> CategoryRequirements { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<PriorityRequirement> PriorityRequirements { get; set; }
        public virtual DbSet<PriorityTask> PriorityTasks { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectTechnology> ProjectTechnologies { get; set; }
        public virtual DbSet<ProjectUser> ProjectUsers { get; set; }
        public virtual DbSet<ReqType> ReqTypes { get; set; }
        public virtual DbSet<Requirement> Requirements { get; set; }
        public virtual DbSet<Scenario> Scenarios { get; set; }
        public virtual DbSet<ScenarioActor> ScenarioActors { get; set; }
        public virtual DbSet<StateTask> StateTasks { get; set; }
        public virtual DbSet<StatusRequirement> StatusRequirements { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TaskHistory> TaskHistories { get; set; }
        public virtual DbSet<UseCase> UseCases { get; set; }
        public virtual DbSet<UseCaseActor> UseCaseActors { get; set; }
        public virtual DbSet<UseCaseRequirement> UseCaseRequirements { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserTechnology> UserTechnologies { get; set; }
        public virtual DbSet<Technology> Technologies { get; set; }
    }
}
