namespace Aimo.Core;

public partial class IdNameLocalizedDto : Dto, IEqualityComparer<IdNameLocalizedDto>
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameLocalized { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public Language Language { get; set; }

    #region Equality

    protected bool Equals(IdNameLocalizedDto other) => Equals(this, other);

    public override int GetHashCode() => GetHashCode(this);

    public bool Equals(IdNameLocalizedDto? x, IdNameLocalizedDto? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id && x.NameEn == y.NameEn;
    }

    public int GetHashCode(IdNameLocalizedDto obj) => HashCode.Combine(obj.Id, obj.NameEn);

    #endregion
}