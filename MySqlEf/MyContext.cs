using Microsoft.EntityFrameworkCore;

namespace MySqlEf;

public sealed class MyContext: DbContext
{
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

    public MyContext(DbContextOptions<MyContext> options): base(options)
    {
    }
}