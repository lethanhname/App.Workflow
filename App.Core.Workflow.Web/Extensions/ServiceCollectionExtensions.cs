using App.Core.Workflow.Business.Scheduler;
using App.Core.Workflow.Contract.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Core.Workflow.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJobQueueServices(this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, WorkflowProcessor>();
            services.AddSingleton<IJobQueueService, JobQueueService>();

            services.AddScoped<IWorkItemRepository, WorkItemRepository>();

            return services;
        }
    }
}