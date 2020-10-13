using Microsoft.EntityFrameworkCore;

namespace NetCoreEfSamples.Context
{
    public class BaseSampleContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //IConfigurationRoot configuration = new ConfigurationBuilder()
                //   .SetBasePath(Directory.GetCurrentDirectory())
                //   .AddJsonFile("appsettings.json")
                //   .Build();
                //var connectionString = configuration.GetConnectionString("DbCoreConnectionString");
                optionsBuilder.UseSqlServer("Server=localhost;Database=EfCoreSample;Integrated Security=SSPI;persist security info=True;");
            }

            base.OnConfiguring(optionsBuilder);
        }

    }
}
