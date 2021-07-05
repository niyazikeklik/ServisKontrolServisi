using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mailServicee.Models
{
    class DataBase
    {
        DateTime sonHataMailiZamani = DateTime.Now.AddHours(-1);
        string BaglantiAdresi = AppSettings.Instance.SQLConnectionString;
        SqlConnection baglanti;
        SqlCommand komut;
        string sutunAdlariForUpdateQuery;
        public string[] sutunAdlari = { "IsRecordLogOpen", "deneme1", "deneme2", "deneme3", "deneme4", "deneme5" };
        static DataBase instance;
        public static DataBase Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataBase();
                return instance;
            }
        }

 
        DataBase()
        {
            try
            {
                this.Baglanti = new SqlConnection(BaglantiAdresi);
                this.Baglanti.Open();
            }
            catch (Exception ex)
            {
                if ((DateTime.Now - sonHataMailiZamani).TotalHours >= 1)
                {
                    AuxiliaryMethods.LogGonder("Veritabanı bağlantısı açılamadı. \nHata Mesajı: " + ex.Message);
                    AuxiliaryMethods.MailGonder("Veritabanı bağlantısı açılamadı. \nHata Mesajı: " + ex.Message, "Servis Kontrol Servisi Database problem");
                    sonHataMailiZamani = DateTime.Now;
                }
            }

        }


        public SqlConnection Baglanti { get { return baglanti; } set { baglanti = value; } }
        public SqlCommand Komut { get { return komut; } set { komut = value; } }
      

        public string[] SutunAdlari
        {
            get { return sutunAdlari; }
        }
        public string SutunAdlariCumle
        {
            get
            {
                sutunAdlariForUpdateQuery = "";
                string cumle = "";
                foreach (var item in sutunAdlari)
                {
                    cumle += item + ","; // sutunadi,sutunadi2,sutunadi3,
                    sutunAdlariForUpdateQuery += (item + "=@" + item + ","); //sutunadi=@psutunadi,sutunadi=@psutunadi2,sutunadi=@psutunadi3,
                }
                cumle = cumle.Remove(cumle.Length - 1); // sondaki virgülü sil.
                SutunAdlariForUpdateQuery = sutunAdlariForUpdateQuery.Remove(sutunAdlariForUpdateQuery.Length - 1); // sondaki virgülü sil.
                return cumle;
            }
        }
        public string SutunAdlariForUpdateQuery { get { return sutunAdlariForUpdateQuery; } set { sutunAdlariForUpdateQuery = value; } }
        public List<int> Values
        {
            get
            {
                DataBase.Instance.Komut = new SqlCommand("SELECT COUNT(*) FROM Company Where CompanyId = 683", DataBase.Instance.Baglanti);
                int count = (Int32)DataBase.Instance.Komut.ExecuteScalar();
                if (count == 1)
                {
                    List<int> values = new List<int>();
                    string komutCumlesi = "SELECT " + SutunAdlariCumle + " FROM Company Where CompanyId = 683";
                    DataBase.Instance.Komut = new SqlCommand(komutCumlesi, DataBase.Instance.Baglanti);
                    SqlDataReader dr = DataBase.Instance.Komut.ExecuteReader();
                    while (dr.Read())
                    {

                        for (int i = 0; i < sutunAdlari.Length; i++)
                        {
                            int value = Convert.ToInt16(dr[i]);
                            values.Add(value);
                        }

                    }
                    dr.Close();
                    return values;
                }
                else if ((DateTime.Now - sonHataMailiZamani).TotalHours >= 1)
                {
                    AuxiliaryMethods.MailGonder("SQL Select sorgusunda bir problem var. Birden fazla veya hiç kayıt döndürmüyor. Dönen veri sayısı: " + count, "Servis Kontrol Servisi Database problem");
                    sonHataMailiZamani = DateTime.Now;
                }
                return null;
            }

        }
        public bool DataBaseUpdate()
        {
            try
            {
                using (SqlCommand command = DataBase.Instance.Baglanti.CreateCommand())
                {
                    command.CommandText = "UPDATE Company SET " + sutunAdlariForUpdateQuery + " Where CompanyId = 683";
                    for (int i = 0; i < sutunAdlari.Length; i++)
                        command.Parameters.AddWithValue("@" + sutunAdlari[i], 1);
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
           
                if ((DateTime.Now - sonHataMailiZamani).TotalHours >= 1)
                {
                    AuxiliaryMethods.LogGonder("Güncelleme işleminde hata. \nHata Mesajı: " + ex.Message);
                    AuxiliaryMethods.MailGonder("Güncelleme işleminde hata. \nHata Mesajı: " + ex.Message, "Servis kontrol servisisinde hata");
                    sonHataMailiZamani = DateTime.Now;
                }
                return false;
            }

        }

    }

}
