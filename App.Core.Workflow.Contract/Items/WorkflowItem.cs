using System;
using System.Collections.Generic;
using System.Linq;
using App.Core.Workflow.Contract.Execution;
using App.CoreLib;
using App.CoreLib.Common;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Workflow.Contract.Items
{
    public class WorkflowItem : IdentityEntityBase, IWorkflow
    {
        public string WorkflowName { get; set; }

        public string State { get; set; }

        public int EntityId { get; set; }

        public string Assignee { get; set; }

        public DateTime Started { get; set; }

        public DateTime? Completed { get; set; }

        public List<WorkflowVariable> WorkflowVariables { get; set; } = new List<WorkflowVariable>();

        public List<WorkflowHistory> WorkflowHistories { get; set; } = new List<WorkflowHistory>();

        public static WorkflowItem Create(
          int entityId,
          string workflowName,
          string state
        )
        {
            return new WorkflowItem
            {
                Started = Globals.Now(),
                EntityId = entityId,
                WorkflowName = workflowName,
                State = state
            };
        }

        public void AddVariable(WorkflowVariableBase variable)
        {
            var type = KeyBuilder.ToKey(variable.GetType());
            var existing = this.WorkflowVariables.FirstOrDefault(v => v.Type == type);
            if (existing != null)
            {
                existing.UpdateContent(variable);

                return;
            }

            this.WorkflowVariables.Add(WorkflowVariable.Create(this, variable));
        }

        public void AddHistoryItem(string fromState, string toState, string assignee, string userName)
        {
            this.WorkflowHistories.Add(new WorkflowHistory
            {
                Created = Globals.Now(),
                FromState = fromState,
                ToState = toState,
                Assignee = assignee,
                UserName = userName,
                WorkflowId = this.IdentityId,
                Workflow = this
            });
        }
    }
}