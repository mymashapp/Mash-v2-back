namespace Aimo.Domain.Users
{
    public partial class UserPhotoDto : Dto
    {
        public int UserId { get; set; }
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
    }
}
