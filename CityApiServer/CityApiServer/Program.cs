var builder = WebApplication.CreateBuilder(args);

// ������ �������� CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // �������� ������ � ����-����� �������
              .AllowAnyMethod()   // �������� �� ������ (GET, POST, PUT, DELETE)
              .AllowAnyHeader();  // �������� ����-�� ���������
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// ������������� CORS
app.UseCors("AllowAll");

// �������� ������������� ��� ����������
app.MapControllers();

app.Run();
