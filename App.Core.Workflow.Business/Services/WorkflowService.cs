using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Workflow.Contract.Definition;
using App.Core.Workflow.Contract.Execution;
using App.Core.Workflow.Contract.Items;
using App.Core.Workflow.Contract.Services;
using App.CoreLib.EF.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Core.Workflow.Business.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository repository;
        private readonly ILogger<WorkflowService> logger;
        private readonly IWorkflowDefinitionProvider workflowDefinitionProvider;

        private readonly IWorkflowEngineService engineService;

        public WorkflowService(
          IWorkflowRepository repository,
          ILogger<WorkflowService> logger,
          IWorkflowDefinitionProvider workflowDefinitionProvider,
          IWorkflowEngineService engineService
        )
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger;
            this.workflowDefinitionProvider = workflowDefinitionProvider;
            this.engineService = engineService;
        }
        public async Task<IEnumerable<TriggerResult>> TriggerAsync(IdentityEntity entity, string entityName, EntityState state)
        {
            var triggers = await GetTriggers(entity, entityName, state);
            var triggerResults = new List<TriggerResult>();
            foreach (var trigger in triggers)
            {
                var result = await engineService.TriggerAsync(trigger);
                triggerResults.Add(result);
            }
            return triggerResults;
        }

        private async Task<List<TriggerParam>> GetTriggers(
          IdentityEntity entity,
          string entityName,
          EntityState state
        )
        {
            var triggers = new List<TriggerParam>();
            var entityWorkflows = workflowDefinitionProvider.GetWorkflowDefinitions(entityName);
            foreach (var workflowDef in entityWorkflows)
            {
                var workflow = await this.repository.FindAsync(entity.IdentityId, workflowDef.WorkflowName);
                var triggerName = "";
                if (workflow == null)
                {
                    var startPoint = workflowDef.Transitions.FirstOrDefault(p => p.State == state.ToString());
                    if (startPoint == null)
                    {
                        throw new ArgumentNullException("Could not first stage to start.");
                    }
                    else
                    {
                        triggerName = startPoint.Trigger;
                        workflow = WorkflowItem.Create(entity.IdentityId, workflowDef.WorkflowName, startPoint.State);
                        this.repository.Add(workflow);
                    }
                }
                triggers.Add(new TriggerParam(workflow, triggerName, entity));
            }
            return triggers;
        }
    }
}