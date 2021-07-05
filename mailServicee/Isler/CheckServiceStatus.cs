using mailServicee.Init;
using mailServicee.Models;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mailServicee
{
   public class CheckServiceStatus : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            ServiceStatusInit.Init();
            return Task.CompletedTask;
        }

    
    }
}
