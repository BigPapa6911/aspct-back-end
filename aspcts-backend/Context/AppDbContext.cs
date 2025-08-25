using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using aspcts_backend.Models;

namespace aspcts_backend.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Psychologist> Psychologists { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<MilestonesAssessment> MilestonesAssessments { get; set; }
        public DbSet<BarriersAssessment> BarriersAssessments { get; set; }
        public DbSet<TransitionAssessment> TransitionAssessments { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<TaskAnalysisStep> TaskAnalysisSteps { get; set; }
        public DbSet<SupportingSkill> SupportingSkills { get; set; }
        public DbSet<ChildSupportingSkillProgress> ChildSupportingSkillProgress { get; set; }
        public DbSet<InterventionPlan> InterventionPlans { get; set; }
        public DbSet<InterventionGoal> InterventionGoals { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<CommunicationMessage> CommunicationMessages { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ChildResource> ChildResources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Herança TPH
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("user_type")
                .HasValue<Psychologist>("Psychologist")
                .HasValue<Parent>("Parent");

            modelBuilder.Entity<Assessment>()
                .HasDiscriminator<string>("assessment_type")
                .HasValue<MilestonesAssessment>("Milestones")
                .HasValue<BarriersAssessment>("Barriers")
                .HasValue<TransitionAssessment>("Transition");

            // Chaves compostas para tabelas de junção
            modelBuilder.Entity<ChildSupportingSkillProgress>()
                .HasKey(cssp => new { cssp.ChildID, cssp.SkillID });

            modelBuilder.Entity<ChildResource>()
                .HasKey(cr => new { cr.ChildID, cr.ResourceID });

            // Mapeamento dos relacionamentos de muitos-para-muitos
            modelBuilder.Entity<ChildSupportingSkillProgress>()
                .HasOne(cssp => cssp.Child)
                .WithMany(c => c.ChildSupportingSkillProgress)
                .HasForeignKey(cssp => cssp.ChildID);

            modelBuilder.Entity<ChildSupportingSkillProgress>()
                .HasOne(cssp => cssp.SupportingSkill)
                .WithMany(ss => ss.ChildSupportingSkillProgress)
                .HasForeignKey(cssp => cssp.SkillID);

            modelBuilder.Entity<ChildResource>()
                .HasOne(cr => cr.Child)
                .WithMany(c => c.ChildResources)
                .HasForeignKey(cr => cr.ChildID);

            modelBuilder.Entity<ChildResource>()
                .HasOne(cr => cr.Resource)
                .WithMany(r => r.ChildResources)
                .HasForeignKey(cr => cr.ResourceID);

            base.OnModelCreating(modelBuilder);
        }

    }
}