using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;

using Data;

using shared.Model;

var builder = WebApplication.CreateBuilder(args);

var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<PostContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));


builder.Services.AddScoped<DataService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData();
}

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

app.MapGet("/", () => new { message = "Hello World!" });

// ALT TIL POST HERUNDER

app.MapGet("/api/posts", (DataService service) =>
{
    return service.GetPosts();
});

app.MapGet("/api/posts/{id:int}", (DataService service, int id) =>
{
    var post = service.GetPost(id);
    return post is null ? Results.NotFound(new { message = "Post not found" }) : Results.Ok(post);
});

app.MapPost("/api/posts", (DataService service, NewPostData data) =>
{
    var message = service.CreatePost(data.title, data.content);
    return new { message };
});

app.MapPut("/api/posts/{id:int}/upvote", (DataService service, int id) =>
{
    var ok = service.UpvotePost(id);
    return ok ? Results.Ok(new { message = "Post upvoted" }) : Results.NotFound(new { message = "Post not found" });
});

app.MapPut("/api/posts/{id:int}/downvote", (DataService service, int id) =>
{
    var ok = service.DownvotePost(id);
    return ok ? Results.Ok(new { message = "Post downvoted" }) : Results.NotFound(new { message = "Post not found" });
});

// ALT TIL COMMENTS HERUNDER

app.MapPost("/api/posts/{id:int}/comments", (DataService service, int id, NewCommentData data) =>
{
    var message = service.AddCommentToPost(id, data.content, data.cUserName);
    return message == "Comment added" ? Results.Ok(new { message }) : Results.NotFound(new { message });
});
app.MapPut("/api/posts/{postid:int}/comments/{commentid:int}/upvote", (DataService service, int postid, int commentid) =>
{
    var ok = service.UpvoteComment(postid, commentid);
    return ok ? Results.Ok(new { message = "Comment upvoted" }) : Results.NotFound(new { message = "Post or comment not found" });
});

app.MapPut("/api/posts/{postid:int}/comments/{commentid:int}/downvote", (DataService service, int postid, int commentid) =>
{
    var ok = service.DownvoteComment(postid, commentid);
    return ok ? Results.Ok(new { message = "Comment downvoted" }) : Results.NotFound(new { message = "Post or comment not found" });
});

app.Run();

record NewPostData(string title, string content);
record NewCommentData(string content, string? cUserName);

