using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using BCryptAlias = BCrypt.Net.BCrypt;


namespace aspcts_backend.Data.Seeds
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedMilestonesAsync();
            await SeedSupportingSkillsAsync();
            await SeedDefaultUsersAsync();
        }

        private async Task SeedMilestonesAsync()
        {
            if (await _context.Milestones.AnyAsync())
                return;

            var milestones = new List<Milestone>
        {
            // Nível 1 (0-18 meses) - Domínio Mand
            new() { Name = "Mand 1", Description = "Faz 2 pedidos diferentes sem ajuda (exceto 'O que você quer?') - 2 vezes cada, em 1 dia.", DevelopmentalLevel = 1, Domain = "Mand", Order = 1 },
            new() { Name = "Mand 2", Description = "Faz 5 pedidos diferentes sem ajuda (exceto 'O que você quer?') - 2 vezes cada, em 1 dia.", DevelopmentalLevel = 1, Domain = "Mand", Order = 2 },
            new() { Name = "Mand 3", Description = "Generaliza 5 pedidos conhecidos para 2 pessoas diferentes e 2 ambientes diferentes.", DevelopmentalLevel = 1, Domain = "Mand", Order = 3 },
            new() { Name = "Mand 4", Description = "Faz 10 pedidos diferentes sem ajuda (exceto 'O que você quer?') com múltiplos exemplos.", DevelopmentalLevel = 1, Domain = "Mand", Order = 4 },
            new() { Name = "Mand 5", Description = "Faz 2 pedidos diferentes de itens ausentes sem ajuda.", DevelopmentalLevel = 1, Domain = "Mand", Order = 5 },
            
            // Nível 1 - Domínio Tato
            new() { Name = "Tact 1", Description = "Nomeia 2 itens sem ajuda - 2 vezes cada, em 1 dia.", DevelopmentalLevel = 1, Domain = "Tato", Order = 1 },
            new() { Name = "Tact 2", Description = "Nomeia 5 itens sem ajuda - 2 vezes cada, em 1 dia.", DevelopmentalLevel = 1, Domain = "Tato", Order = 2 },
            new() { Name = "Tact 3", Description = "Generaliza 5 tatos conhecidos para 2 pessoas diferentes e 2 ambientes diferentes.", DevelopmentalLevel = 1, Domain = "Tato", Order = 3 },
            new() { Name = "Tact 4", Description = "Nomeia 10 itens sem ajuda com múltiplos exemplos.", DevelopmentalLevel = 1, Domain = "Tato", Order = 4 },
            new() { Name = "Tact 5", Description = "Nomeia 2 ações sem ajuda.", DevelopmentalLevel = 1, Domain = "Tato", Order = 5 },
            
            // Nível 1 - Domínio Ouvinte
            new() { Name = "Listener 1", Description = "Atende à voz de um falante virando-se ou olhando para ele 5 vezes.", DevelopmentalLevel = 1, Domain = "Ouvinte", Order = 1 },
            new() { Name = "Listener 2", Description = "Responde ao seu nome olhando para o falante 5 vezes.", DevelopmentalLevel = 1, Domain = "Ouvinte", Order = 2 },
            new() { Name = "Listener 3", Description = "Responde corretamente a 'vem aqui' 5 vezes.", DevelopmentalLevel = 1, Domain = "Ouvinte", Order = 3 },
            new() { Name = "Listener 4", Description = "Responde corretamente a 'senta' 5 vezes.", DevelopmentalLevel = 1, Domain = "Ouvinte", Order = 4 },
            new() { Name = "Listener 5", Description = "Responde corretamente à instrução 'me dá _____' para 10 itens diferentes.", DevelopmentalLevel = 1, Domain = "Ouvinte", Order = 5 },
            
            // Adicionar mais marcos conforme necessário para Nível 2 e 3
        };

            _context.Milestones.AddRange(milestones);
            await _context.SaveChangesAsync();

            // Adicionar etapas de Análise de Tarefa para alguns marcos
            await SeedTaskAnalysisStepsAsync();
        }

        private async Task SeedTaskAnalysisStepsAsync()
        {
            if (await _context.TaskAnalysisSteps.AnyAsync())
                return;

            var mand1 = await _context.Milestones.FirstAsync(m => m.Name == "Mand 1");
            var taskSteps = new List<TaskAnalysisStep>
        {
            new() { MilestoneId = mand1.MilestoneId, Description = "A criança demonstra interesse no item desejado", Order = 1 },
            new() { MilestoneId = mand1.MilestoneId, Description = "A criança vocaliza ou tenta vocalizar", Order = 2 },
            new() { MilestoneId = mand1.MilestoneId, Description = "A criança usa aproximação da palavra", Order = 3 },
            new() { MilestoneId = mand1.MilestoneId, Description = "A criança usa vocalização clara", Order = 4 },
            new() { MilestoneId = mand1.MilestoneId, Description = "A criança usa a palavra espontaneamente", Order = 5 }
        };

            _context.TaskAnalysisSteps.AddRange(taskSteps);
            await _context.SaveChangesAsync();
        }

        private async Task SeedSupportingSkillsAsync()
        {
            if (await _context.SupportingSkills.AnyAsync())
                return;

            var supportingSkills = new List<SupportingSkill>
        {
            new() { Name = "Contato Visual", Description = "Mantém contato visual apropriado durante interações", Domain = "Social" },
            new() { Name = "Atenção Compartilhada", Description = "Compartilha atenção com outra pessoa em relação a um objeto ou evento", Domain = "Social" },
            new() { Name = "Turnos de Conversa", Description = "Respeita turnos em atividades e conversas", Domain = "Social" },
            new() { Name = "Seguir Instruções Simples", Description = "Segue instruções de um passo de forma consistente", Domain = "Ouvinte" },
            new() { Name = "Tolerância para Sentar", Description = "Senta-se adequadamente por períodos apropriados para a idade", Domain = "Sala de Aula" },
            new() { Name = "Esperar", Description = "Espera de forma adequada por itens ou atividades desejadas", Domain = "Autocontrole" },
            new() { Name = "Transição", Description = "Move-se entre atividades com mínima ajuda", Domain = "Sala de Aula" },
            new() { Name = "Brincadeira Independente", Description = "Brinca sozinho de forma apropriada por períodos adequados", Domain = "Brincadeira" },
            new() { Name = "Habilidades de Imitação", Description = "Imita ações e sons quando solicitado", Domain = "Aprendizagem" },
            new() { Name = "Habilidades de Associação", Description = "Associa objetos ou figuras idênticas", Domain = "Acadêmico" }
        };

            _context.SupportingSkills.AddRange(supportingSkills);
            await _context.SaveChangesAsync();
        }

        private async Task SeedDefaultUsersAsync()
        {
            if (await _context.Users.AnyAsync())
                return;

            // Criar psicólogo padrão
            var psychUser = new User
            {
                Username = "dr.silva",
                Email = "dr.silva@example.com",
                PasswordHash = BCryptAlias.HashPassword("123456"),
                Role = "Psicólogo",
                FirstName = "Dra. Maria",
                LastName = "Silva",
                ContactNumber = "(11) 99999-9999"
            };

            _context.Users.Add(psychUser);
            await _context.SaveChangesAsync();

            var psychologist = new Psychologist
            {
                UserId = psychUser.UserId,
                LicenseNumber = "CRP-123456",
                Specialization = "Terapia ABA",
                ClinicName = "Clínica Neurodesenvolvimento"
            };

            _context.Psychologists.Add(psychologist);

            // Criar pais padrão
            var parent1User = new User
            {
                Username = "joao.santos",
                Email = "joao.santos@example.com",
                PasswordHash = BCryptAlias.HashPassword("123456"),
                Role = "Responsável",
                FirstName = "João",
                LastName = "Santos",
                ContactNumber = "(11) 88888-8888"
            };

            var parent2User = new User
            {
                Username = "maria.santos",
                Email = "maria.santos@example.com",
                PasswordHash = BCryptAlias.HashPassword("123456"),
                Role = "Responsável",
                FirstName = "Maria",
                LastName = "Santos",
                ContactNumber = "(11) 77777-7777"
            };

            _context.Users.AddRange(parent1User, parent2User);
            await _context.SaveChangesAsync();

            var parent1 = new Parent
            {
                UserId = parent1User.UserId,
                ChildRelationship = "Pai"
            };

            var parent2 = new Parent
            {
                UserId = parent2User.UserId,
                ChildRelationship = "Mãe"
            };

            _context.Parents.AddRange(parent1, parent2);
            await _context.SaveChangesAsync();

            // Criar criança padrão
            var child = new Child
            {
                FirstName = "Pedro",
                LastName = "Santos",
                DateOfBirth = new DateTime(2020, 3, 15),
                Gender = "Masculino",
                Diagnosis = "Transtorno do Espectro Autista (TEA)",
                AssignedPsychologistId = psychologist.PsychologistId,
                PrimaryParentId = parent1.ParentId,
                SecondaryParentId = parent2.ParentId,
                MedicalHistory = "Diagnóstico aos 3 anos. Sem comorbidades médicas significativas."
            };

            _context.Children.Add(child);
            await _context.SaveChangesAsync();
        }
    }
}