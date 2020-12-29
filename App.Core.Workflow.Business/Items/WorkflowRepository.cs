using System.Threading.Tasks;
using App.Core.Workflow.Contract.Items;
using App.CoreLib.EF;
using App.CoreLib.EF.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace App.Core.Workflow.Business.Items
{
    public class WorkflowRepository : Repository<WorkflowItem>, IWorkflowRepository
    {
        public WorkflowRepository(IStorage context) : base(context)
        {
        }

        public Task<WorkflowItem> FindAsync(int entityId, string workflowName)
        {
            return DbSet.FirstOrDefaultAsync(p => p.EntityId == entityId && p.WorkflowName == workflowName);
        }
    }
}