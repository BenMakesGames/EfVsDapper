using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using MySqlEf;

namespace Performance;

[MemoryDiagnoser(false)]
public class Benchmarks
{
    private IDbContextFactory<MyContext> MyContextFactory { get; }

    private MemoryCache Cache { get; }

    private const string ConnectionString = "Server=localhost;Database=blog;Uid=root;Pwd=YOUR_PASSWORD_HERE";

    public Benchmarks()
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
        optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
        MyContextFactory = new PooledDbContextFactory<MyContext>(optionsBuilder.Options);

        Cache = new(new MemoryCacheOptions());
    }

    [Benchmark]
    public async Task<BlogPost?> GetBlogPostById_Ef()
    {
        long postId = 1;

        await using var context = await MyContextFactory.CreateDbContextAsync();

        return await context.BlogPosts.FirstOrDefaultAsync(post => post.Id == postId);
    }

    [Benchmark]
    public async Task<BlogPost?> GetBlogPostById_Dapper()
    {
        long postId = 1;

        await using var connection = new MySqlConnection(ConnectionString);

        return await connection.QueryFirstOrDefaultAsync<BlogPost>("""
            SELECT * FROM `blogposts` WHERE `id` = @Id LIMIT 1
        """, new { Id = postId });
    }

    [Benchmark]
    public async Task<BlogPost?> GetBlogPostById_Ef_WithCache()
    {
        long postId = 1;

        var blogPost = Cache.Get<BlogPost>(postId);

        if (blogPost != null)
        {
            return blogPost;
        }

        await using var context = await MyContextFactory.CreateDbContextAsync();

        blogPost = await context.BlogPosts.FirstOrDefaultAsync(post => post.Id == postId);

        if (blogPost != null)
        {
            Cache.Set(postId, blogPost);
        }

        return blogPost;
    }

    [Benchmark]
    public async Task<BlogPost?> GetBlogPostById_Dapper_WithCache()
    {
        long postId = 1;

        var blogPost = Cache.Get<BlogPost>(postId);

        if (blogPost != null)
        {
            return blogPost;
        }

        await using var connection = new MySqlConnection(ConnectionString);

        blogPost = await connection.QueryFirstOrDefaultAsync<BlogPost>("""
            SELECT * FROM `blogposts` WHERE `id` = @Id LIMIT 1
        """, new { Id = postId });

        if (blogPost != null)
        {
            Cache.Set(postId, blogPost);
        }

        return blogPost;
    }
}