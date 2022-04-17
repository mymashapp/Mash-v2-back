namespace Aimo.Domain.Category;

public partial class CategoryDto : Dto
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}