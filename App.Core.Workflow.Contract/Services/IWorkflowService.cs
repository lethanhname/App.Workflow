using System.Collections.Generic;
using System.Threading.Tasks;
using App.Core.Workflow.Contract.Execution;
using App.CoreLib.EF.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace App.Core.Workflow.Contract.Services
{
    public interface IWorkflowService
    {
        Task<IEnumerable<TriggerResult>> TriggerAsync(IdentityEntity entity, string entityName, EntityState state);
    }
}