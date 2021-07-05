using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mailServicee
{
    public class CheckedServiceItem
    {
        public CheckedServiceItem(string par1, DateTime par2)
        {
            this.ServiceName = par1;
            this.MailSendDate = par2;
        }
        public string ServiceName { get; set; }
        public DateTime MailSendDate { get; set; }
    }
}
