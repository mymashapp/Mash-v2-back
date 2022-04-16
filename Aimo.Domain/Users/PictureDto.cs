namespace Aimo.Domain.Users
{
    public class PictureDto
    {
        public int UserId { get; set; }
        public string PictureUrl { get; set; }
        public PictureType PictureType { get; set; }
    }
}
