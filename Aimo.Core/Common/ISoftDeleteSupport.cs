namespace Aimo.Core;

public interface ISoftDeleteSupport
{
    public bool IsDeleted { get; set; }

    public void Delete() => IsDeleted = true;

    public void UnDelete() => IsDeleted = false;
    public void Toggle() => IsDeleted=!IsDeleted ;
}