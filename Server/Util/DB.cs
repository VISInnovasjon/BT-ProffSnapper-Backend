namespace Util.DB;

using Npgsql;
using dotenv.net;

class Database
{
    /// <summary>
    /// Queries postgres database and reads return stream.
    /// </summary>
    /// <param name="sqlQuery"></param>
    /// <param name="processRowAction">Function determining how to process the return stream.</param>
    public static void Query(string sqlQuery, Action<NpgsqlDataReader> processRowAction)
    {
        DotEnv.Load();
        string connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")};Database={Environment.GetEnvironmentVariable("POSTGRES_NAME")}";
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connected to db.");
                using (var cmd = new NpgsqlCommand(sqlQuery, connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            processRowAction(reader);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error has occured: {ex.Message}");
        }
    }
}