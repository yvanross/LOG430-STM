using Microsoft.EntityFrameworkCore;

namespace Infrastructure.WriteRepositories;

public static class DbContextExtensions
{
    public static bool IsInMemory(this DbContext context)
    {
        return context.Database.ProviderName?.Equals("Microsoft.EntityFrameworkCore.InMemory") ?? true;
    }
}