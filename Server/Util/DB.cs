namespace Util.DB;

using Npgsql;
using dotenv.net;

class Database
{
    /// <summary>
    /// Queries postgres database and reads return stream.
    /// DotEnv.Load must be updated when env file changes.
    /// </summary>
    /// <param name="sqlQuery"></param>
    /// <param name="processRowAction">Takes in an action with a single reader parameter. Allows to create an action, then calling query and using said action as a param for handling DB stream.</param>
    public static void Query(string sqlQuery, Action<NpgsqlDataReader> processRowAction)
    {
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../.env" }, ignoreExceptions: false));
        string connectionString = $"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};Username={Environment.GetEnvironmentVariable("DATABASE_USER")};Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}";
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connected to db.");
                using (NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            processRowAction(reader);
                        }
                    }
                }
                connection.Close();
                Console.WriteLine("Connection Closed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error has occured: {ex.Message}");
        }
    }
}