namespace Aimo.Domain.Users
{
    public partial class UserPreference : Entity
    {
        public int UserId { get; set; }
        public int AgeTo { get; set; }
        public int AgeFrom { get; set; }
        public string Gender { get; set; }
        public int GroupOf { get; set; }
    }
}
