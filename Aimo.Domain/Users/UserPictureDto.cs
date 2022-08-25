namespace Aimo.Domain.Users
{
    public class UserPictureDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PictureUrl { get; set; }
        public PictureType PictureType { get; set; }
    }
}
