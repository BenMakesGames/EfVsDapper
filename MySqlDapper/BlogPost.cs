namespace MySqlDapper;

public sealed class BlogPost
{
    public long Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public required string Title { get; set; }
    public required string Body { get; set; }
}