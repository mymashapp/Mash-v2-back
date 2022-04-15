namespace Aimo.Core;

public interface IActiveInactiveSupport
{
    public bool IsActive { get; set; }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
    public void Toggle() => IsActive = !IsActive;
}