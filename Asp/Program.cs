using System.Reflection;
using Asp.ControllerServices;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

#region db

// mysql
string mysqlConnectionString =
    builder.Configuration.GetConnectionString("MysqlConnection")
    ?? throw new InvalidOperationException("Connection string 'MysqlConnection' not found.");

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(mysqlConnectionString, new MySqlServerVersion(new Version(8, 4, 3)))
);

// redis
string redisConnectionString =
    builder.Configuration.GetConnectionString("RedisConnection")
    ?? throw new InvalidOperationException("Connection string 'RedisConnection' not found.");

IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);

builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<StackExchange.Redis.IDatabase>(redis.GetDatabase());

#endregion

#region controller

builder.Services.TryAddServices();

builder.Services.AddControllers();

#endregion

#region swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.MapSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
