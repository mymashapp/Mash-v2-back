namespace Aimo.Domain.Categories;

public partial class SubCategoryDto : Dto
{
    public string Title { get; set; }
    public string Alias { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }

}