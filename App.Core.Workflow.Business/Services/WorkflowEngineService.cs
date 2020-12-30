using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Workflow.Business.Execution;
using App.Core.Workflow.Contract.Definition;
using App.Core.Workflow.Contract.Execution;
using App.Core.Workflow.Contract.Items;
using App.Core.Workflow.Contract.Scheduler;
using App.Core.Workflow.Contract.Services;
using App.CoreLib;
using App.CoreLib.Common;
using App.CoreLib.EF.Data.Entity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace App.Core.Workflow.Business.Services
{
    public class WorkflowEngineService : IWorkflowEngineService
    {
        private readonly IWorkflowRepository repository;

        private readonly IWorkItemRepository wIRepository;
        private readonly ILogger<WorkflowEngineService> logger;
        private readonly IWorkflowDefinitionProvider workflowDefinitionProvider;

        public WorkflowEngineService(
          IWorkflowRepository repository,
          IWorkItemRepository wIRepository,
          ILogger<WorkflowEngineService> logger,
          IWorkflowDefinitionProvider workflowDefinitionProvider
        )
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.wIRepository = wIRepository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger;
            this.workflowDefinitionProvider = workflowDefinitionProvider;
        }

        public async Task<TriggerResult> CanTriggerAsync(TriggerParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            var execution = this.GetExecution(param.Workflow.WorkflowName);

            return await Task.FromResult(execution.CanTrigger(param));
        }

        public async Task<IEnumerable<TriggerResult>> GetTriggersAsync(
          IWorkflow workflow,
          IdentityEntity instance,
          Dictionary<string, WorkflowVariableBase> variables = null
        )
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var execution = this.GetExecution(workflow.WorkflowName);

            return await Task.FromResult(execution.GetTriggers(workflow, instance, variables));
        }

        public async Task<TriggerResult> TriggerAsync(TriggerParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            this.logger.LogTrace(
              "Trigger transition for instance {Instance}",
              LogHelper.SerializeObject(param.Workflow)
            );

            var result = await this.TriggerForPersistingInstance(param);


            if (result.IsAborted)
            {
                this.logger.LogInformation(
                  "Transition for instance {Instance} aborted! Aborting reason: {Reason}",
                  LogHelper.SerializeObject(param.Workflow),
                  LogHelper.SerializeObject(result.Errors)
                );
            }
            else
            {
                this.logger.LogTrace(
                  "Transition for instance {Instance} successfully triggered",
                  LogHelper.SerializeObject(param.Instance)
                );
            }

            return result;
        }

        private WorkflowExecution GetExecution(string workflowName)
        {
            var definition = this.workflowDefinitionProvider.GetWorkflowDefinition(workflowName);

            return new WorkflowExecution(definition);
        }

        private TriggerResult TriggerForNonPersistingInstance(TriggerParam param)
        {
            var execution = this.GetExecution(param.Workflow.WorkflowName);

            return execution.Trigger(param);
        }

        private async Task<TriggerResult> TriggerForPersistingInstance(TriggerParam param)
        {
            TriggerResult result;

            using (var transaction = this.repository.BeginTransaction())
            {
                try
                {
                    WorkflowItem workflow = (WorkflowItem)param.Workflow;
                    var execution = this.GetExecution(param.Workflow.WorkflowName);

                    this.EnsureWorkflowVariables(workflow, param);

                    result = execution.Trigger(param);
                    if (!result.IsAborted)
                    {
                        await this.PersistWorkflow(workflow, param, result);

                        await this.repository.SaveChangesAsync();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    this.logger.LogError(
                      "Trigger with TriggerParameter: {TriggerParameter} failed! {Exception}",
                      LogHelper.SerializeObject(param),
                      ex
                    );

                    var transitionContext = new TransitionContext(param.Workflow, param.Instance);
                    transitionContext.AddError(ex.ToString());

                    result = new TriggerResult(
                      param.TriggerName,
                      transitionContext,
                      false
                    );
                }
            }

            return result;
        }



        private void EnsureWorkflowVariables(WorkflowItem workflow, TriggerParam param)
        {
            if (workflow.WorkflowVariables.Count == 0) return;

            foreach (var workflowVariable in workflow.WorkflowVariables)
            {
                var variable = WorkflowVariable.ConvertContent(workflowVariable);
                if (variable is WorkflowVariableBase)
                {
                    var key = workflowVariable.Type;
                    if (param.Variables.ContainsKey(key))
                    {
                        param.Variables[key] = variable as WorkflowVariableBase;
                    }
                    else
                    {
                        param.Variables.Add(key, variable as WorkflowVariableBase);
                    }
                }
            }
        }

        private async Task PersistWorkflow(
          WorkflowItem workflow,
          TriggerParam param,
          TriggerResult result
        )
        {
            if (workflow == null) throw new ArgumentNullException(nameof(workflow));

            if (param.HasVariables)
            {
                foreach (var v in param.Variables)
                {
                    var variable = workflow.WorkflowVariables
                      .FirstOrDefault(variables => variables.Type == v.Key);

                    if (variable != null)
                    {
                        variable.Content = JsonConvert.SerializeObject(v.Value);
                    }
                    else
                    {
                        workflow.AddVariable(v.Value);
                    }
                }
            }

            workflow.WorkflowName = param.Workflow.WorkflowName;
            workflow.Assignee = param.Workflow.Assignee;

            workflow.AddHistoryItem(workflow.State, param.Workflow.State, param.Workflow.Assignee, Globals.CurrentUser);
            workflow.State = param.Workflow.State;

            if (result.HasAutoTrigger)
            {
                this.wIRepository.AddAutoTrigger(result.AutoTrigger, workflow);
            }

            if (await this.WorkflowIsCompleted(param))
            {
                workflow.Completed = Globals.Now();
            }
        }

        private async Task<bool> WorkflowIsCompleted(TriggerParam triggerParam)
        {
            var triggerResults
              = await this.GetTriggersAsync(triggerParam.Workflow, triggerParam.Instance, triggerParam.Variables);

            return triggerResults.All(r => !r.CanTrigger);
        }

        public async Task<IWorkflow> FindAsync(int entityId, string workflowName)
        {
            var wf = await this.repository.FindAsync(entityId, workflowName);
            return (IWorkflow)wf;
        }
    }
}