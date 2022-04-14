namespace Aimo.Domain.Users
{
    public partial class UserDto: Dto
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int ProfilePictureId { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public bool IsNew { get; set; }
        public string ProfilePictureUrl { get; set; }
        
        public List<UserInterestDto> UserInterests { get; set; } = new();
        public UserPreferencesDto UserPreferences { get; set; } = new();
        public List<UserPhotoDto> UserPhotos { get; set; } = new();
    }
}
