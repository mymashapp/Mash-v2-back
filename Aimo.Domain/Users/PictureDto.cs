namespace Aimo.Domain.Users
{
    public class PictureDto : Dto
    {
        public int UserId { get; set; }
        public string PictureBase64 { get; set; }
        
    }
}
