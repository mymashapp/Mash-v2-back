namespace Aimo.Core;

public abstract class Filter : IdBase
{
    public virtual int? PageIndex { get; set; }
    public virtual int? Size { get; set; }

    public virtual string? SortColumn { get; set; }
    public virtual SortDirection SortDirection { get; set; }

    public virtual string SearchValue { get; set; } = string.Empty;

    public int Draw { get; set; } //for datatable
}