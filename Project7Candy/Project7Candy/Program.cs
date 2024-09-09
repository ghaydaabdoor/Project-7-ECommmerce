using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.EntityFrameworkCore;
using Project7Candy.Models;
using Project7Candy.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;




internal class Program
{
    private static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<MyDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("YourConnectionString")));


        builder.Services.AddCors(options =>
       options.AddPolicy("Development", builder =>
       {
           builder.AllowAnyOrigin();
           builder.AllowAnyMethod();
           builder.AllowAnyHeader();
       }
       ));

        builder.Services.AddSingleton<IConverter, SynchronizedConverter>(provider =>
            new SynchronizedConverter(new PdfTools()));
       

        builder.Services.AddScoped<TokenGenerator>();
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = jwtSettings.GetValue<string>("Key");
        var issuer = jwtSettings.GetValue<string>("Issuer");
        var audience = jwtSettings.GetValue<string>("Audience");
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("JWT settings are not properly configured.");
        }

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("Jwt");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        });

        // PayPal Service Injection
        builder.Services.AddSingleton<PayPalService>(sp =>
        {
            // You can get these values from your appsettings.json or environment variables
            var clientId = builder.Configuration["PayPal:ClientId"];
            var clientSecret = builder.Configuration["PayPal:ClientSecret"];

            // Ensure you toggle this flag for live/sandbox environments
            var isLive = builder.Configuration.GetValue<bool>("PayPal:IsLive");

            return new PayPalService(clientId, clientSecret, isLive);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("Development");


        app.MapControllers();

        app.Run();
    }
}