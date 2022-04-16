namespace Aimo.Domain.Users.Entities
{
    public partial class Picture : Entity
    {
        public int UserId { get; set; }
        public string Url { get; set; }
        public PictureType PictureType { get; set; }
    }
}