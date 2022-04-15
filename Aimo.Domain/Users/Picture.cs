namespace Aimo.Domain.Users
{
    public partial class Picture : Entity
    {
        public int UserId { get; set; }
        public string Url { get; set; }
    }
}
