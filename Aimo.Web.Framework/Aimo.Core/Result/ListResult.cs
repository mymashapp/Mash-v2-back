#nullable disable
#nullable enable annotations
namespace Aimo.Core;

[Serializable]
public partial record ListResult<T> : Result<T> where T : new()
{
    public new ICollection<T> Data { get; set; } = new List<T>();

    /*#region Datatable //TODO: remove 

    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
    public int Draw { get; set; }

    #endregion*/

    #region Paging

    private Paging _paging = new(); //TODO: remove

    public int PageSize
    {
        get => _paging.Size;
        set => _paging.Size = value;
    }

    public int PageIndex
    {
        get => _paging.Index;
        set => _paging.Index = value;
    }

    public int PageTotal
    {
        get => _paging.Total;
        set => _paging.Total = value;
    }

    public string SortBy
    {
        get => _paging.SortColumn;
        set => _paging.SortColumn = value;
    }

    public string SortDirection
    {
        get => _paging.SortDirection;
        set => _paging.SortDirection = value;
    }

    #endregion

    public new ListResult<T> Failure(string message)
    {
        base.Failure(message);
        return this;
    }

    public new ListResult<T> Exception(Exception ex)
    {
        base.Exception(ex);
        return this;
    }

    public new ListResult<T> Success(string message = null, params object[] args)
    {
        base.Success(message, args);
        return this;
    }

    public ListResult<T> SetPaging(int page, int size, int total)
    {
        PageSize = size == 0 ? AimoDefaults.DefaultPageSize : size;
        PageIndex = page;
        PageTotal = total;
        return this;
    }

    public ListResult<T> SetData(ICollection<T> data, dynamic? additionalData = default)
    {
        (this as Result).Data = data;
        Data = data;
        AdditionalData = additionalData;
        return this;
    }

    public ListResult<T> From(ListResult<T> other)
    {
        base.From(other);
        _paging = other._paging;
        return this;
    }
}