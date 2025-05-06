using System.Text;
using ChatProject.ConfigModels;
using ChatProject.Data;
using ChatProject.Helpers;
using ChatProject.Hubs;
using ChatProject.Models.ChatUserModels;
using ChatProject.Repositories;
using ChatProject.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

string? dbConnection = builder.Configuration["DatabaseConnectionString"];

var allowedOrigins = new List<string?>
{
    builder.Configuration["AllowedOrigin"] ?? "http://localhost:5173",
    builder.Configuration["DevelopmentOrigin"]
};

if (!string.IsNullOrWhiteSpace(dbConnection))
{
    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseNpgsql(dbConnection));
}
else
{
    builder.Services.AddDbContext<ChatDbContext>(options =>
        options.UseSqlite("Data Source=chat.db"));
}


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowChat",  
        policy  =>
        {
            policy.WithOrigins(allowedOrigins.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray()!)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddIdentity<ChatUser, IdentityRole>()
    .AddEntityFrameworkStores<ChatDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Bind JWT settings from configuration
var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BelongToChannel", policy =>
    {
        policy.RequireClaim("Channel");
    });
});

// Add services to the container.
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<IChannelService, ChannelService>();

builder.Services.AddScoped<IChatUserRepository, ChatUserRepository>();
builder.Services.AddScoped<IChatUserService, ChatUserService>();

builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddSingleton(jwtSettings!);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowChat");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/ChatHub");

app.Run();
