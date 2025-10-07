using Microsoft.EntityFrameworkCore;
using Model;

namespace Data
{
    public class PostContext : DbContext
    {
        //DB<post> er indholdet af tabellen "Posts" i databasen
        //Posts er navnet på tabellen i databasen
        //Post er navnet på klassen i Model biblioteket
        //Set<Post>() er en metode i DbContext klassen som laver en DbSet
        public DbSet<Post> Posts => Set<Post>();
        //Samme herunder
        public DbSet<Comment> Comments => Set<Comment>();


        public PostContext (DbContextOptions<PostContext> options)
            : base(options)
        {
            // Den her er tom. Men ": base(options)" sikre at constructor
            // på DbContext super-klassen bliver kaldt.
        }
    }
}