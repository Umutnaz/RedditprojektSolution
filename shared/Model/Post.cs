namespace shared.Model;

public class Post {
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public int Votes { get; set; }
    public string? PUserName { get; set; }
    public DateTime PCreatedAt { get; set; }
    public List<Comment> Comments { get; set; } = new();

    public Post(string title = "", string content = "", int votes = 0) {
        Title = title;
        Content = content;
        Votes = votes;
        PCreatedAt = DateTime.Now;
    }

    public Post() {
        Id = 0;
        Title = "";
        Content = "";
        Votes = 0;
        PCreatedAt = DateTime.Now;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, Content: {Content}, Votes: {Votes}";
    }
}
