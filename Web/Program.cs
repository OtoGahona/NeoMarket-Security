var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();  // 👈 Necesario para Swagger
builder.Services.AddSwaggerGen();            // 👈 Registra los servicios de Swagger

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // 👈 Habilita Swagger JSON
    app.UseSwaggerUI();     // 👈 Habilita Swagger UI en /swagger
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
