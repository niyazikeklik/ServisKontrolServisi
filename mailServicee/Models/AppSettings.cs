using mailServicee.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace mailServicee
{
    public class AppSettings
    {
        private bool KontrolInt(string data, string configName)
        {

           if (!int.TryParse(data, out int result))
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Servis Kontrol Servisi";
                    eventLog.WriteEntry("Config dosyasında girilen değer integer değil. Prop Adı: " + configName, EventLogEntryType.Warning);
                    AuxiliaryMethods.BuServisiKapat();
                    return false;
                }
            }
            return true;
            
        }
        private bool KontrolString(string data, string configName)
        {
            if (string.IsNullOrEmpty(data))
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Servis Kontrol Servisi";
                    eventLog.WriteEntry("Config dosyasında girilen değer boş geçilemez. Prop Adı: " + configName, EventLogEntryType.Warning);
                    AuxiliaryMethods.BuServisiKapat();
                    return false;
                }
            }
            return true;
        }

        static AppSettings instance;
        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                    instance = new AppSettings();
                return instance;
            }
        }


        AppSettings()
        {
            if (KontrolString(ConfigurationManager.AppSettings["ServiceNames"], "ServiceNames"))
                this.ServiceNames = ConfigurationManager.AppSettings["ServiceNames"]?.Split(';').ToList();

            if (KontrolInt(ConfigurationManager.AppSettings["ProgramDonguSuresiSaniye"], "ProgramDonguSuresiSaniye"))
                this.ProgramDonguSuresiSaniye = int.Parse(ConfigurationManager.AppSettings["ProgramDonguSuresiSaniye"]);

            if (KontrolInt(ConfigurationManager.AppSettings["TekrarEpostaGondermeSuresiDakika"], "TekrarEpostaGondermeSuresiDakika"))
                this.TekrarEpostaGondermeSuresiDakika = int.Parse(ConfigurationManager.AppSettings["TekrarEpostaGondermeSuresiDakika"]);

            if (KontrolString(ConfigurationManager.AppSettings["SmtpHost"], "SmtpHost"))
                this.SmtpHost = ConfigurationManager.AppSettings["SmtpHost"];

            if (KontrolInt(ConfigurationManager.AppSettings["SmtpPort"], "SmtpPort"))
                this.SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);

            if (KontrolString(ConfigurationManager.AppSettings["SourceMail"], "SourceMail"))
                this.SourceMail = ConfigurationManager.AppSettings["SourceMail"];

            if (KontrolString(ConfigurationManager.AppSettings["Password"], "Password"))
                this.Password = ConfigurationManager.AppSettings["Password"];

            if (KontrolString(ConfigurationManager.AppSettings["SQLConnectionString"], "SQLConnectionString"))
                this.SQLConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];

            if (KontrolString(ConfigurationManager.AppSettings["JsonYolu"], "JsonYolu"))
            {
                var jsonStr = File.ReadAllText(ConfigurationManager.AppSettings["JsonYolu"]);
                if (!jsonStr.Contains("[") || !jsonStr.Contains("]"))
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Servis Kontrol Servisi";
                        eventLog.WriteEntry("Adresi girilen json dosyası boştu. Boş değer için [] giriniz. \n Girilen Json Yolu: " + ConfigurationManager.AppSettings["JsonYolu"], EventLogEntryType.Warning);
                        AuxiliaryMethods.BuServisiKapat();
                    }
                this.JsonYolu = ConfigurationManager.AppSettings["JsonYolu"];
            }

            if (KontrolString(ConfigurationManager.AppSettings["SendMails"], "SendMails"))
                this.SendMails = ConfigurationManager.AppSettings["SendMails"]?.Split(';').ToList();
            if (ConfigurationManager.AppSettings["IsLogCheckActive"] == "false")
                this.IsLogCheckActive = false;
            else this.IsLogCheckActive = true;

            if (ConfigurationManager.AppSettings["IsLogCheckActive"] == "false")
                this.IsLogCheckActive = false;
            else this.IsLogCheckActive = true;

            ;
        }
        public List<string> ServiceNames{ get; set;}
        public int ProgramDonguSuresiSaniye{ get; set;  }
        public int TekrarEpostaGondermeSuresiDakika {get; set;}
        public string SourceMail { get; set;}
        public string Password  { get; set;}
        public List<string> SendMails   {get; set;}
        public string SmtpHost  { get; set; }
        public string JsonYolu { get; set; }
        public string SQLConnectionString { get; set; }
        public int SmtpPort {  get; set; }
        public bool IsLogCheckActive { get; set; }
    }
}
