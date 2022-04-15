namespace Aimo.Domain.Issues;

public partial class Comment : AuditableEntity
{
    public string Message { get; set; }
}