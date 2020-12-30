using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Workflow.Contract.Scheduler;
using App.CoreLib.EF;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Repositories;

namespace App.Core.Workflow.Business.Scheduler
{
    public class WorkItemRepository : Repository<WorkItem>, IWorkItemRepository
    {
        public WorkItemRepository(IStorage context) : base(context)
        {
        }
        public async Task PersistWorkItemsAsync(IEnumerable<Contract.Scheduler.WorkItem> items)
        {
            // updates
            var updates = items.Where(p => p.RowVersion > 0);
            this.DbSet.UpdateRange(updates);

            // new items
            var inserts = items.Where(p => p.RowVersion == 0);
            this.DbSet.AddRange(inserts);

            await this.Storage.SaveChangesAsync();
        }

        public async Task Reschedule(WorkItem model)
        {
            var item = this.Find(model.IdentityId);

            item.Retries = WorkItem.WORKITEM_RETRIES; // so it reschedules only once!
            if (model.DueDate.HasValue)
            {
                item.DueDate = model.DueDate.Value;
            }

            this.Edit(item);
            await this.Storage.SaveChangesAsync();
        }
    }
}