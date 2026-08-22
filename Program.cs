using Managment_System_API_Application.Interfaces;
using Managment_System_API_Application.Services;
using Managment_System_API_Infrastructure.Database;
using Managment_System_Application.Interfaces;
using Managment_System_Application.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using System.Text;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// 1. DATABASE / INFRASTRUCTURE
// =====================================================

builder.Services.AddSingleton<IDbConnectionFactory, DapperContext>();


// =====================================================
// 2. APPLICATION SERVICES
// =====================================================

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IAuthService, AuthService>();


// =====================================================
// 3. CONTROLLERS
// =====================================================

builder.Services.AddControllers();


// =====================================================
// 4. JWT SETTINGS
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
// 5. JWT AUTHENTICATION
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
// 6. AUTHORIZATION
// =====================================================

builder.Services.AddAuthorization();


// =====================================================
// 7. SWAGGER
// =====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // =================================================
    // SWAGGER DOCUMENT
    // =================================================

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
    // JWT SECURITY DEFINITION
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


    // =================================================
    // JWT SECURITY REQUIREMENT
    // =================================================

    options.AddSecurityRequirement(
        document =>
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)]
                    = new List<string>()
            });
});


// =====================================================
// 8. BUILD APPLICATION
// =====================================================

var app = builder.Build();


// =====================================================
// 9. SWAGGER MIDDLEWARE
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Management System API v1");

        // Swagger opens directly at:
        //
        // https://localhost:XXXX/
        //

        options.RoutePrefix = string.Empty;
    });
}


// =====================================================
// 10. HTTPS
// =====================================================

app.UseHttpsRedirection();


// =====================================================
// 11. AUTHENTICATION
// =====================================================

app.UseAuthentication();


// =====================================================
// 12. AUTHORIZATION
// =====================================================

app.UseAuthorization();


// =====================================================
// 13. CONTROLLERS
// =====================================================

app.MapControllers();


// =====================================================
// 14. RUN
// =====================================================

app.Run();