﻿using Aimo.Domain.Chats;

namespace Aimo.Domain.Users
{
    public partial class UserDto : Dto
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string CurrentChat { get; set; }
        public string Device { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        
        public string? University { get; set; }
        public string? Location { get; set; }
        public float? Height { get; set; }
        
        public string Bio { get; set; }
        public bool IsNew { get; set; }
        public bool IsVaccinated { get; set; }
        public bool UserLocationEnabled { get; set; }

        public int PreferenceAgeTo { get; set; }
        public int PreferenceAgeFrom { get; set; }
        public GroupType? PreferenceGroupOf { get; set; }
        public Gender? PreferenceGender { get; set; }

        #region View
        public List<UserInterestDto> Interests { get; set; } = new();
        public ICollection<UserPictureDto> Pictures { get; set; } = Array.Empty<UserPictureDto>();

        #endregion

        #region Save
        public int[] SelectedInterestIds { get; set; } = Array.Empty<int>();
        public ICollection<UserPictureDto> UploadedPictures { get; set; } = Array.Empty<UserPictureDto>();
        public ICollection<ChatDto> Chats { get; set; } = Array.Empty<ChatDto>();
        #endregion
    }
}