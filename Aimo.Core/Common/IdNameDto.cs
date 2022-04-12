namespace Aimo.Core;

public partial class IdNameDto : Dto, IEqualityComparer<IdNameDto>
{
    public string Name{ get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    #region Equality

    protected bool Equals(IdNameDto other) => Equals(this, other);

    public override int GetHashCode() => GetHashCode(this);

    public bool Equals(IdNameDto? x, IdNameDto? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id && x.Name == y.Name;
    }

    public int GetHashCode(IdNameDto obj) => HashCode.Combine(obj.Id, obj.Name);

    #endregion
}