namespace Aimo.Domain.Categories;
#nullable disable
public partial class Category : AuditableEntity, IActiveInactiveSupport
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<SubCategory> SubCategories { get; set; } = new();
}