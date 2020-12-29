using App.Core.Workflow.Contract.Execution;
using App.CoreLib.Common;
using App.CoreLib.EF.Data.Entity;
using Newtonsoft.Json;

namespace App.Core.Workflow.Contract.Items
{
    public partial class WorkflowVariable : IdentityEntityBase
    {
        public string Type { get; set; }

        public string Content { get; set; }

        public string WorkflowName { get; set; }

        public int WorkflowId { get; set; }

        [JsonIgnore]
        public WorkflowItem Workflow { get; set; }

        public static WorkflowVariable Create(WorkflowItem workflow, WorkflowVariableBase variable)
        {
            return new WorkflowVariable
            {
                WorkflowId = workflow.IdentityId,
                Workflow = workflow,
                Type = KeyBuilder.ToKey(variable.GetType()),
                Content = JsonConvert.SerializeObject(variable)
            };
        }

        public static WorkflowVariableBase ConvertContent(WorkflowVariable workflowVariable)
        {
            return (WorkflowVariableBase)JsonConvert.DeserializeObject(
              workflowVariable.Content,
              KeyBuilder.FromKey(workflowVariable.Type)
            );
        }

        internal void UpdateContent(WorkflowVariableBase variable)
        {
            this.Content = JsonConvert.SerializeObject(variable);
        }
    }
}