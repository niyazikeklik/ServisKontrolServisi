using mailServicee.Models;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace mailServicee.Init
{
    public static class ServiceStatusInit
    {
        public static void Init()
        {
            string hataMesajlari = "";
            var kontrolEdilmeyecekServisler = AuxiliaryMethods.KontrolEdilmeyecekServisler();
            var kontrolEdilecekServisler = AuxiliaryMethods.KontrolEdilecekServisler(kontrolEdilmeyecekServisler, AppSettings.Instance.ServiceNames);
            int count = 0;
            //SERVİSLERİN DURUMUNU KONTROL ETTİRİYORUM
            foreach (var item in kontrolEdilecekServisler)
            {
                try
                {
                    if (!AuxiliaryMethods.IsRunningService(item))
                    {
                        count++;
                        kontrolEdilmeyecekServisler.Add(new CheckedServiceItem(item.ServiceName, DateTime.Now));
                        hataMesajlari += "<h4>" + item.ServiceName + "</h4> Servis çalışmayı durdurdu, yeniden çalıştırma denendi ama başarılı olamadı. <br>";

                    }

                }
                catch (Exception ex)
                {
                    count++;
                    kontrolEdilmeyecekServisler.Add(new CheckedServiceItem(item.ServiceName, DateTime.Now));
                    hataMesajlari += "<h4>" + item.ServiceName + "</h4> Servis çalışmayı durdurdu, yeniden çalıştırırken hata ile karşılaşıldı." +
                        "<h4 style=\"display:inline\"> Hata Mesajı:</h4> " + ex.Message + "<br>";
                }
            }

            if (!string.IsNullOrEmpty(hataMesajlari))
            {

                string tumCalismayanServisler = "";
                kontrolEdilmeyecekServisler.ForEach(x => { tumCalismayanServisler += x.ServiceName + ", "; });

                AuxiliaryMethods.MailGonder(hataMesajlari + "<br> <h6> Toplam çalışmayan servis listesi: " + tumCalismayanServisler
                      + " - Toplam sayı: " + kontrolEdilmeyecekServisler.Count, count + " Servis Çalışmayı Durdurdu!");

                string stringJSON = JsonConvert.SerializeObject(kontrolEdilmeyecekServisler);
                System.IO.File.WriteAllText(AppSettings.Instance.JsonYolu, stringJSON);


            }
        }
    }
}
