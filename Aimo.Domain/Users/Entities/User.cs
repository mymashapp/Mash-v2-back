﻿using Aimo.Domain.Interests;

namespace Aimo.Domain.Users.Entities
{
    public partial class User : AuditableEntity, ISoftDeleteSupport, IActiveInactiveSupport
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? Bio { get; set; }

        public string? University { get; set; }
        public string? Location { get; set; }
        public int? Height { get; set; }
        
        public int PreferenceAgeTo { get; set; }
        public int PreferenceAgeFrom { get; set; }
        public Gender? PreferenceGender { get; set; }
        public GroupType? PreferenceGroupOf { get; set; }


        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsNew { get; set; }

        public List<Interest> Interests { get; init; } = new();
        public List<UserPicture> Pictures { get; init; } = new();
    }
}