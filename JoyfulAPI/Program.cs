using Joyful.API.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHostDbContext(builder.Configuration, "DefaultConnection");
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.RegisterServices();
builder.Services.AddAuthentication();
builder.Services.AddSwaggerGen(
    setupAction =>
    {
        setupAction.AddSecurityDefinition("JoyfulApiBearerAuth", new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            Description = "Input a valid JWT Bearer token to access this API",
        });

        setupAction.AddSecurityRequirement(new()
        {
            {
                new ()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "JoyfulApiBearerAuth"
                    }
                },
                new List<string>()
            }
        });
    }
);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"])
            )
        };
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
