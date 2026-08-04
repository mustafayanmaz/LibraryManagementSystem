var builder = WebApplication.CreateBuilder(args);

// Controller sınıflarını projeye dahil eder.
builder.Services.AddControllers();

// React uygulamasının backend API'ye erişmesine izin verir.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("FrontendPolicy");

app.MapControllers();

app.Run();