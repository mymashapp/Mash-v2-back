namespace Aimo.Domain;

public partial class IssuePriority : Entity
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
}