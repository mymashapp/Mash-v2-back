namespace Aimo.Domain.Users
{
    public partial class UserViewDto : Dto
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int ProfilePictureId { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public bool IsNew { get; set; }
        public string ProfilePictureUrl { get; set; }

        public int PreferenceAgeTo { get; set; }
        public int PreferenceAgeFrom { get; set; }
        public string PreferenceGender { get; set; }
        public Group PreferenceGroupOf { get; set; }

        public List<UserInterestDto> UserInterests { get; set; } = new();
        public List<UserPhotoViewDto> UserPhotos { get; set; } = new();
    }

    public partial class UserSaveDto : Dto
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int ProfilePictureId { get; set; }
        public Gender? Gender { get; set; }
        public string Bio { get; set; }
        public string ProfilePictureUrl { get; set; }

        public int[] InterestIds { get; set; } = Array.Empty<int>();
        public int PreferenceAgeTo { get; set; }
        public int PreferenceAgeFrom { get; set; }
        public Gender? PreferenceGender { get; set; }
        public Group? PreferenceGroupOf { get; set; }

        public List<UserPhotoSaveDto> UserPhotos { get; set; } = new();
    }
}