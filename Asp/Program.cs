using System.Reflection;
using Asp.ControllerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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

var redis = ConnectionMultiplexer.Connect(redisConnectionString);

builder.Services.AddSingleton<ConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<StackExchange.Redis.IDatabase>(redis.GetDatabase());

#endregion

#region controller

builder.Services.TryAddServices();

builder.Services.AddControllers();

#endregion

#region swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "A Very Big Project",
            Description = "This is a very Big Project!!!!",
        }
    );

    // 允许解析xml
    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    // 允许 oneOf
    options.UseOneOfForPolymorphism();
    // 允许 allOf
    options.UseAllOfForInheritance();
    // 使用自定义 schemaId
    // options.CustomSchemaIds(type => $"{type.Name}_{type.GUID}");
});
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
