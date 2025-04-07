using Microsoft.EntityFrameworkCore;

namespace BookstoreApi
{
    public class BookDb : DbContext
    {
        public BookDb(DbContextOptions<BookDb> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();
    }
}
