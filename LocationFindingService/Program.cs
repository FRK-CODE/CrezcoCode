using IPinfo;
using LocationFindingService.Data;
using LocationFindingService.Extensions;
using LocationFindingService.Models.Data;
using LocationFindingService.Models.Services;
using LocationFindingService.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

AddServices(builder);

builder.Services.AddEndpointDefinitions();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Would use REDIS here for caching rather than relying on in memory due to the possible need to scale this service
builder.Services.AddOutputCache();

var app = builder.Build();

app.UseExceptionHandler();
app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseEndpointDefinitions();

app.Run();

void AddServices(WebApplicationBuilder builder)
{    
    builder.Services.AddTransient<ILocationLookupProvider, IpInfoService>();
    builder.Services.AddTransient<ILocationRepository, LocationRepository>();

    builder.Services.AddDbContext<DatabaseContext>();
}
