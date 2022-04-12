namespace Aimo.Domain;

public partial class Project : AuditableEntity, IActiveInactiveSupport, ISoftDeleteSupport
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}