namespace Aimo.Core;

public abstract class Filter : IdBase
{
    private int? _pageIndex;
    private int? _size;

    public virtual int? PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value is null || value == 0 ? 1 : value;
    }

    public virtual int? Size
    {
        get => _size;
        set => _size =  value is null || value == 0 ?  AimoDefaults.DefaultPageSize : value;
    }
    
    public virtual string? SortColumn { get; set; }
    public virtual SortDirection SortDirection { get; set; }

    public virtual string SearchValue { get; set; } = string.Empty;

    public int Draw { get; set; } //for datatable
}