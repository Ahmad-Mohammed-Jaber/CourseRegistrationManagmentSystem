using Serilog.Core;

namespace Shared.Logging;

public static class MssqlSinkFactory
{
    public static ILogEventSink Create(string connectionString, string table)
    {
        return new MssqlExceptionSink(connectionString, ValidateTable(table));
    }

    private static string ValidateTable(string table)
    {
        if (string.IsNullOrWhiteSpace(table))
        {
            throw new ArgumentException("Table name is required.", nameof(table));
        }

        foreach (char c in table)
        {
            if (!char.IsLetterOrDigit(c) && c != '_')
            {
                throw new ArgumentException($"Invalid table name: {table}", nameof(table));
            }
        }

        MssqlExceptionSink.ResolveProcedure(table);

        return table;
    }
}
