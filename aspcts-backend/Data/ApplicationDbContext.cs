using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Psychologist> Psychologists { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<MilestonesAssessment> MilestonesAssessments { get; set; }
        public DbSet<BarriersAssessment> BarriersAssessments { get; set; }
        public DbSet<TransitionAssessment> TransitionAssessments { get; set; }
        public DbSet<InterventionPlan> InterventionPlans { get; set; }
        public DbSet<InterventionGoal> InterventionGoals { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<CommunicationMessage> CommunicationMessages { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<TaskAnalysisStep> TaskAnalysisSteps { get; set; }
        public DbSet<SupportingSkill> SupportingSkills { get; set; }
        public DbSet<ChildSupportingSkillProgress> ChildSupportingSkillProgresses { get; set; }
        public DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Psychologist Configuration
            modelBuilder.Entity<Psychologist>(entity =>
            {
                entity.HasKey(e => e.PsychologistId);
                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Parent Configuration
            modelBuilder.Entity<Parent>(entity =>
            {
                entity.HasKey(e => e.ParentId);
                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Child Configuration
            modelBuilder.Entity<Child>(entity =>
            {
                entity.HasKey(e => e.ChildId);

                // Relationship with Psychologist
                entity.HasOne(c => c.AssignedPsychologist)
                      .WithMany(p => p.Children)
                      .HasForeignKey(c => c.AssignedPsychologistId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Primary Parent
                entity.HasOne(c => c.PrimaryParent)
                      .WithMany(p => p.PrimaryChildren)
                      .HasForeignKey(c => c.PrimaryParentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Secondary Parent (optional)
                entity.HasOne(c => c.SecondaryParent)
                      .WithMany(p => p.SecondaryChildren)
                      .HasForeignKey(c => c.SecondaryParentId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Session Configuration
            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.SessionId);

                entity.HasOne(s => s.Child)
                      .WithMany(c => c.Sessions)
                      .HasForeignKey(s => s.ChildId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Psychologist)
                      .WithMany(p => p.Sessions)
                      .HasForeignKey(s => s.PsychologistId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Assessment Configuration
            modelBuilder.Entity<Assessment>(entity =>
            {
                entity.HasKey(e => e.AssessmentId);
                entity.HasDiscriminator<string>("AssessmentType")
                      .HasValue<MilestonesAssessment>("Milestones")
                      .HasValue<BarriersAssessment>("Barriers")
                      .HasValue<TransitionAssessment>("Transition");

                entity.HasOne(a => a.Child)
                      .WithMany(c => c.Assessments)
                      .HasForeignKey(a => a.ChildId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Psychologist)
                      .WithMany(p => p.Assessments)
                      .HasForeignKey(a => a.PsychologistId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // InterventionPlan Configuration
            modelBuilder.Entity<InterventionPlan>(entity =>
            {
                entity.HasKey(e => e.PlanId);

                entity.HasOne(ip => ip.Child)
                      .WithMany(c => c.InterventionPlans)
                      .HasForeignKey(ip => ip.ChildId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ip => ip.Psychologist)
                      .WithMany(p => p.InterventionPlans)
                      .HasForeignKey(ip => ip.PsychologistId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // InterventionGoal Configuration
            modelBuilder.Entity<InterventionGoal>(entity =>
            {
                entity.HasKey(e => e.GoalId);

                entity.HasOne(ig => ig.InterventionPlan)
                      .WithMany(ip => ip.InterventionGoals)
                      .HasForeignKey(ig => ig.PlanId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Report Configuration
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId);

                entity.HasOne(r => r.Child)
                      .WithMany(c => c.Reports)
                      .HasForeignKey(r => r.ChildId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Psychologist)
                      .WithMany(p => p.Reports)
                      .HasForeignKey(r => r.PsychologistId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // CommunicationMessage Configuration
            modelBuilder.Entity<CommunicationMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.HasOne(cm => cm.Sender)
                      .WithMany()
                      .HasForeignKey(cm => cm.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cm => cm.Receiver)
                      .WithMany()
                      .HasForeignKey(cm => cm.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cm => cm.Child)
                      .WithMany(c => c.CommunicationMessages)
                      .HasForeignKey(cm => cm.ChildId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Milestone Configuration
            modelBuilder.Entity<Milestone>(entity =>
            {
                entity.HasKey(e => e.MilestoneId);
                entity.HasIndex(e => new { e.DevelopmentalLevel, e.Domain, e.Order });
            });

            // TaskAnalysisStep Configuration
            modelBuilder.Entity<TaskAnalysisStep>(entity =>
            {
                entity.HasKey(e => e.StepId);

                entity.HasOne(tas => tas.Milestone)
                      .WithMany(m => m.TaskAnalysisSteps)
                      .HasForeignKey(tas => tas.MilestoneId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SupportingSkill Configuration
            modelBuilder.Entity<SupportingSkill>(entity =>
            {
                entity.HasKey(e => e.SkillId);
            });

            // ChildSupportingSkillProgress Configuration
            modelBuilder.Entity<ChildSupportingSkillProgress>(entity =>
            {
                entity.HasKey(e => e.ProgressId);

                entity.HasOne(cssp => cssp.Child)
                      .WithMany(c => c.SupportingSkillProgresses)
                      .HasForeignKey(cssp => cssp.ChildId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cssp => cssp.SupportingSkill)
                      .WithMany(ss => ss.ChildProgresses)
                      .HasForeignKey(cssp => cssp.SkillId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ChildId, e.SkillId }).IsUnique();
            });

            // Resource Configuration
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.ResourceId);
            });
        }
    }
}