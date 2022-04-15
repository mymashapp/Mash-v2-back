namespace Aimo.Domain.Users
{
    public partial class UserInterestDto: Dto
    {
        public int UserId { get; set; }
        public int InterestId { get; set; }
        public string InterestName { get; set; }
    }
}
