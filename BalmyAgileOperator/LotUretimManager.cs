using BalmyAgilev1;
using System.Data.SqlClient;
using System.Windows.Forms;
using System;
using System.Data;

public class LotUretimManager
{
    private readonly DatabaseHelper dbHelper;

    public LotUretimManager(DatabaseHelper databaseHelper)
    {
        dbHelper = databaseHelper;
    }


    public void ExecuteAltIstasyonAta(DataGridView dgv_lotokut, string altIstasyonAdi, string sorumluKisiAdi, string createUser)
    {
        try
        {
            // DataTable oluştur ve DataGridView'dan verileri aktar
            DataTable lotDetailsTable = new DataTable();
            lotDetailsTable.Columns.Add("LotNumarasi", typeof(string));

            foreach (DataGridViewRow row in dgv_lotokut.Rows)
            {
                if (row.Cells["LotNumarasi"].Value != null) // Eğer değer null değilse ekle
                {
                    lotDetailsTable.Rows.Add(row.Cells["LotNumarasi"].Value.ToString());
                }
            }

            // Eğer tablo boşsa kullanıcıyı uyar
            if (lotDetailsTable.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir lot numarası ekleyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // SQL parametrelerini tanımla
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@Altistasyonadi", altIstasyonAdi),
            new SqlParameter("@Sorumlukisiadi", sorumluKisiAdi),
            new SqlParameter("@CreateUser", createUser),
            new SqlParameter("@LotDetails", lotDetailsTable)
            };

            // Stored Procedure'ü çalıştır ve sonuç al
            object result = dbHelper.ExecuteScalar(5, "sp_altistasyonataV2c", parameters);

            // Sonucu değerlendir
            if (result != null)
            {
                string message = result.ToString();
                MessageBox.Show(message, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Alt istasyon ataması sırasında bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"SQL hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    // sp_IstasyonAtaByLotNumarasi fonksiyonu
    public void IstasyonAtaByLotNumarasi(string lotNumarasi, string istasyonAdi, string altIstasyonAdi, string sorumluKisiAdi, string createUser)
    {
        try
        {
            SqlParameter[] parameters = new SqlParameter[]
            {

                new SqlParameter("@LotNumarasi", lotNumarasi),
                new SqlParameter("@IstasyonAdi", istasyonAdi),
                new SqlParameter("@altistasyonAdi", altIstasyonAdi),
                new SqlParameter("@SorumluKisiAdi", sorumluKisiAdi),
                new SqlParameter("@CreateUser", createUser)
            };


            // SQL'den dönen sonucu al
            object result = dbHelper.ExecuteScalar(5, "sp_IstasyonAtaByLotNumarasi", parameters);
           



            if (result != null)
            {
                //string message = result.ToString();
                //MessageBox.Show(message, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("İstasyon ataması sırasında bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"SQL hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public string GetUretimEmriIstasyonNo(string lotNumarasi, string istasyonAdi, string altIstasyonAdi)
    {
        try
        {
            string uretimEmriNo = string.Empty;

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@LotNumarasi", lotNumarasi),
            new SqlParameter("@IstasyonAdi", istasyonAdi),
            new SqlParameter("@altistasyonadi", altIstasyonAdi)
            };

            using (SqlDataReader reader = dbHelper.ExecuteReader("sp_getistasyonemrino", parameters))
            {
                if (reader.Read())
                {
                    uretimEmriNo = reader["UretimEmriIstasyonNo"].ToString();
                }
            }

            return uretimEmriNo;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    public string UpdateLotBitisTarihiByLotNumarasi(string lotNumarasi)
    {
        try
        {
            // Parametreleri hazırla
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("@LotNumarasi", SqlDbType.NVarChar) { Value = lotNumarasi }
            };

            // Stored Procedure'ü çalıştır ve sonucu al
            DataTable resultTable = dbHelper.ExecuteStoredProcedure(5, "sp_UpdateLotTakipBitisTarihiByLotNumarasi", parameters);

            // Sonuç kontrolü
            if (resultTable.Rows.Count > 0)
            {
                return resultTable.Rows[0]["Mesaj"].ToString();
            }
            else
            {
                return "Beklenmeyen bir hata oluştu!";
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            return $"Yetkilendirme hatası: {ex.Message}";
        }
        catch (SqlException ex)
        {
            return $"SQL hatası: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Beklenmeyen bir hata oluştu: {ex.Message}";
        }
    }

// sp_CreateOrUpdateLotUretbyLotno fonksiyonu
public void CreateOrUpdateLotUretByLotno(string lotNumarasi, string istasyonAdi, string altIstasyonAdi, string sorumluKisiAdi, decimal miktar, string createUser)
    {
        try
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@LotNumarasi", lotNumarasi),
            new SqlParameter("@IstasyonAdi", istasyonAdi),
            new SqlParameter("@altistasyonadi", altIstasyonAdi),
            new SqlParameter("@sorumlukisiadi", sorumluKisiAdi),
            new SqlParameter("@Miktar", miktar),
            new SqlParameter("@CreateUser", createUser)
            };

            dbHelper.ExecuteNonQuery(5, "sp_CreateOrUpdateLotUretbyLotno", parameters);
            //MessageBox.Show("Lot üretim bilgileri başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

}
