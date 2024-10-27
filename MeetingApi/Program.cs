using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Text;
using MeetingApi.Services;
using Microsoft.EntityFrameworkCore;
using MeetingApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Dodavanje EF Core-a i konekcijskog stringa za bazu podataka
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registracija AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


// Dodavanje JWT autentifikacije
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "your-issuer",
        ValidAudience = "your-audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"))
    };
});

// Omoguæavanje sesija
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Uèitavanje SMTP konfiguracije iz appsettings.json
var smtpConfig = builder.Configuration.GetSection("Smtp");
builder.Services.AddSingleton(sp => new SmtpClient(smtpConfig["Host"])
{
    Port = int.Parse(smtpConfig["Port"]),
    Credentials = new NetworkCredential(smtpConfig["User"], smtpConfig["Password"]),
    EnableSsl = bool.Parse(smtpConfig["EnableSsl"]),
});

// Registracija servisa
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7158")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");

// Autorizacija i autentifikacija
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
