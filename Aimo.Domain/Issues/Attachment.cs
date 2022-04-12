#nullable disable
namespace Aimo.Domain.Issues;

public partial class Attachment : AuditableEntity
{
    public string FileName { get; set; }
    public string FileUrl { get; set; }
}