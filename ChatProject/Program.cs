using ChatProject.Data;
using ChatProject.Hubs;
using ChatProject.Repositories;
using ChatProject.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MessageContext>(options =>
    options.UseSqlite("Data Source=chatlog.db"), ServiceLifetime.Scoped);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowChat",  
        policy  =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Add services to the container.
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();

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
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/ChatHub");

app.Run();