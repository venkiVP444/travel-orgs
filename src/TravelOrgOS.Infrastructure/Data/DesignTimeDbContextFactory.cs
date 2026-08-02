using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TravelOrgOS.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TravelOrgOSDbContext>
{
    public TravelOrgOSDbContext CreateDbContext(string[] args)
    {
        const string connStr = @"Server=(localdb)\MSSQLLocalDB;Database=TravelOrgOS_Dev;Trusted_Connection=True;TrustServerCertificate=True;";
        DatabaseSafetyChecker.AssertConnectionIsLocalDbOnly(connStr);

        var optionsBuilder = new DbContextOptionsBuilder<TravelOrgOSDbContext>();
        optionsBuilder.UseSqlServer(connStr);

        return new TravelOrgOSDbContext(optionsBuilder.Options);
    }
}
