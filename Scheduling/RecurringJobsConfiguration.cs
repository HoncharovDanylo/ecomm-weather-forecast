using Hangfire.ScheduledJobs;
using Hangfire.Storage;
using Infrastructure.Configs;

namespace Hangfire;

public class RecurringJobsConfiguration
{
    private readonly IMainConfiguration _mainConfiguration;

    public RecurringJobsConfiguration(IMainConfiguration mainConfiguration)
    {
        _mainConfiguration = mainConfiguration;
    }

    public void RegisterAllTasks()
    {
        if (_mainConfiguration.IsTelegramIntegrationEnabled)
        {
            RecurringJob.AddOrUpdate<BroadcastHangfireTask>(r => r.ExecuteAsync(null, JobCancellationToken.Null), "* 9 * * *", TimeZoneInfo.Local);
        }
    }
    
    public void RemoveAllTasks()
    {
        using (var connection = JobStorage.Current.GetConnection())
        {
            foreach (var recurringJob in connection.GetRecurringJobs())
            {
                RecurringJob.RemoveIfExists(recurringJob.Id);
            }
        }
    }
}