using Backend_Recruiting_Apply_App.Hubs;
using Backend_Recruiting_Apply_App.Services;
using Microsoft.EntityFrameworkCore;
using SystemAPIdotnet.Data;
using Net.payOS; // Đảm bảo thêm using này

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

// Thêm cấu hình JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Đăng ký PayOS
builder.Services.AddScoped<PayOS>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var clientId = configuration["PayOS:ClientId"];
    var apiKey = configuration["PayOS:ApiKey"];
    var checksumKey = configuration["PayOS:ChecksumKey"];
    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(checksumKey))
    {
        throw new InvalidOperationException("PayOS configuration is missing or invalid in appsettings.json");
    }
    return new PayOS(clientId, apiKey, checksumKey);
});

// Đăng ký các dịch vụ khác
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicantService, ApplicantService>();
builder.Services.AddScoped<IApplyService, ApplyService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobNameService, JobNameService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<IRecruiterService, RecruiterService>();
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build(); // Đảm bảo chỉ gọi Build() một lần tại đây

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