/* WIP */

namespace Server.Util;
using Server.Context;
using Server.Views;
using Microsoft.EntityFrameworkCore;

public class ScheduleUpdateFromProff : BackgroundService
{
    private readonly IServiceProvider _provider;
    private Timer? _timer;
    private ILogger<ScheduleUpdateFromProff> _logger;
    public ScheduleUpdateFromProff(ILogger<ScheduleUpdateFromProff> logger, IServiceProvider provider)
    {
        _provider = provider;
        _logger = logger;
    }
    protected override Task ExecuteAsync(CancellationToken token)
    {

        _logger.LogInformation("Starting Scheduler.");
        _timer = new Timer(RunFetchAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }
    public override Task StopAsync(CancellationToken token)
    {
        return base.StopAsync(token);
    }
    public override void Dispose()
    {
        base.Dispose();
    }
    private void RunFetchAsync(object? state)
    {
        using (var scope = _provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BtdbContext>();
            Console.WriteLine(context.Database.CanConnect());

            /* List<int> orgNrList = context.BedriftInfos.Where(b => b.Likvidert != true).Select(b => b.Orgnummer).ToList();
            List<ReturnStructure> fetchedData = [];
            if (orgNrList.Count > 0) fetchedData = await FetchProffData.GetDatabaseValues(orgNrList);
            if (fetchedData.Count > 0)
            {
                foreach (var param in fetchedData)
                {
                    Console.WriteLine($"Adding {param.Name} to DB");
                    try
                    {
                        await param.InsertIntoDatabase(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine("Insert Complete, updating delta.");
                try
                {
                    context.Database.ExecuteSqlRaw("SELECT update_delta()");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } */
            Console.WriteLine($"Scheduler updated.");
        }
    }
}