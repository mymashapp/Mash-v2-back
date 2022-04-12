namespace Aimo.Domain.Users
{
    public partial class User : AuditableEntity, ISoftDeleteSupport
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int ProfilePictureId { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public bool IsDeleted { get; set; }
        public List<Interest> Interests { get; set; }
        public UserPreference UserPreference { get; set; }
        public List<Pictures> Pictures { get; set; }
    }
}
