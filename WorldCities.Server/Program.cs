using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Adds Serilog support
builder.Host.UseSerilog((ctx, lc) => lc
.ReadFrom.Configuration(ctx.Configuration)
.WriteTo.MSSqlServer(connectionString:
ctx.Configuration.GetConnectionString("DefaultConnection"),
restrictedToMinimumLevel: LogEventLevel.Information,
sinkOptions: new MSSqlServerSinkOptions
{
    TableName = "LogEvents",
    AutoCreateSqlDatabase = true,
    AutoCreateSqlTable = true,
})
.WriteTo.Console()
);

// Add services to the container.

builder.Services.AddControllers();
    //.AddJsonOptions(options =>
    //{
    //    //options.JsonSerializerOptions.WriteIndented = true;
    //    //If we wanted to switch from camelCase (default) to PascalCase,
    //    //we could also add the following option:
    //    //options.JsonSerializerOptions.PropertyNamingPolicy = null;
    //});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    .AddApiEndpoints()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<JwtHandler>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["JwtSettings:SecurityKey"]!))
    };
}).AddBearerToken(IdentityConstants.BearerScheme);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
