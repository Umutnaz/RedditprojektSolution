using Microsoft.EntityFrameworkCore;
using Data;
using shared.Model;

namespace shared.Model
{
    public class DataService
    {
        private PostContext db { get; }

        public DataService(PostContext db)
        {
            this.db = db;
        }

public void SeedData()
{
    if (!db.Posts.Any())
    {
        var post1 = new Post
        {
            Id = 1,
            Title = "Min første post",
            Content = "Hej alle sammen, dette er min første post!",
            Upvotes = 5,
            Downvotes = 0,
            PUserName = "Klaus",
            Comments = new List<Comment>
            {
                new Comment
                {
                    Id = 1,
                    Content = "Velkommen til forumet!",
                    Upvotes = 2,
                    Downvotes = 0,
                    CUserName = "Lars"
                },
                new Comment
                {
                    Id = 2,
                    Content = "God post – glæder mig til at læse mere.",
                    Upvotes = 3,
                    Downvotes = 1,
                    CUserName = "Sofie"
                }
            }
        };

        // Anden post (navn sat eksplicit)
        var post2 = new Post
        {
            Id = 2,
            Title = "En anden post",
            Content = "Dette er endnu en post, bare for at teste seed data.",
            Upvotes = 1,
            Downvotes = 0,
            PUserName = "Anders",
            Comments = new List<Comment>
            {
                new Comment
                {
                    Id = 3,
                    Content = "Interessant, fortæl mere!",
                    Upvotes = 1,
                    Downvotes = 0,
                    CUserName = "Maja"
                },
                new Comment
                {
                    Id = 4,
                    Content = "Enig, det lyder spændende!",
                    Upvotes = 0,
                    Downvotes = 0,
                    CUserName = "StoneMountain64" 
                }
            }
        };

        db.Posts.AddRange(post1, post2);
        db.SaveChanges();
    }
}


        // -----------------------
        // Get
        // -----------------------
        public List<Post> GetPosts()
        {
            return db.Posts
                .Include(p => p.Comments)
                .ToList();
        }

        public Post? GetPost(int id)
        {
            return db.Posts
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);
        }

        // -----------------------
        // Post
        // -----------------------
        public string CreatePost(string title, string content, string? pUserName = null)
        {
            if (string.IsNullOrWhiteSpace(pUserName))
                pUserName = "Anonymous";

            var post = new Post
            {
                Title = title ?? "",
                Content = content ?? "",
                PUserName = pUserName,
                Upvotes = 0,
                Downvotes = 0,
                Comments = new List<Comment>()
            };

            db.Posts.Add(post);
            db.SaveChanges();
            return "Post created";
        }

        public string AddCommentToPost(int postId, string content, string? cUserName = null)
        {
            var post = db.Posts
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == postId);

            if (post == null) return "Post not found";

            // Sæt navn til "Anonymous" hvis tomt
            if (string.IsNullOrWhiteSpace(cUserName))
                cUserName = "Anonymous";

            var newComment = new Comment
            {
                // Lad DB generere Id (vigtigt!)
                Content = content ?? "",
                Upvotes = 0,
                Downvotes = 0,
                CUserName = cUserName
            };

            post.Comments ??= new List<Comment>();
            post.Comments.Add(newComment);

            db.SaveChanges();
            return "Comment added";
        }




        // -----------------------
        // Votes (posts)
        // -----------------------
        public bool UpvotePost(int id)
        {
            var post = db.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null) return false;

            post.Upvotes += 1;
            db.SaveChanges();
            return true;
        }

        public bool DownvotePost(int id)
        {
            var post = db.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null) return false;

            post.Downvotes += 1;
            db.SaveChanges();
            return true;
        }

        // -----------------------
        // Votes (comments)
        // -----------------------
        public bool UpvoteComment(int postId, int commentId)
        {
            var post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
            if (post == null) return false;

            var comment = post.Comments?.FirstOrDefault(c => c.Id == commentId);
            if (comment == null) return false;

            comment.Upvotes += 1;
            db.SaveChanges();
            return true;
        }

        public bool DownvoteComment(int postId, int commentId)
        {
            var post = db.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
            if (post == null) return false;

            var comment = post.Comments?.FirstOrDefault(c => c.Id == commentId);
            if (comment == null) return false;

            comment.Downvotes += 1;
            db.SaveChanges();
            return true;
        }
    }
}
