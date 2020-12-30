using System.Collections.Generic;
using System.Threading.Tasks;
using App.CoreLib.EF.Data;

namespace App.Core.Workflow.Contract.Scheduler
{
    public interface IWorkItemRepository : IRepository<WorkItem>
    {
        Task PersistWorkItemsAsync(IEnumerable<WorkItem> items);

        Task Reschedule(WorkItem model);

        Task<IEnumerable<WorkItem>> ResumeWorkItemsAsync();
    }
}