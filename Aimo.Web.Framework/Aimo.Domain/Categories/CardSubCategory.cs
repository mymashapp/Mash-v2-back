using Aimo.Domain.Cards;

namespace Aimo.Domain.Categories;

public partial class CardSubCategory :Entity
{
    public int CardId { get; set; }
    public int SubCategoryId { get; set; }

    public virtual Card Card { get; set; }
    public virtual SubCategory SubCategory { get; set; }
}