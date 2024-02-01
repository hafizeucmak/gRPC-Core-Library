using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData.Utils
{
    public class DatabaseUtils
    {
        public static void DropTables(DbContext context, string scriptName, bool isString = false)
        {
            string sqlCode = !isString ? Encoding.UTF8.GetString(GetResourceData(scriptName)) : GetResourceString(scriptName);
           
            if (string.IsNullOrEmpty(sqlCode))
            {
                throw new InvalidOperationException($"Cannot find embedded resource {scriptName}");
            }
            else
            {
                context.Database.ExecuteSqlRaw(sqlCode);

                if (context.Database.CurrentTransaction != null)
                {
                    context.Database.CommitTransaction();
                }
            }
        }

        public static byte[] GetResourceData(string resourceName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            if (executingAssembly == null)
            {
                throw new Exception();
            }

            var embedResource = executingAssembly?.GetManifestResourceNames().FirstOrDefault(resource => resource.Contains(resourceName));

            if (!string.IsNullOrWhiteSpace(embedResource))
            {
                using (Stream stream = executingAssembly.GetManifestResourceStream(embedResource))
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    return data;
                }
            }

            return new byte[] { };
        }

        private static string GetResourceString(string resourceName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var sqlScriptContent = string.Empty;

            if (executingAssembly == null)
            {
                throw new InvalidOperationException($"Failed to retrieve executing assembly.");
            }

            var embedResource = executingAssembly?.GetManifestResourceNames().FirstOrDefault(resource => resource.Contains(resourceName));

            if (string.IsNullOrEmpty(embedResource))
            {
                throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
            }

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embedResource))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Failed to retrieve embedded resource stream for '{embedResource}'.");
                }

                using (var reader = new StreamReader(stream))
                {
                    sqlScriptContent = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(sqlScriptContent))
                    {
                        throw new InvalidDataException($"Could not load script file. ({embedResource})");
                    }
                }
            }

            return sqlScriptContent;
        }
    }
}
