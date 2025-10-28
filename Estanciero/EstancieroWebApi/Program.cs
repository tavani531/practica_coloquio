var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MiPoliticaDeCORS",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                  .AllowAnyHeader()  
                  .AllowAnyMethod(); 
        });
});

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI();
}

app.UseCors("MiPoliticaDeCORS");
app.UseAuthorization();
app.MapControllers();
app.Run();

//Este código configura e inicia una API web en .NET, habilitando la documentación Swagger en modo de desarrollo
//y aplicando una política de seguridad (CORS) para permitir que páginas web específicas (como http://127.0.0.1:5500)
//puedan consumir los datos de esta API.