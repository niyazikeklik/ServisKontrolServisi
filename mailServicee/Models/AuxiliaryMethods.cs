using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace mailServicee.Models
{
    public static class AuxiliaryMethods
    {
        public static void LogGonder(string mesaj)
        {
            mesaj =  mesaj.HTMLEtiketleriniTemizle();
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Servis Kontrol Servisi";
                eventLog.WriteEntry(mesaj, EventLogEntryType.Error);
            }
        }
        static public void BuServisiKapat(string mesaj ="")
        {
            MailGonder("Servis Kontrol servisi bir hata sebebiyle durdu! Sistem Log Kayıtlarını İnceleyiniz.<br>Hata Mesajı: " +mesaj, "serviceControl İsimli servis durdu");
            ServiceController controller = new ServiceController("serviceControl");
            if (controller != null)
            {
                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        public static bool IsRunningService(ServiceController service)
        {
            if (service.Status != ServiceControllerStatus.Running)
            {
                service.Start();
                // Zaman aşımı
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                if (service.Status != ServiceControllerStatus.Running) return false;
                return true;
            }
            return true;

        }
        public static List<CheckedServiceItem> ReadDataFromJsonFile()
        {
            try
            {
                var jsonStr = File.ReadAllText(AppSettings.Instance.JsonYolu);
                return JsonConvert.DeserializeObject<List<CheckedServiceItem>>(jsonStr);
            }
            catch (Exception ex)
            {
                LogGonder("HATA KODU: X11 ReadDataFromJsonFile METHODUNDA HATA MEYDANA GELDİ. SERVİS DURDURULDU \nHATA MESAJI: " + ex.Message);
                BuServisiKapat(ex.Message);
                return null;
            }

        }
        public static List<ServiceController> KontrolEdilecekServisler(List<CheckedServiceItem> kontrolEdilmeyecek, List<string> tamServisList)
        {
            try
            {
                var mailGonderilenServisler = kontrolEdilmeyecek.Select(x => x.ServiceName).ToList();
                var result = tamServisList.Except(mailGonderilenServisler).Select(x =>
                {
                    return new ServiceController(x);
                }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                LogGonder("HATA KODU: X12 KontrolEdilecekServisler METHODUNDA HATA MEYDANA GELDİ. SERVİS DURDURULDU \nHATA MESAJI:  " + ex.Message);
                BuServisiKapat(ex.Message);
                return null;
            }

        }
        public static List<CheckedServiceItem> KontrolEdilmeyecekServisler()
        {
            try
            {
                var mailGonderilenServisler = ReadDataFromJsonFile()
               .Where(x => (DateTime.Now - x.MailSendDate).TotalMinutes < AppSettings.Instance.TekrarEpostaGondermeSuresiDakika)
               .ToList();
                return mailGonderilenServisler;
            }
            catch (Exception ex)
            {
                LogGonder("HATA KODU: X13 KontrolEdilmeyecekServisler METHODUNDA HATA MEYDANA GELDİ. SERVİS DURDURULDU.\nKonumu: " + AppSettings.Instance.JsonYolu + "olan json belgesinin içinin boş olmadığına emin olunuz. Boş değer için [] giriniz.  \nHATA MESAJI: " + ex.Message);
                BuServisiKapat(ex.Message);
                return null;
            }


        }
       static string HTMLEtiketleriniTemizle(this string mesaj)
        {
            string yeniIcerik = "";
            for (int i = 0; i < mesaj.Length; i++)
            {
                if (mesaj[i] == '<')
                {
                    while (mesaj[i] != '>')
                    {
                        i++;
                    }
                    i++;
                }
                else yeniIcerik += mesaj[i];
            }
            return yeniIcerik;
        }
        public static void MailGonder(string icerik, string konu)
        {
            try
            {
                //dosyayaYaz("Mail gönderiliyor \n" + "Hata Mesajı:yok \n" + DateTime.Now + "\n\n------------------------------\n", "MailGonder");
                MailMessage mail = new MailMessage();
                {
                    NetworkCredential cred = new NetworkCredential(AppSettings.Instance.SourceMail, AppSettings.Instance.Password);
                    var mails = AppSettings.Instance.SendMails;
                    foreach (var item in mails)
                    {
                        mail.To.Add(item);
                    }
                    mail.Subject = konu;
                    mail.From = new MailAddress(AppSettings.Instance.SourceMail);
                    mail.Priority = MailPriority.High;
                    mail.IsBodyHtml = true;
                    mail.Body = icerik.Replace("'.'", Environment.MachineName);
                    SmtpClient smtp = new SmtpClient(AppSettings.Instance.SmtpHost, AppSettings.Instance.SmtpPort);
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Credentials = cred;
                    smtp.Send(mail);
                }

            }
            catch (Exception ex)
            {
                LogGonder("İçeriği:\n \"" + icerik.Replace("<br>", "\n") + "\n olan hata bilgilendirme maili gönderilemedi. \nMail gönderirken karşılaşılan hata mesajı: " + ex.Message + "\nKaynak: " + ex.Source);
            }


        }
    }
}
