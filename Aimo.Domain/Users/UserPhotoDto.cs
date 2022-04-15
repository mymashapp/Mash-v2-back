namespace Aimo.Domain.Users
{
    public partial class UserPhotoViewDto : Dto
    {
        public int UserId { get; set; }
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
    }
    
    public partial class UserPhotoSaveDto : Dto
    {
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
    }
}
