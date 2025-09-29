using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Parent;

namespace aspcts_backend.Services
{
    public class ParentService : IParentService
    {
        private readonly IUserRepository _userRepository;
        private readonly IChildRepository _childRepository;

        public ParentService(IUserRepository userRepository, IChildRepository childRepository)
        {
            _userRepository = userRepository;
            _childRepository = childRepository;
        }

        public async Task<ParentSearchResult?> FindParentByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = await _userRepository.GetByEmailAsync(email.Trim().ToLower());

            if (user == null || user.Role != "Parent" || !user.IsActive)
                return null;

            var parent = await _userRepository.GetParentByUserIdAsync(user.UserId);

            if (parent == null)
                return null;

            return new ParentSearchResult
            {
                ParentId = parent.ParentId,
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ContactNumber = user.ContactNumber,
                ChildRelationship = parent.ChildRelationship
            };
        }

        public async Task<IEnumerable<ParentSearchResult>> SearchParentsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
                return new List<ParentSearchResult>();

            var searchTerm = query.Trim().ToLower();

            var users = await _userRepository.FindAsync(u =>
                u.Role == "Parent" &&
                u.IsActive &&
                (u.FirstName.ToLower().Contains(searchTerm) ||
                 u.LastName.ToLower().Contains(searchTerm) ||
                 u.Email.ToLower().Contains(searchTerm))
            );

            var results = new List<ParentSearchResult>();

            foreach (var user in users.Take(10))
            {
                var parent = await _userRepository.GetParentByUserIdAsync(user.UserId);
                if (parent != null)
                {
                    results.Add(new ParentSearchResult
                    {
                        ParentId = parent.ParentId,
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        ContactNumber = user.ContactNumber,
                        ChildRelationship = parent.ChildRelationship
                    });
                }
            }

            return results;
        }

        public async Task<ParentDetailInfo?> GetParentInfoAsync(Guid parentId)
        {
            var parent = await _userRepository.GetParentByUserIdAsync(parentId);
            if (parent == null)
                return null;

            var user = await _userRepository.GetByIdAsync(parent.UserId);
            if (user == null || !user.IsActive)
                return null;

            // Contar quantas crianças este responsável tem
            var children = await _childRepository.GetByParentIdAsync(parent.ParentId);

            return new ParentDetailInfo
            {
                ParentId = parent.ParentId,
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ContactNumber = user.ContactNumber,
                ChildRelationship = parent.ChildRelationship,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                IsActive = user.IsActive,
                ChildrenCount = children.Count()
            };
        }
    }
}