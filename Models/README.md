```shell
dotnet ef dbcontext scaffold "Server=your_server;Database=your_database;User=your_user;Password=your_password;" Pomelo.EntityFrameworkCore.MySql -o Models --data-annotations --context-dir Context --no-onconfiguring -f --context DatabaseContext
```
