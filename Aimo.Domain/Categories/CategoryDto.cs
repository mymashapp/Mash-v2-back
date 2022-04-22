namespace Aimo.Domain.Categories;
/// <summary>
/// this is categorydto
/// </summary>
public partial class CategoryDto : Dto
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    public ICollection<SubCategory> SubCategories { get; set; } = Array.Empty<SubCategory>();

   // public List<SubCategoryWithCategoryDto> SubCategories { get; init; } = new();

}