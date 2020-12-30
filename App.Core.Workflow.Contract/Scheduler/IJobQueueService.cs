using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Core.Workflow.Contract.Scheduler
{
    public interface IJobQueueService
    {
        Task Enqueue(WorkItem workItem);

        Task ProcessItemsAsync();

        Task ResumeWorkItems();

        Task PersistWorkItemsAsync();

        IEnumerable<WorkItem> GetSnapshot();
    }
}