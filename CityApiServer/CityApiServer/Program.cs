var builder = WebApplication.CreateBuilder(args);

// Додаємо підтримку CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Дозволяє запити з будь-якого джерела
              .AllowAnyMethod()   // Дозволяє всі методи (GET, POST, PUT, DELETE)
              .AllowAnyHeader();  // Дозволяє будь-які заголовки
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Використовуємо CORS
app.UseCors("AllowAll");

// Включаємо маршрутизацію для контролерів
app.MapControllers();

app.Run();
