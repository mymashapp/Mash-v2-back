namespace Aimo.Domain;

public partial class IssueStatus : Entity
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
}