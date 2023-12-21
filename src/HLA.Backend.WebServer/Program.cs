using Microsoft.AspNetCore.ResponseCompression;
using HLA.Backend.WebServer.Extensions;
using HLA.Backend.WebServer.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
config.SetBasePath(Directory.GetCurrentDirectory());
config.AddEnvironmentVariables();

// Add services to the container.
services.AddControllers()
    .AddJsonOptions( options => 
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
services.AddSwagger(config);
services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});
services.AddControllersWithViews()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
services.AddApplicationServices(builder, config);
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddHealthChecks();
var app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwaggerDashboard(config);

// global request handler
app.UseMiddleware<RequestHandler>();

app.Run();
