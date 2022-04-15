namespace Aimo.Core;

public record Paging
{
    public int Index { get; set; }
    public int Size { get; set; }
    public int Total { get; set; }
    public string SortColumn { get; set; } = string.Empty;
    public string SortDirection { get; set; } = string.Empty;

    public int PageCount => (Total + Size - 1) / Size; //Math.Ceiling((decimal)Total / (decimal)Size);

    public int LastPage
    {
        get
        {
            if (Index == 0) return 0;

            var lastPage = Total / (decimal)Size;

            if (lastPage == 0)
                lastPage = 1;

            return (int)Math.Ceiling(lastPage);
        }
    }
}