using LibraryManagement.BorrowingGrpcService;
using Microsoft.AspNetCore;
using Serilog;

internal class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            var server = BuildWebHost(args);
            server.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            })
            .Build();
    }
}
