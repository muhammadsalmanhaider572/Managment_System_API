using Managment_System_API_Application.Interfaces;
using Managment_System_API_Application.Services;
using Managment_System_API_Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// CONTROLLERS
// =====================================================

builder.Services.AddControllers();


// =====================================================
// DATABASE
// =====================================================

builder.Services.AddScoped<DapperContext>();

builder.Services.AddScoped<
    IDbConnectionFactory,
    DbConnectionFactory>();


// =====================================================
// APPLICATION SERVICES
// =====================================================

builder.Services.AddScoped<IUserService, UserService>();


// =====================================================
// SWAGGER
// =====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// =====================================================
// BUILD
// =====================================================

var app = builder.Build();


// =====================================================
// SWAGGER
// =====================================================

app.UseSwagger();

app.UseSwaggerUI();


// =====================================================
// HTTPS
// =====================================================

app.UseHttpsRedirection();


// =====================================================
// AUTHORIZATION
// =====================================================

app.UseAuthorization();


// =====================================================
// CONTROLLERS
// =====================================================

app.MapControllers();


// =====================================================
// RUN
// =====================================================

app.Run();