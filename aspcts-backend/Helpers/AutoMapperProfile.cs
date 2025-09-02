using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using aspcts_backend.Models.Entities;
using aspcts_backend.Models.DTOs.Child;
using aspcts_backend.Models.DTOs.Assessment;
using aspcts_backend.Models.DTOs.Session;
using aspcts_backend.Models.DTOs.InterventionPlan;
using aspcts_backend.Models.DTOs.Report;
using aspcts_backend.Models.DTOs.Communication;

namespace aspcts_backend.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Child mappings
            CreateMap<Child, ChildResponse>()
                .ForMember(dest => dest.PsychologistName,
                    opt => opt.MapFrom(src => $"{src.AssignedPsychologist.User.FirstName} {src.AssignedPsychologist.User.LastName}"))
                .ForMember(dest => dest.TotalSessions,
                    opt => opt.MapFrom(src => src.Sessions.Count))
                .ForMember(dest => dest.TotalAssessments,
                    opt => opt.MapFrom(src => src.Assessments.Count))
                .ForMember(dest => dest.LastSessionDate,
                    opt => opt.MapFrom(src => src.Sessions.OrderByDescending(s => s.SessionDate).FirstOrDefault() != null
                        ? src.Sessions.OrderByDescending(s => s.SessionDate).First().SessionDate : (DateTime?)null))
                .ForMember(dest => dest.LastAssessmentDate,
                    opt => opt.MapFrom(src => src.Assessments.OrderByDescending(a => a.AssessmentDate).FirstOrDefault() != null
                        ? src.Assessments.OrderByDescending(a => a.AssessmentDate).First().AssessmentDate : (DateTime?)null));

            CreateMap<Parent, ParentInfo>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.User.ContactNumber));

            // Assessment mappings
            CreateMap<Assessment, AssessmentResponse>()
                .ForMember(dest => dest.ChildName,
                    opt => opt.MapFrom(src => $"{src.Child.FirstName} {src.Child.LastName}"))
                .ForMember(dest => dest.PsychologistName,
                    opt => opt.MapFrom(src => $"{src.Psychologist.User.FirstName} {src.Psychologist.User.LastName}"));

            // Session mappings
            CreateMap<Session, SessionResponse>()
                .ForMember(dest => dest.ChildName,
                    opt => opt.MapFrom(src => $"{src.Child.FirstName} {src.Child.LastName}"))
                .ForMember(dest => dest.PsychologistName,
                    opt => opt.MapFrom(src => $"{src.Psychologist.User.FirstName} {src.Psychologist.User.LastName}"));

            CreateMap<SessionCreateRequest, Session>();
            CreateMap<SessionUpdateRequest, Session>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // InterventionPlan mappings
            CreateMap<InterventionPlan, InterventionPlanResponse>()
                .ForMember(dest => dest.ChildName,
                    opt => opt.MapFrom(src => $"{src.Child.FirstName} {src.Child.LastName}"))
                .ForMember(dest => dest.PsychologistName,
                    opt => opt.MapFrom(src => $"{src.Psychologist.User.FirstName} {src.Psychologist.User.LastName}"));

            CreateMap<InterventionGoal, InterventionGoalResponse>();
            CreateMap<InterventionGoalRequest, InterventionGoal>();

            // Report mappings
            CreateMap<Report, ReportResponse>()
                .ForMember(dest => dest.ChildName,
                    opt => opt.MapFrom(src => $"{src.Child.FirstName} {src.Child.LastName}"))
                .ForMember(dest => dest.PsychologistName,
                    opt => opt.MapFrom(src => $"{src.Psychologist.User.FirstName} {src.Psychologist.User.LastName}"));

            // Communication mappings
            CreateMap<CommunicationMessage, MessageResponse>()
                .ForMember(dest => dest.SenderName,
                    opt => opt.MapFrom(src => $"{src.Sender.FirstName} {src.Sender.LastName}"))
                .ForMember(dest => dest.SenderRole,
                    opt => opt.MapFrom(src => src.Sender.Role))
                .ForMember(dest => dest.ReceiverName,
                    opt => opt.MapFrom(src => $"{src.Receiver.FirstName} {src.Receiver.LastName}"))
                .ForMember(dest => dest.ReceiverRole,
                    opt => opt.MapFrom(src => src.Receiver.Role))
                .ForMember(dest => dest.ChildName,
                    opt => opt.MapFrom(src => $"{src.Child.FirstName} {src.Child.LastName}"));
        }
    }
}