namespace Aimo.Domain.Users
{


    public partial class UserPreferencesSaveDto : Dto
    {
        public int AgeTo { get; set; }
        public int AgeFrom { get; set; }
        public string Gender { get; set; }
        public int GroupOf { get; set; }
    }
}