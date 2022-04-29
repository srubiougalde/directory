using DirectoryApi;
using DirectoryApi.Repositories;
using DirectoryApi.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "DirectoryApi", Version = "v1" }));

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
