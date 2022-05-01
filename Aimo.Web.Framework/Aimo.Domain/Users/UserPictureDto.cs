namespace Aimo.Domain.Users
{
    public class UserPictureDto
    {
        public int UserId { get; set; }
        public string PictureUrl { get; set; }
        public PictureType PictureType { get; set; }
    }
}
