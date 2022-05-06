#nullable disable
#nullable enable annotations
namespace Aimo.Core;

[Serializable]
public partial record Result<T> : Result where T : new()
{
    public new T Data { get; set; } = new T();

    public new Result<T> Failure(string message)
    {
        base.Failure(message);
        return this;
    }

    public new Result<T> Exception(Exception ex)
    {
        base.Exception(ex);
        return this;
    }

    public new Result<T> Success(string message = null, params object[] args)
    {
        base.Success(message, args);
        return this;
    }

    /*public new Result<T> SetPaging(int page, int size, int total)
    {
        base.SetPaging(page, size, total);
        return this;
    }*/

    public Result<T> SetData(T data, dynamic additionalData = null)
    {
        base.Data = data; 
        AdditionalData = additionalData;
        
        Data = data;
        return this;
    }
}