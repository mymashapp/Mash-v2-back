namespace Aimo.Domain.Users
{
    public partial class BlockedUser : Entity
    {
        public int BlockingUserId { get; set; }
        public int BlockedUserId { get; set; }
    }
}
