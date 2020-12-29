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

namespace App.Core.Workflow.Web
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<WorkflowHandler>();
            EventHandlerContainer.Subscribe<AfterSaveEvent, WorkflowHandler>();
        }

        public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {

        }
    }
}
