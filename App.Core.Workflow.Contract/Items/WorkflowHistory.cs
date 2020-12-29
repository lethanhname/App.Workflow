using System;
using System.Text.Json.Serialization;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Workflow.Contract.Items
{
    public partial class WorkflowHistory : IdentityEntityBase
    {
        public DateTime Created { get; set; }

        public string FromState { get; set; }

        public string ToState { get; set; }

        public string Assignee { get; set; }

        public string UserName { get; set; }

        public int WorkflowId { get; set; }

        [JsonIgnore]
        public WorkflowItem Workflow { get; set; }
    }
}