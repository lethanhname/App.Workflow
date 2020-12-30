using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Core.Workflow.Contract.Definition;
using App.Core.Workflow.Contract.Execution;
using App.Core.Workflow.Contract.Items;
using App.Core.Workflow.Contract.Scheduler;
using App.Core.Workflow.Contract.Services;
using App.CoreLib.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Core.Workflow.Business.Scheduler
{
    public class JobQueueService : IJobQueueService
    {
        private readonly ILogger<JobQueueService> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private ConcurrentQueue<WorkItem> items;

        private ConcurrentQueue<WorkItem> Items
        {
            get
            {
                if (items == null)
                {
                    items = new ConcurrentQueue<WorkItem>();
                }

                return items;
            }
        }

        public JobQueueService(
          ILogger<JobQueueService> logger,
          IServiceScopeFactory serviceScopeFactory
        )
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public JobQueueService(ILogger<JobQueueService> logger)
        {
            this.logger = logger;
        }

        public async Task Enqueue(WorkItem item)
        {
            this.logger.LogTrace("Enqueue work item", item);

            if (item.Retries > WorkItem.WORKITEM_RETRIES)
            {
                this.logger.LogInformation("Amount of retries for work item {WorkItem} exceeded!",
                  LogHelper.SerializeObject(item)
                );

                await this.PersistWorkItemsAsync(new List<WorkItem> { item });
            }
            else
            {
                this.Items.Enqueue(item);

                await Task.CompletedTask;
            }
        }

        public async Task ProcessItemsAsync()
        {
            while (this.Items.Count > 0)
            {
                var item = this.Dequeue();
                if (item == null) continue;

                try
                {
                    this.logger.LogTrace("Processing work item {WorkItem}", LogHelper.SerializeObject(item)
                    );

                    await this.ProcessItemInternal(item);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(
                      ex,
                      "Processing of work item {WorkItem} failed",
                      LogHelper.SerializeObject(item)
                    );
                    item.Error = $"{ex.Message} - {ex.StackTrace}";
                    item.Retries++;

                    await this.Enqueue(item);
                }
            }

            await Task.CompletedTask;
        }

        public async Task ResumeWorkItems()
        {
            this.logger.LogTrace("Resuming work items");

            using (var scope = this.serviceScopeFactory.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                var service = serviceProvider.GetRequiredService<IWorkItemRepository>();
                var items = await service.ResumeWorkItemsAsync();

                foreach (var item in items)
                {
                    await this.Enqueue(item);
                }
            }
        }

        public async Task PersistWorkItemsAsync()
        {
            this.logger.LogTrace("Persisting work items");

            await this.PersistWorkItemsAsync(this.Items.ToArray());
        }

        public IEnumerable<WorkItem> GetSnapshot()
        {
            return this.Items.ToArray();
        }

        private WorkItem Dequeue()
        {
            if (this.Items.TryDequeue(out WorkItem item))
            {
                item.Error = string.Empty;

                return item;
            }

            return null;
        }

        private async Task PersistWorkItemsAsync(IEnumerable<WorkItem> items)
        {
            this.logger.LogTrace("Persisting work items");

            using (var scope = this.serviceScopeFactory.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                var service = serviceProvider.GetRequiredService<IWorkItemRepository>();
                await service.PersistWorkItemsAsync(items);
            }
        }

        private async Task DeleteWorkItem(WorkItem item)
        {
            this.logger.LogTrace("Deleting work item {WorkItem}", item);

            using (var scope = this.serviceScopeFactory.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                var service = serviceProvider.GetRequiredService<IWorkItemRepository>();
                service.Remove(item.IdentityId);
                await service.SaveChangesAsync();
            }
        }

        private async Task ProcessItemInternal(WorkItem item)
        {
            TriggerResult triggerResult = await this.ProcessItemAsync(item);
            await this.HandleTriggerResult(triggerResult, item);
        }

        private async Task<TriggerResult> ProcessItemAsync(WorkItem item)
        {
            this.logger.LogTrace("Processing work item {WorkItem}", item);

            using (var scope = this.serviceScopeFactory.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                var engine = serviceProvider.GetRequiredService<IWorkflowEngineService>();
                var workflowDefinitionProvider
                  = serviceProvider.GetRequiredService<IWorkflowDefinitionProvider>();

                var workflowDefinition = (IWorkflowDefinition)workflowDefinitionProvider
                                                   .GetWorkflowDefinition(item.WorkflowType);

                var workflow = await engine.FindAsync(item.EntityId, workflowDefinition.WorkflowName);
                TriggerParam triggerParam = new TriggerParam(workflow, item.TriggerName);

                return await engine.TriggerAsync(triggerParam);
            }
        }

        private async Task HandleTriggerResult(TriggerResult triggerResult, WorkItem item)
        {
            if (triggerResult.HasErrors || triggerResult.IsAborted)
            {
                item.Error = string.Join(" - ", triggerResult.Errors);
                this.logger.LogError(
                  "Bad TriggerResult: {TriggerResult}",
                  LogHelper.SerializeObject(triggerResult)
                );

                item.Retries++;
                await this.Enqueue(item);
            }
            else
            {
                if (item.IdentityId > 0)
                {
                    // Delete it from db if it was once persisted!
                    await this.DeleteWorkItem(item);
                }
            }
        }
    }
}