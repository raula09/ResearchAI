
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ResearchCopilot.Api.Repos;
using ResearchCopilot.Api.Services;
using Swashbuckle.AspNetCore.SwaggerUI;
using ResearchCopilot.Api.Utils;
using ResearchCopilot.Api.Database;
using System.IO;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
var root = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
var envPath = Path.Combine(root ?? "", ".env");

if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"✅ Loaded .env from: {envPath}");
}
else
{
    Console.WriteLine($"⚠️ Could not find .env at: {envPath}");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SupportNonNullableReferenceTypes();
    c.DescribeAllParametersInCamelCase();
    c.OperationFilter<FileUploadOperationFilter>();

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token.\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR..."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<DocumentRepo>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddHttpClient<OpenRouterService>();
builder.Services.AddScoped<ChunkingService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<RetrievalService>();
builder.Services.AddScoped<ChatRepo>();
builder.Services.AddScoped<OpenRouterService>();

var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "this_is_a_development_jwt_secret_please_change_it_to_32chars_minimum";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "researchcopilot",
            ValidAudience = "researchcopilot",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireAuthenticatedUser());
});
builder.Services.AddCors(o => o.AddPolicy("all", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("all");
app.UseAuthentication();  
app.UseAuthorization(); 
app.MapControllers();

app.Run();
