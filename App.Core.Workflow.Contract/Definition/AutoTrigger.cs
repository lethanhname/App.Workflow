using System;

namespace App.Core.Workflow.Contract.Definition
{
    public class AutoTrigger
    {
        public string Trigger { get; set; }

        public DateTime? DueDate { get; set; }
    }
}