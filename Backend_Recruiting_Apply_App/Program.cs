using Backend_Recruiting_Apply_App.Hubs;
using Microsoft.EntityFrameworkCore;
using TopCVSystemAPIdotnet.Data;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ Controllers
builder.Services.AddControllers();

// Thêm Swagger (chỉ dùng khi dev)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Kết nối database SQL Server
builder.Services.AddDbContext<RAADbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"))
);

// Cấu hình CORS
builder.Services.AddCors(p =>
    p.AddPolicy("CORS_POLICY", build =>
    {
        build.WithOrigins(
                "http://192.168.1.12:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    })
);

builder.Services.AddSignalR();

// Thêm cấu hình JWT (thêm khi cần dùng JWT)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var app = builder.Build();

// Bật Swagger khi ở chế độ Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Thêm WebSocket trước MapHub
app.UseWebSockets();
app.UseCors("CORS_POLICY");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHub<MessageHub>("/messageHub");
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();

app.Run();