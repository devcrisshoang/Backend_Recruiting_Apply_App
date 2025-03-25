using Microsoft.EntityFrameworkCore;
using TopCVSystemAPIdotnet.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RAADbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactNative", builder =>
    {
        builder.WithOrigins("http://192.168.137.230:8082")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5032); // HTTP
    options.ListenAnyIP(7093, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS với chứng chỉ mặc định
    });
});


builder.Services.AddControllers();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Sử dụng CORS
app.UseCors("AllowReactNative");
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();