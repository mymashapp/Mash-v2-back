namespace Aimo.Domain;

public partial class IssueCategory : Entity
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
}