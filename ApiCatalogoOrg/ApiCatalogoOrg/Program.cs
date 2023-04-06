using ApiCatalogoOrg.ApiEndPoints;
using ApiCatalogoOrg.AppServicesExtensions;
using ApiCatalogoOrg.Context;
using ApiCatalogoOrg.Models;
using ApiCatalogoOrg.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();


// Bloco de autorização Jwt do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiCAtalogoOrg", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.
                   Enter 'Bearer'[space].Example: \'Bearer 12345abcdef\'",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
    });
});



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
                 options
                 .UseMySql(connectionString,
                 ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication
                 (JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,

                         ValidIssuer = builder.Configuration["Jwt:Issuer"],
                         ValidAudience = builder.Configuration["Jwt:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey
                         (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

                     };
                 });

builder.Services.AddAuthorization();


var app = builder.Build();
// Configure the HTTP request pipeline.//--> m;etodo Configure()


app.MapAutenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutosEndpoints();


var environment = app.Environment;
app.UseExceptionHandling(environment)
    .UseSwaggerMiddleware()
    .UseAppCors();

app.UseAuthentication();
app.UseAuthorization();


app.Run();

