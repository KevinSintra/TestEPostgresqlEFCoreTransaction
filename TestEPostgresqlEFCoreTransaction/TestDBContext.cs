using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace TestEPostgresqlEFCoreTransaction
{
    public class TestDBContext : DbContext
    {
        public DbSet<pg_stat_statements_record> pg_stat_statements_record { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
#if DEBUG
            // Logging in Entity Framework Core: https://www.entityframeworktutorial.net/efcore/logging-in-entityframework-core.aspx
            // How to create a LoggerFactory with a ConsoleLoggerProvider? https://stackoverflow.com/questions/53690820/how-to-create-a-loggerfactory-with-a-consoleloggerprovider

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Debug)
            );
            var loggerFactory = serviceCollection.BuildServiceProvider()
                .GetService<ILoggerFactory>();

            optionsBuilder.UseLoggerFactory(loggerFactory)  //tie-up DbContext with LoggerFactory object
            .EnableSensitiveDataLogging()
            .UseNpgsql("Server=192.168.10.168;Port=5432;Database=pbn;User Id=dpasa;Password=postgres;Pooling=true;");
            //.AddInterceptors( new WithLockDbCommandInterceptor());

#else
            optionsBuilder.UseSqlServer("server=localhost;database=TestDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite");
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
