using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace mailServicee
{
    public partial class Service1 : ServiceBase
    {
      
        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            SchedulerHelper.SchedulerSetup();
            
        }
        //singleton
    

        protected override void OnStop()
        {
           // Zamanlayici.Stop();
        }

        protected override void OnContinue()
        {
            //Zamanlayici.Start();
        }
    }
}
