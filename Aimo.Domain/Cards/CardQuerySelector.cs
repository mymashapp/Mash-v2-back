namespace Aimo.Domain.Cards;

public partial record struct CardQuerySelector
{
    public Card Card { get; set; }
    public int? SwipeCount { get; set; }
}