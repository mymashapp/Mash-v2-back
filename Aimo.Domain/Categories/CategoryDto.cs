namespace Aimo.Domain.Categories;
/// <summary>
/// this is categorydto
/// </summary>
public partial class CategoryDto : IdNameDto
{
    public bool IsActive { get; set; }

    public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}