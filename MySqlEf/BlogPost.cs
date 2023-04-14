using System.ComponentModel.DataAnnotations;

namespace MySqlEf;

public sealed class BlogPost
{
    public long Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    [MaxLength(200)] public required string Title { get; set; }
    public required string Body { get; set; }
}