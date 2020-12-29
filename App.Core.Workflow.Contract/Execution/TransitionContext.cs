using System;
using System.Collections.Generic;
using App.Core.Workflow.Contract.Definition;
using App.Core.Workflow.Contract.Items;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Workflow.Contract.Execution
{
    public class TransitionContext
    {
        private Dictionary<string, WorkflowVariableBase> _variables { get; set; }
        private List<string> _errors;

        public bool HasVariables
        {
            get { return _variables.Count > 0; }
        }

        public bool TransitionAborted { get; private set; }

        public IdentityEntity Instance { get; private set; }
        public IWorkflow Workflow { get; private set; }

        public IEnumerable<string> Errors
        {
            get { return _errors; }
        }

        public bool HasErrors
        {
            get { return _errors.Count > 0; }
        }

        public TransitionContext(IWorkflow workflow, IdentityEntity instance)
        {
            Workflow = workflow;
            Instance = instance;
            _variables = new Dictionary<string, WorkflowVariableBase>();
            _errors = new List<string>();
        }

        public T GetInstance<T>() where T : IEntity
        {
            return (T)Instance;
        }

        public T GetWorkflow<T>() where T : IWorkflow
        {
            return (T)Workflow;
        }

        public bool ContainsKey(string key)
        {
            if (!HasVariables) return false;

            return _variables.ContainsKey(key);
        }

        public void SetVariable<T>(string key, T value) where T : WorkflowVariableBase
        {
            if (_variables.ContainsKey(key))
            {
                _variables[key] = value;
            }
            else
            {
                _variables.Add(key, value);
            }
        }

        public T GetVariable<T>(string key) where T : WorkflowVariableBase
        {
            if (!_variables.ContainsKey(key))
                throw new Exception(string.Format("Key '{0}' not found!", key));

            return (T)_variables[key];
        }

        public void AddError(string message)
        {
            _errors.Add(message);
        }

        public void AbortTransition(string reason)
        {
            TransitionAborted = true;
            AddError(reason);
        }
    }
}