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
            
            // ═══════════════════════════════════════════════════════════
            // NOVOS DBSETS
            // ═══════════════════════════════════════════════════════════
            public DbSet<SessionProtocolData> SessionProtocolData { get; set; }

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

                        // ═══════════════════════════════════════════════════════════
                        // NOVO: Relacionamento One-to-One com SessionProtocolData
                        // ═══════════════════════════════════════════════════════════
                        entity.HasOne(s => s.ProtocolData)
                    .WithOne(spd => spd.Session)
                    .HasForeignKey<SessionProtocolData>(spd => spd.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                        // Índices para performance
                        entity.HasIndex(s => s.ChildId)
                    .HasDatabaseName("IX_Sessions_ChildId");

                        entity.HasIndex(s => s.PsychologistId)
                    .HasDatabaseName("IX_Sessions_PsychologistId");

                        entity.HasIndex(s => s.SessionDate)
                    .HasDatabaseName("IX_Sessions_SessionDate");
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

                  // ═══════════════════════════════════════════════════════════
                  // Report Configuration (ATUALIZADO)
                  // ═══════════════════════════════════════════════════════════
                  modelBuilder.Entity<Report>(entity =>
                  {
                        entity.HasKey(e => e.ReportId);

                        entity.HasOne(r => r.Child)
                    .WithMany(c => c.Reports)
                    .HasForeignKey(r => r.ChildId)
                    .OnDelete(DeleteBehavior.Restrict); // Mudado de Cascade para Restrict

                        entity.HasOne(r => r.Psychologist)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(r => r.PsychologistId)
                    .OnDelete(DeleteBehavior.Restrict);

                        // Relacionamento com SessionProtocolData
                        entity.HasMany(r => r.SessionsProtocolData)
                    .WithOne(spd => spd.Report)
                    .HasForeignKey(spd => spd.ReportId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                        // Índices para performance
                        entity.HasIndex(r => r.ChildId)
                    .HasDatabaseName("IX_Reports_ChildId");

                        entity.HasIndex(r => r.PsychologistId)
                    .HasDatabaseName("IX_Reports_PsychologistId");

                        entity.HasIndex(r => new { r.StartPeriod, r.EndPeriod })
                    .HasDatabaseName("IX_Reports_Period");

                        entity.HasIndex(r => r.ReportDate)
                    .HasDatabaseName("IX_Reports_ReportDate");

                        // Valor padrão
                        entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
                  });

                  // ═══════════════════════════════════════════════════════════
                  // SessionProtocolData Configuration (NOVO)
                  // ═══════════════════════════════════════════════════════════
                  modelBuilder.Entity<SessionProtocolData>(entity =>
                  {
                        entity.HasKey(e => e.SessionProtocolDataId);

                        // Relacionamento One-to-One com Session
                        entity.HasOne(spd => spd.Session)
                    .WithOne(s => s.ProtocolData)
                    .HasForeignKey<SessionProtocolData>(spd => spd.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                        // Relacionamento Many-to-One com Report (Opcional)
                        entity.HasOne(spd => spd.Report)
                    .WithMany(r => r.SessionsProtocolData)
                    .HasForeignKey(spd => spd.ReportId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                        // Índices para performance
                        entity.HasIndex(spd => spd.SessionId)
                    .IsUnique()
                    .HasDatabaseName("IX_SessionProtocolData_SessionId");

                        entity.HasIndex(spd => spd.ReportId)
                    .HasDatabaseName("IX_SessionProtocolData_ReportId");

                        // Valor padrão
                        entity.Property(spd => spd.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
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