namespace Aimo.Domain.Users.Entities
{
    public partial class UserInterest : Entity
    {
        public int UserId { get; set; }
        public int InterestId { get; set; }
    }
}
