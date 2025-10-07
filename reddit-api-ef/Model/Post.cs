namespace Model;

public class Post
{
    public int postid { get; set; }
    public string? author { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Comment>? comments { get; set; }
    public int upvotes { get; set; } = 0;
    public int downvotes { get; set; } = 0;

}