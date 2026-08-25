using Managment_System_API_Application.Interfaces;
using Managment_System_API_Application.Services;
using Managment_System_API_Infrastructure.Database;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using System.Text;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// 1. DATABASE CONNECTION
// =====================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection is missing in appsettings.json.");


// =====================================================
// 2. DATABASE / DAPPER
// =====================================================

builder.Services.AddScoped<
    IDbConnectionFactory,
    DapperContext>();


// =====================================================
// 3. APPLICATION SERVICES
// =====================================================

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IFileService, FileService>();


// =====================================================
// 4. CONTROLLERS
// =====================================================

builder.Services.AddControllers();


// =====================================================
// 5. JWT SETTINGS
// =====================================================

var jwtSettings =
    builder.Configuration.GetSection("Jwt");

var jwtKey =
    jwtSettings["Key"]
    ?? throw new InvalidOperationException(
        "JWT Key is missing in appsettings.json.");

var jwtIssuer =
    jwtSettings["Issuer"]
    ?? throw new InvalidOperationException(
        "JWT Issuer is missing in appsettings.json.");

var jwtAudience =
    jwtSettings["Audience"]
    ?? throw new InvalidOperationException(
        "JWT Audience is missing in appsettings.json.");


// =====================================================
// 6. JWT AUTHENTICATION
// =====================================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,

                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
            };
    });


// =====================================================
// 7. AUTHORIZATION
// =====================================================

builder.Services.AddAuthorization();


// =====================================================
// 8. SWAGGER
// =====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Management System API",

            Version = "v1",

            Description =
                "University Management System REST API"
        });


    // =================================================
    // JWT SECURITY
    // =================================================

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description =
                "Enter your JWT token."
        });


    options.AddSecurityRequirement(
        document =>
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)
                ] = []
            });
});


// =====================================================
// 9. BUILD
// =====================================================

var app = builder.Build();


// =====================================================
// 10. SWAGGER
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Management System API v1");

        options.RoutePrefix = "swagger";
    });
}


// =====================================================
// 11. HTTPS
// =====================================================

app.UseHttpsRedirection();


// =====================================================
// 12. AUTHENTICATION
// =====================================================

app.UseAuthentication();


// =====================================================
// 13. AUTHORIZATION
// =====================================================

app.UseAuthorization();


// =====================================================
// 14. CONTROLLERS
// =====================================================

app.MapControllers();


// =====================================================
// 15. RUN
// =====================================================

app.Run();