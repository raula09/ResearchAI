using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ResearchCopilot.Api.Repos;
using ResearchCopilot.Api.Services;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<DocumentRepo>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped<ChunkingService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<RetrievalService>();

var key = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "dev";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "researchcopilot",
        ValidAudience = "researchcopilot",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
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
