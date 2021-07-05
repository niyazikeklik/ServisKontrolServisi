using mailServicee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mailServicee.Init
{
    public class RecordLogInit
    {
        static public void Init()
        {
            var values = DataBase.Instance.Values;
            if (values.Contains(0))
            {
                bool result = DataBase.Instance.DataBaseUpdate();
                if (result)
                {
                    string mailIcerik = @"<table><tr><th>Sütun Adı</th> <th>Eski Değer</th><th>Yeni Değer</th></tr>";
                    for (int i = 0; i < values.Count; i++)
                    {
                        mailIcerik += @"<tr><td>" + DataBase.Instance.SutunAdlari[i] + @"</td> <td>" + values[i] + @"</td> <td>" + 1 + @"</td></tr>";
                    }
                    AuxiliaryMethods.MailGonder(mailIcerik + "</table>", "Database tablo güncellendi");
                }
            }
        }

    }
}
