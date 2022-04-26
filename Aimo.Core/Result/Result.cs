using System.Collections.Concurrent;

namespace Aimo.Core;

[Serializable]
public partial record Result
{
    #region Properties

    //public long Id { get; set; }
    public ConcurrentDictionary<string, string> Errors { get; set; } = new();

    public bool IsSucceeded { get; set; }

    public dynamic Data { get; set; } = null!;
    public dynamic AdditionalData { get; set; } = null!;
    public string Message { get; set; } = string.Empty;

    public ResultType ResultType { get; set; } = ResultType.Success;

    #endregion

   
    public static Result Create(dynamic? data = default)
    {
        var result = new Result();
        return data is null ? result : result.SetData(data);
    }
    
    #region Methods

    public virtual Result Failure(string message)
    {
        IsSucceeded = false;
        Message = message;
        ResultType = ResultType.Error;
        return this;
    }

    public virtual Result Success(string? message = null, params object[]? args)
    {
        IsSucceeded = true;
        ResultType = ResultType.Success;

        if (!message!.IsNullOrWhiteSpace())
            Message = args is not null ? message.Format(args) : message;

        return this;
    }

    public virtual Result SetData(dynamic data, dynamic? additionalData = null)
    {
        Data = data;
        AdditionalData = additionalData!;
        return this;
    }

    public virtual Result From(Result other)
    {
        //Id = other.Id;
        Data = other.Data;
        AdditionalData = other.AdditionalData;
        IsSucceeded = other.IsSucceeded;
        Message = other.Message;
        ResultType = other.ResultType;
        Errors = other.Errors;
        return this;
    }

    public static async Task<Result<T>> CreateAsync<T>(Task<T>? dataTask = default) where T : new() =>
        Create(await dataTask!);


    public static Result<T> Create<T>(T? data = default) where T : new()
    {
        var result = new Result<T>();
        return data is null ? result : result.SetData(data);
    }

    public static ListResult<T> Create<T>(ICollection<T> data) where T : new()
    {
       return new ListResult<T>().SetData(data);
    }

    /*public static async Task<Result<List<T>>> CreateAsync<T>(Task<IEnumerable<T>> dataTask) => Create(await dataTask);*/

    #endregion
}