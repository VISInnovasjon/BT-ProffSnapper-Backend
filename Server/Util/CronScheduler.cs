/* WIP */

/* namespace Util.CronScheduler;
using Cronos;
class ScheduleFetchFromProff{
    private static CronExpression _cronExpression;
    private static DateTime _nextRunTime;
    public static async Task FetchProffData(int[] orgnrs){
        string cronExpression = " * * /1 * * *";
        _cronExpression = CronExpression.Parse(cronExpression);
        _nextRunTime = _cronExpression.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local) ?? DateTime.MaxValue;
        var cts = new CancellationTokenSource();
        var task = RunFetchAsync(cts.Token);
        Console.WriteLine("CronScheduler Started, press any key to crash the server.");
        Console.ReadLine();
        cts.Cancel();
        await task;
    }
    public static async Task RunFetchAsync(CancellationToken token){
        while(!token.IsCancellationRequested){
            DateTime now = DateTime.UtcNow;
            if(now>=_nextRunTime){
                await FetchFromProff();
                _nextRunTime = _cronExpression.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local) ?? DateTime.MaxValue;
            }
            await Task.Delay(_nextRunTime-now, token);
        }
    }
}
 */