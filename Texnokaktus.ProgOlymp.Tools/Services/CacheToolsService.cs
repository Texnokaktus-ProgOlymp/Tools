using Docker.DotNet;
using Texnokaktus.ProgOlymp.Tools.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Tools.Services;

public class CacheToolsService(IConfiguration configuration, ILogger<CacheToolsService> logger) : IToolsService
{
    public async Task ExecuteWipeAsync()
    {
        var endpoint = new Uri(configuration.GetConnectionString("DefaultDocker")!);
        using var client = new DockerClientConfiguration(endpoint).CreateClient();

        var containers = configuration.GetSection("RestartContainers").Get<IEnumerable<string>>() ?? [];
        
        foreach (var container in containers)
        {
            logger.LogInformation("Restarting container {ContainerId}", container);
            await client.Containers.RestartContainerAsync(container, new());
            logger.LogInformation("Container {ContainerId} restarted", container);
        }
    }
}
