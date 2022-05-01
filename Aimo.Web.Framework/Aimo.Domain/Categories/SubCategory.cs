using Aimo.Domain.Cards;

namespace Aimo.Domain.Categories;

public partial class SubCategory : Entity
{
    public string Title { get; set; }
    public string Alias { get; set; }
    public int DisplayOrder { get; set; }
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } 
    public List<Card> Cards { get;  init; } 

}