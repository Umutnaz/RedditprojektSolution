using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using shared.Model;

namespace kreddit_app.Data;

public class ApiService
{
    private readonly HttpClient http;
    private readonly IConfiguration configuration;
    private readonly string baseAPI = "";

    public ApiService(HttpClient http, IConfiguration configuration)
    {
        this.http = http;
        this.configuration = configuration;
        this.baseAPI = configuration["base_api"];
    }

    public async Task<Post[]> GetPosts()
    {
        string url = $"{baseAPI}posts/";
        return await http.GetFromJsonAsync<Post[]>(url);
    }

    public async Task<Post> GetPost(int id)
    {
        string url = $"{baseAPI}posts/{id}/";
        return await http.GetFromJsonAsync<Post>(url);
    }

    public async Task<string> CreateComment(string content, int postId, string? cUserName = null)
    {
        string url = $"{baseAPI}posts/{postId}/comments";
        var payload = new { content = content ?? "", cUserName };
        using var msg = await http.PostAsJsonAsync(url, payload);
        msg.EnsureSuccessStatusCode();
        return "Comment added";
    }

    public async Task<string> CreatePost(string title, string content, string? pUserName = null)
    {
        string url = $"{baseAPI}posts/";
        var payload = new { title = title ?? "", content = content ?? "", pUserName };
        using var msg = await http.PostAsJsonAsync(url, payload);
        msg.EnsureSuccessStatusCode();
        return "Post created";
    }

    // ----- Votes (posts) -----
    public async Task<Post?> UpvotePost(int id)
    {
        string url = $"{baseAPI}posts/{id}/upvote/";
        var msg = await http.PutAsJsonAsync(url, "");
        msg.EnsureSuccessStatusCode();
        var json = await msg.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Post>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<Post?> DownvotePost(int id)
    {
        string url = $"{baseAPI}posts/{id}/downvote/";
        var msg = await http.PutAsJsonAsync(url, "");
        msg.EnsureSuccessStatusCode();
        var json = await msg.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Post>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    // ----- Votes (comments) -----
    public async Task<string> UpvoteComment(int postId, int commentId)
    {
        string url = $"{baseAPI}posts/{postId}/comments/{commentId}/upvote/";
        var msg = await http.PutAsJsonAsync(url, "");
        msg.EnsureSuccessStatusCode();
        return "OK";
    }

    public async Task<string> DownvoteComment(int postId, int commentId)
    {
        string url = $"{baseAPI}posts/{postId}/comments/{commentId}/downvote/";
        var msg = await http.PutAsJsonAsync(url, "");
        msg.EnsureSuccessStatusCode();
        return "OK";
    }
}
