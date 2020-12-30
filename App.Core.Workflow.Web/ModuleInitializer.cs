using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using App.CoreLib.Module;
using App.Core.Workflow.Contract;
using App.Core.Workflow.Business;
using System;
using App.Core.Workflow.Contract.Items;
using App.Core.Workflow.Business.Items;
using App.Core.Workflow.Business.Events;
using App.CoreLib.EF.Events;
using App.Core.Workflow.Contract.Services;
using App.Core.Workflow.Business.Services;
using App.Core.Workflow.Contract.Definition;
using App.Core.Workflow.Business.Definition;

namespace App.Core.Workflow.Web
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<IWorkflowDefinitionProvider, WorkflowDefinitionProvider>();
            services.AddScoped<IWorkflowEngineService, WorkflowEngineService>();
            services.AddScoped<IWorkflowService, WorkflowService>();

            services.AddScoped<WorkflowHandler>();
            EventHandlerContainer.Subscribe<AfterSaveEvent, WorkflowHandler>();
        }

        public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {

        }
    }
}
