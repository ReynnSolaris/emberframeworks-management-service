using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace EmberFrameworksService.Managers.SQL;

public class MySqlManager
{
    private MySqlConnection GetConnection(string connectionString)
    {
        try
        {
            return new MySqlConnection(connectionString);
        }
        catch (Exception e)
        {
           //_log.LogIncident(LogType.Error, e.Message+"\n"+e.StackTrace);
            throw e;
        }
    }
    
    public MySqlCommand BuildCommand(MySqlConnection connection, string cmdString, string[] cmdParameters)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand(cmdString, connection);
            cmdParameters = cmdParameters != null ? cmdParameters : new string[] { "INVALID" };
            Regex r = new Regex("@param\\d+");
            MatchCollection m = r.Matches(cmdString);
            //_log.LogIncident(0, $"Building MySQL Command: {cmdString}");
            string endCmd = cmdString;
            if (m.Count != 0)
            {
                if (cmdParameters.Length != m.Count)
                    throw new Exception("Parameters must reach the amount specified.");
                int s = 0;
                foreach (Match match in m)
                {
                    //_log.LogIncident(0, $"Adding Parameter #{s}: {match.Value}, {cmdParameters[s]}");
                    cmd.Parameters.AddWithValue(match.Value, cmdParameters[s]);
                    endCmd = endCmd.Replace(match.Value, cmdParameters[s]);
                    s += 1;
                }
                cmd.Prepare();
            }

            return cmd;
        }
        catch (Exception e)
        {
            //_log.LogIncident(LogType.Error, e.Message+"\n"+e.StackTrace);
            throw e;
        }
    }
    
    public void ExecuteNonQuery(string cmdString, string[]? cmdParameters, string ConnectionString)
    {
        try
        {
            using (MySqlConnection connection = GetConnection(ConnectionString))
            {
                connection.Open();
                MySqlCommand cmd = BuildCommand(connection, cmdString, cmdParameters);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (Exception e) {
            //_log.LogIncident(LogType.Error, e.Message+"\n"+e.StackTrace);
            throw e;
        }
    }
    
    public Dictionary<int, Dictionary<int, Object>> ExecuteQuery(string cmdString, string[]? cmdParameters, string connectionString)
    {
        Dictionary<int, Dictionary<int, Object>> QueryResults = new();
        try {
            using (MySqlConnection connection = GetConnection(connectionString))
            {
                connection.Open();
                MySqlCommand cmd = BuildCommand(connection, cmdString, cmdParameters);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    int currentRow = 0;
                    while (reader.Read())
                    {
                        QueryResults.Add(currentRow, new Dictionary<int, Object>() {});
                            
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (!reader.IsDBNull(i))
                            {
                                QueryResults[currentRow][i] = reader.GetValue(i);
                            }
                        }

                        currentRow += 1;
                    }
                        
                }
            }

            return QueryResults;
        }
        catch (Exception e) {
           // _log.LogIncident(LogType.Error, e.Message+"\n"+e.StackTrace);
            throw e;
        }
    }
}