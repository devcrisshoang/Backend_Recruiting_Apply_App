using Backend_Recruiting_Apply_App.Hubs;
using Microsoft.EntityFrameworkCore;
using TopCVSystemAPIdotnet.Data;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ Controllers
builder.Services.AddControllers();

// Thêm Swagger (chỉ dùng khi dev)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Kết nối database SQL Server
builder.Services.AddDbContext<RAADbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"))
);

// ✅ Cấu hình CORS (Cho phép tất cả nguồn)
builder.Services.AddCors(p =>
    p.AddPolicy("CORS_POLICY", build =>
    {
        build.WithOrigins("http://localhost:3000") // Ví dụ cho phép từ localhost:3000, có thể thay đổi tùy vào yêu cầu của bạn
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials();
    })
);

builder.Services.AddSignalR();

// Thêm cấu hình JWT (thêm khi cần dùng JWT)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var app = builder.Build();

// ✅ Bật Swagger khi ở chế độ Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Đảm bảo CORS chạy trước UseRouting()
app.UseCors("CORS_POLICY");
app.MapHub<MessageHub>("/messageHub");

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
