using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Quartz;
using mailServicee.Models;
using mailServicee.Init;

namespace mailServicee
{
    class CheckRecordLog : IJob
    {



        /*
         "  -Database bağlantısı singleton olacak. *
            -Connection tek yere bağlı olacak. *
            -Service1 kısmındaki methodlar classlara taşınacak *
            -AppConfig ile işlemin çalışıp çalıştırılmaması kontrol edilecek*
         */

        /*
               SQL BAĞLANTI CÜMLESİ CONFİG DOSYASINDAN ÇEKİELCEK
              SELECT UPDATE SORGULARI BURAYA GELECEK **
              json yolunu configden al.
              jobs execute methodlarının içinden tek method çağır.

   */
        public Task Execute(IJobExecutionContext context)
        {
            RecordLogInit.Init();
            return Task.CompletedTask;
        }
    }


}
