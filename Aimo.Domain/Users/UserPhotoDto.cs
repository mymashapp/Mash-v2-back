namespace Aimo.Domain.Users
{
    public partial class UserPhotoDto : Dto
    {
        public int UserId { get; set; }
        public string Url { get; set; }
    }
}
