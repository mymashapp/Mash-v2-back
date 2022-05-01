namespace Aimo.Core;
/// <summary>
/// Represents any class that supports Id, should inherit from IdBase class this will help in reflection and generic constrains
/// </summary>
public abstract partial class IdBase // TODO: to interface
{
    public virtual int Id { get; set; }
}

/// <summary>
/// use for entity
/// </summary>
public abstract partial class Entity : IdBase
{ }

/// <summary>
/// use for dto
/// </summary>
public partial class Dto : IdBase // TODO: dto should be record struct type??
{ }

public partial class AuditableEntity : Entity
{
    public int CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

/*/// <summary>
/// use for model and view model
/// </summary>
public abstract class BaseModel : IdBase
{ }*/

