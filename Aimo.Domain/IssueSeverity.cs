namespace Aimo.Domain;

public partial class IssueSeverity : Entity
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
}