using LibraryManagement.BorrowingGrpcService;
using Microsoft.AspNetCore;

internal class Program
{
    public static void Main(string[] args)
    {
        var server = BuildWebHost(args);
        server.Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
    }
}
