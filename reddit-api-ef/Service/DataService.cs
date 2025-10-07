using Microsoft.EntityFrameworkCore;
using Data;
using Model;

namespace Service;

public class DataService
{
    private PostContext db { get; }

    public DataService(PostContext db) {
        this.db = db;
    }
    public void SeedData() {
        if (!db.Posts.Any())
        {
            var now = DateTime.Now;

            var post1 = new Post
            {
                postid = 1,
                author = "Kristian",
                title = "Min første post",
                content = "Hej alle sammen, dette er min første post!",
                upvotes = 5,
                downvotes = 0,
                comments = new List<Comment>
                {
                    new Comment
                    {
                        commentid = 1,
                        author = "Søren",
                        content = "Velkommen til forumet!",
                        upvotes = 2,
                        downvotes = 0
                    },
                    new Comment
                    {
                        commentid = 2,
                        author = "Mette",
                        content = "God post – glæder mig til at læse mere.",
                        upvotes = 3,
                        downvotes = 1
                    }
                }
            };

            var post2 = new Post
            {
                postid = 2,
                author = "Søren",
                title = "En anden post",
                content = "Dette er endnu en post, bare for at teste seed data.",
                upvotes = 1,
                downvotes = 0,
                comments = new List<Comment>
                {
                    new Comment
                    {
                        commentid = 3,
                        author = "Kristian",
                        content = "Interessant, fortæl mere!",
                    }
                }
            };

            db.Posts.Add(post1);
            db.Posts.Add(post2);
            db.SaveChanges();
        }
    }


//Get
    public List<Post> GetPosts()
    {
        return db.Posts.Include(p => p.comments).ToList();
    }

    public Post? GetPost(int id)
    {
        return db.Posts.Include(p => p.comments).FirstOrDefault(p => p.postid == id);
    }

    //Post

    public string CreatePost(string author, string title, string content)
    {
        int nextId = db.Posts.Select(p => p.postid).DefaultIfEmpty(0).Max() + 1;

        var post = new Post
        {
            postid = nextId,
            author = string.IsNullOrWhiteSpace(author) ? "Anonymous" : author,
            title = title ?? "",
            content = content ?? "",
            comments = new List<Comment>()
        };

        db.Posts.Add(post);
        db.SaveChanges();
        return "Post created";
    }

    public string AddCommentToPost(int postId, string author, string content)
    {
        var post = db.Posts.Include(p => p.comments).FirstOrDefault(p => p.postid == postId);
        if (post == null) return "Post not found";

        post.comments ??= new List<Comment>();
        int nextCid = post.comments.Select(c => c.commentid).DefaultIfEmpty(0).Max() + 1;

        var newComment = new Comment
        {
            commentid = nextCid,
            author = string.IsNullOrWhiteSpace(author) ? "Anonymous" : author,
            content = content ?? "",
        };

        post.comments.Add(newComment);
        db.SaveChanges();
        return "Comment added";
    }

    // Votes

    public bool UpvotePost(int id)
    {
        var post = db.Posts.FirstOrDefault(p => p.postid == id);
        if (post == null) return false;

        post.upvotes += 1;
        db.SaveChanges();
        return true;
    }

    public bool DownvotePost(int id)
    {
        var post = db.Posts.FirstOrDefault(p => p.postid == id);
        if (post == null) return false;

        post.downvotes += 1;
        db.SaveChanges();
        return true;
    }

    // Votes

    public bool UpvoteComment(int postId, int commentId)
    {
        var post = db.Posts.Include(p => p.comments).FirstOrDefault(p => p.postid == postId);
        if (post == null) return false;

        var comment = post.comments?.FirstOrDefault(c => c.commentid == commentId);
        if (comment == null) return false;

        comment.upvotes += 1;
        db.SaveChanges();
        return true;
    }

    public bool DownvoteComment(int postId, int commentId)
    {
        var post = db.Posts.Include(p => p.comments).FirstOrDefault(p => p.postid == postId);
        if (post == null) return false;

        var comment = post.comments?.FirstOrDefault(c => c.commentid == commentId);
        if (comment == null) return false;

        comment.downvotes += 1;
        db.SaveChanges();
        return true;
    }
}
