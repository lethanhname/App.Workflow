using System.Collections.Generic;
using System.Threading.Tasks;
using App.Core.Workflow.Contract.Definition;
using App.Core.Workflow.Contract.Items;
using App.CoreLib.EF.Data;

namespace App.Core.Workflow.Contract.Scheduler
{
    public interface IWorkItemRepository : IRepository<WorkItem>
    {
        Task PersistWorkItemsAsync(IEnumerable<WorkItem> items);

        Task Reschedule(WorkItem model);

        Task<IEnumerable<WorkItem>> ResumeWorkItemsAsync();

        void AddAutoTrigger(AutoTrigger autoTrigger, IWorkflow entity);
    }
}