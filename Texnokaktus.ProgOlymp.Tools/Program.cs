using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using Texnokaktus.ProgOlymp.Tools.Extensions;
using Texnokaktus.ProgOlymp.Tools.Services;
using Texnokaktus.ProgOlymp.Tools.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddScoped<IToolsService, DatabaseToolsService>()
       .AddScoped<IToolsService, CacheToolsService>();

builder.Services.AddOpenApi();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
       .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddConfiguredJwtBearer(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.ConfigObject.Urls = [new() { Name = "v1", Url = "/openapi/v1.json" }]);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("wipe", async (IEnumerable<IToolsService> services) =>
{
    foreach (var service in services)
        await service.ExecuteWipeAsync();
});

await app.RunAsync();

