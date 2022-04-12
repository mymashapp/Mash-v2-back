namespace Aimo.Domain.Users
{
    public partial class Picture : Entity
    {
        public byte[] Binary { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public List<User> Users { get; set; }
    }
}
