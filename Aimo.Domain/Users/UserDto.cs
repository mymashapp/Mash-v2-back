namespace Aimo.Domain.Users
{
    public partial class UserDto : Dto
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int ProfilePictureId { get; set; }
        public Gender? Gender { get; set; }

        public string Bio { get; set; }
        public bool IsNew { get; set; }
        public string ProfilePictureUrl { get; set; }

        public int PreferenceAgeTo { get; set; }
        public int PreferenceAgeFrom { get; set; }
        public Group? PreferenceGroupOf { get; set; }
        public Gender? PreferenceGender { get; set; }

        #region View
        public List<UserInterestDto> Interests { get; set; } = new();
        public List<UserPhotoDto> Pictures { get; set; } = new();

        #endregion

        #region Save
        public int[] SelectedInterestIds { get; set; } = Array.Empty<int>();
        public string[] PicturesBase64 { get; set; } = Array.Empty<string>();
        #endregion
    }
}