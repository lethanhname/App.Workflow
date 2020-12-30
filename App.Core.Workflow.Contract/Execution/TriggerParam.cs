using System;
using System.Collections.Generic;
using App.Core.Workflow.Contract.Items;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Workflow.Contract.Execution
{
    public class TriggerParam
    {
        public IWorkflow Workflow { get; private set; }
        public string TriggerName { get; private set; }
        public IdentityEntity Instance { get; private set; }
        public Dictionary<string, WorkflowVariableBase> Variables { get; private set; }
        public bool HasVariables
        {
            get
            {
                return Variables.Count > 0;
            }
        }

        public TriggerParam(
            IWorkflow workflow,
          string triggerName,
          IdentityEntity instance = null,
          Dictionary<string, WorkflowVariableBase> variables = null)
        {
            Workflow = workflow;
            TriggerName = triggerName;
            Instance = instance;
            Variables = variables != null
              ? variables
              : new Dictionary<string, WorkflowVariableBase>();
        }

        public TriggerParam AddVariable(string key, WorkflowVariableBase value)
        {
            if (Variables.ContainsKey(key))
            {
                throw new InvalidOperationException($"Key {key} exists already!");
            }

            Variables.Add(key, value);

            return this;
        }
    }
}