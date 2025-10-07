namespace Model;

public class Comment
{
    public int commentid { get; set; }
    public string author { get; set; }
    public string content { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int upvotes { get; set; } = 0;
    public int downvotes { get; set; } = 0;
    
    public int? postid { get; set; }
        public Post? post { get; set; }
}