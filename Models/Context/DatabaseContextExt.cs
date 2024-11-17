using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Models.Context;

public partial class DatabaseContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}