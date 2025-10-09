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
                // Opret brugere
                var kristian = new User { Id = 1, Username = "Kristian" };
                var søren    = new User { Id = 2, Username = "Søren" };
                var mette    = new User { Id = 3, Username = "Mette" };
                var line     = new User { Id = 4, Username = "Line" };

                // Første post
                var post1 = new Post
                {
                    Id = 1,
                    User = kristian,
                    Title = "Min første post",
                    Content = "Hej alle sammen, dette er min første post!",
                    Upvotes = 5,
                    Downvotes = 0,
                    Comments = new List<Comment>
                    {
                        new Comment
                        {
                            Id = 1,
                            User = søren,
                            Content = "Velkommen til forumet!",
                            Upvotes = 2,
                            Downvotes = 0
                        },
                        new Comment
                        {
                            Id = 2,
                            User = mette,
                            Content = "God post – glæder mig til at læse mere.",
                            Upvotes = 3,
                            Downvotes = 1
                        }
                    }
                };

                // Anden post
                var post2 = new Post
                {
                    Id = 2,
                    User = søren,
                    Title = "En anden post",
                    Content = "Dette er endnu en post, bare for at teste seed data.",
                    Upvotes = 1,
                    Downvotes = 0,
                    Comments = new List<Comment>
                    {
                        new Comment
                        {
                            Id = 3,
                            User = kristian,
                            Content = "Interessant, fortæl mere!",
                            Upvotes = 1,
                            Downvotes = 0
                        },
                        new Comment
                        {
                            Id = 4,
                            User = line,
                            Content = "Enig, det lyder spændende!",
                            Upvotes = 0,
                            Downvotes = 0
                        }
                    }
                };

                // Tilføj brugere og posts til databasen
                db.Users.AddRange(kristian, søren, mette, line);
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
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .ToList();
        }

        public Post? GetPost(int id)
        {
            return db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefault(p => p.Id == id);
        }

        // -----------------------
        // Post
        // -----------------------
        public string CreatePost(User user, string title, string content)
        {
            int nextId = db.Posts.Select(p => p.Id).DefaultIfEmpty(0).Max() + 1;
            user.Username = string.IsNullOrWhiteSpace(user.Username) ? "Anonymous" : user.Username;

            var post = new Post
            {
                Id = nextId,
                User = user,
                Title = title ?? "",
                Content = content ?? "",
                Comments = new List<Comment>()
            };

            db.Posts.Add(post);
            db.SaveChanges();
            return "Post created";
        }

        public string AddCommentToPost(int postId, User user, string content)
        {
            var post = db.Posts
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == postId);

            if (post == null) return "Post not found";

            post.Comments ??= new List<Comment>();
            int nextCid = post.Comments.Select(c => c.Id).DefaultIfEmpty(0).Max() + 1;

            user.Username = string.IsNullOrWhiteSpace(user.Username) ? "Anonymous" : user.Username;

            var newComment = new Comment
            {
                Id = nextCid,
                User = user,
                Content = content ?? "",
                Upvotes = 0,
                Downvotes = 0
            };

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
