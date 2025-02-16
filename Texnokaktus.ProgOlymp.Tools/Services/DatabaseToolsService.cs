using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Texnokaktus.ProgOlymp.Tools.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Tools.Services;

public class DatabaseToolsService(IConfiguration configuration, ILogger<DatabaseToolsService> logger) : IToolsService
{
    public async Task ExecuteWipeAsync()
    {
        var connectionString = configuration.GetConnectionString("DefaultDb");

        await using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        var tables = configuration.GetSection("WipeTables").Get<IEnumerable<string>>() ?? [];

        var commandText = GetTruncateCommandText(tables);

        logger.LogInformation("Executing SQL command:\n{Command}", commandText);

        await using var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.CommandType = CommandType.Text;

        await command.ExecuteNonQueryAsync();

        logger.LogInformation("SQL executed");
    }

    private static string GetTruncateCommandText(IEnumerable<string> tables) =>
        tables.Aggregate(new StringBuilder(),
                         (builder, table) => builder.AppendLine($"TRUNCATE TABLE {table};"))
              .ToString();
}
