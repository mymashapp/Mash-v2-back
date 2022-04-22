namespace Aimo.Domain.Categories;

public partial class SubCategoryWithCategoryDto : Dto
{
    public string Title { get; set; }
    public string Alias { get; set; }
    public string CategoryName { get; set; }
    public int CategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}