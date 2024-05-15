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
    public static void Query(string sqlQuery, Action<NpgsqlDataReader> processRowAction, List<NpgsqlParameter>? paramList = null)
    {
        DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../.env" }, ignoreExceptions: false));
        string connectionString = $"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};Username={Environment.GetEnvironmentVariable("DATABASE_USER")};Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};Database={Environment.GetEnvironmentVariable("DATABASE_NAME")}";
        Console.WriteLine(connectionString);
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connected to db.");
                using (NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, connection))
                {
                    if (paramList != null)
                    {
                        foreach (var param in paramList)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
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
            foreach (var key in ex.Data)
            {
                Console.WriteLine($"{key.ToString()}");
            }
        }
    }
    /// <summary>
    /// Takes in a list with elements of type T, and converts it to an SQL array.</br>It also needs a parameter name. </br>This represents where in the sql query this parameter gets injected.</br>If the name is listOfNumbers, it gets injected into @listOfNumbers.
    /// </summary>
    /// <typeparam name="T">Any type, consistent across the list.</typeparam>
    /// <param name="list">List containing elements of type T, currently only strings and numbers are supported. Strings gets converted into VARCHAR(255).</param>
    /// <param name="parameterName">the name of the sql parameter that this is injected in to the sql string as.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static NpgsqlParameter ConvertListToParameter<T>(List<T> list, string parameterName)
    {
        if (list == null || list.Count == 0)
        {
            throw new ArgumentOutOfRangeException("List cannot be empty.", nameof(list));
        }
        NpgsqlParameter? param = null;

        if (typeof(T) == typeof(int))
        {
            param = new NpgsqlParameter(parameterName, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer);
        }
        else if (typeof(T) == typeof(string))
        {
            param = new NpgsqlParameter(parameterName, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Varchar);
        }
        else
        {
            throw new NotSupportedException($"The type '{typeof(T)}' is not supported yet.");
        }
        param.Value = list;
        return param;
    }
}