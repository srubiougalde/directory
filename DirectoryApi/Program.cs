using System.Text;
using System.Text.Json.Serialization;
using DirectoryApi;
using DirectoryApi.Auth;
using DirectoryApi.Repositories;
using DirectoryApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DirectoryApi.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
                .AddJsonOptions(c => c.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddDataAnnotationsLocalization(c => c.DataAnnotationLocalizerProvider = (t, f) => f.Create(typeof(SharedResource)));

builder.Services.AddLocalization(c => c.ResourcesPath = "Resources");

builder.Services.AddCors(c => c.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ToDo. Add an other db provider when is not Development environment
// Configure DbContext to use InMemoryDatabase
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase("DirectoryDb"));

// Configure dependencies
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IWebsiteRepository, WebsiteRepository>();
builder.Services.AddScoped<IDirectoryService, DirectoryService>();
builder.Services.AddScoped<IWebsiteService, WebsiteService>();

builder.Services.AddSingleton<IJwtFactory, JwtFactory>();
builder.Services.AddSingleton<IAuthorizationCodeFactory, AuthorizationCodeFactory>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<IUsersService, UsersService>();

// jwt wire up
var secret = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("this is my custom Secret key for authentication"));

// Configure JwtIssuerOptions
builder.Services.Configure<JwtIssuerOptions>(options =>
{
    options.Issuer = "https://localhost:5001";
    options.Audience = "https://localhost:5001";
    options.SigningCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
});

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = "https://localhost:5001",

    ValidateAudience = true,
    ValidAudience = "https://localhost:5001",

    ValidateIssuerSigningKey = true,
    IssuerSigningKey = secret,

    RequireExpirationTime = false,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(configureOptions =>
{
    configureOptions.ClaimsIssuer = "https://localhost:5001";
    configureOptions.RequireHttpsMetadata = false;
    configureOptions.SaveToken = true;
    configureOptions.TokenValidationParameters = tokenValidationParameters;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DirectoryApi", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "DirectoryApi v1");
                    x.RoutePrefix = string.Empty;
                });
}
else
{
    app.UseExceptionHandler(c => c.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerPathFeature>()
            .Error;
        var response = new { error = exception.Message };
        await context.Response.WriteAsJsonAsync(response);
    }));
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/ping", () =>
{
    return "pong";
});

if (app.Environment.IsDevelopment())
{
    app.MapGet("/api/throw", () =>
    {
        throw new Exception("Sample exception.");
    });
}

app.Run();
