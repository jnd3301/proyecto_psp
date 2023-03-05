using Microsoft.EntityFrameworkCore;

namespace test
{
    class EmailDb : DbContext
    {
        public EmailDb(DbContextOptions<EmailDb> options)
            : base(options) { }

        public DbSet<Email> Todos => Set<Email>();
    }
}
