using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Dashboard.Common
{
    public class DatabaseHelper
    {
        public static void RunSqlFileUpdateDatabaseWebsite()
        {
            const string databaseFolder = "/DataScripts";
            var fullPath = HostingEnvironment.MapPath(databaseFolder);
            var fileVersions = new List<string>();

            foreach (var file in Directory.GetFiles(fullPath, "*.sql").Select(Path.GetFileName))
            {
                var fileNameSplit = file.Split('_');
                if (fileNameSplit.Length > 1)
                {
                    var checkVersionValid = UltiHelper.ConvertStringToDecimal(fileNameSplit[0]);
                    if (checkVersionValid != null && checkVersionValid > 0)
                    {
                        fileVersions.Add(fileNameSplit[0]);
                        continue;
                    }
                }
            }

            if (fileVersions.Count > 0)
            {
                var duplicateVersion = fileVersions
                    .GroupBy(x => x)
                    .Where(g => g.Count() > 1)
                    .Select(y => y.Key)
                    .ToList();
                if (duplicateVersion.Count > 0)
                {
                    throw new Exception($"Duplicate file .sql version: {string.Join(", ", duplicateVersion)}");
                }
            }

            var dataVersions = new List<string>();

            var connStr = WebConfigurationManager.ConnectionStrings["ConnectDB"].ConnectionString;
            var conn = new SqlConnection(connStr);

            try
            {
                conn.Open();
                using (var comm = new SqlCommand("SELECT Name FROM DatabaseVersion", conn))
                {
                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                dataVersions.Add(reader["Name"].ToString());
                            }
                        }
                    }
                }

                var result = fileVersions
                    .Except(dataVersions)
                    .OrderBy(v => v) // ensure deterministic ascending execution by version
                    .ToList();
                if (result.Count > 0)
                {
                    foreach (var file in result)
                    {
                        var files = Directory.GetFiles(fullPath, $"{file}_*.sql").FirstOrDefault();

                        string dataScript = System.IO.File.ReadAllText(files);
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = dataScript;
                            var rows_affected = cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                conn.Close();
            }
        }

    }
}