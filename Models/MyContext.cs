using Microsoft.EntityFrameworkCore;

namespace finalGSA.Models {
    public class MyContext : DbContext {
        public MyContext (DbContextOptions options) : base (options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Request> Requests { get; set; }

    }
}