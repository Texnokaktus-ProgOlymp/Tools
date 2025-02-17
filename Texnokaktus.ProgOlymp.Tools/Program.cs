using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;
using Texnokaktus.ProgOlymp.Tools.Services;
using Texnokaktus.ProgOlymp.Tools.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddScoped<IToolsService, DatabaseToolsService>()
       .AddScoped<IToolsService, CacheToolsService>();

builder.Services.AddOpenApi();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// builder.Services
//        .AddAuthentication(options =>
//         {
//             options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//             options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//             options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
//         })
//        .AddConfiguredJwtBearer(builder.Configuration);

// builder.Services.AddAuthorization();

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.ConfigObject.Urls = [new() { Name = "v1", Url = "/openapi/v1.json" }]);
}

// app.UseAuthentication();
// app.UseAuthorization();

app.MapPost("wipe", async Task<Results<NoContent, ForbidHttpResult>>(TimeProvider timeProvider, IEnumerable<IToolsService> services) =>
{
    if (timeProvider.GetUtcNow() >= new DateTimeOffset(2025, 02, 20, 00, 00, 00, TimeSpan.FromHours(3)))
        return TypedResults.Forbid();

    foreach (var service in services)
        await service.ExecuteWipeAsync();

    return TypedResults.NoContent();
});

await app.RunAsync();

