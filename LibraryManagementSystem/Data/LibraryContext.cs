using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class LibraryContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<IssueRecord> IssueRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IssueRecord>()
                .HasOne(i => i.Book)
                .WithMany(b => b.IssueRecords)
                .HasForeignKey(i => i.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssueRecord>()
                .HasOne(i => i.Member)
                .WithMany(m => m.IssueRecords)
                .HasForeignKey(i => i.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
