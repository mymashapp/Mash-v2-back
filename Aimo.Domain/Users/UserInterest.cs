namespace Aimo.Domain.Users
{
    public partial class Interest : Entity
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public List<User> Users { get; set; }
    }
    
    public partial class UserInterest : Entity
    {
        public int UserId { get; set; }
        public int InterestId { get; set; }
    }
}
