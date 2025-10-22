namespace shared.Model;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public int Votes { get; set; }
    public string CUserName { get; set; } = "";
    public DateTime CCreatedAt { get; set; }

    public Comment(string content = "", int votes = 0)
    {
        Content = content;
        Votes = votes;
        CCreatedAt = DateTime.Now;
    }

    public Comment() {
        Id = 0;
        Content = "";
        Votes = 0;
        CCreatedAt = DateTime.Now;
    }
}