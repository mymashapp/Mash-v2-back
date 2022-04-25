namespace Aimo.Domain.Users.Entities
{
    public partial class UserPicture : Entity
    {
        public int UserId { get; set; }
        public string Url { get; set; }
        public PictureType PictureType { get; set; }
    }
}