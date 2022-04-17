namespace Aimo.Domain.Category;

public partial class Category:AuditableEntity, IActiveInactiveSupport
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}