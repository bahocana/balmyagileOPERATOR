using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zuby.ADGV;

namespace BalmyAgilev1
{
    public partial class istasyonForm : Form
    {
        private DatabaseHelper dbHelper;
        private DataGridViewSearchHelper _searchHelper;
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

        String connectionstring = "Server=10.41.17.2\\WINWIN_SQL;Initial Catalog=Balmy_Agile; Persist Security Info=True;User ID=sa;Password=SI&wrErItoVe";
        int currentUserID = Form1.UserID; // LoginForm'dan UserID al
        int currentRoleID = Form1.RoleID; // LoginForm'dan RoleID al
        public istasyonForm()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper(connectionstring);
            LoadUserInfo();
        }
        private void LoadUserInfo()
        {
   


        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void LoadAltistasyonlar(string istasyontipi)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@istasyontipi", SqlDbType.NVarChar) { Value = istasyontipi },
         
            };
            // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
            DataTable result = dbHelper.ExecuteStoredProcedure(currentUserID, "sp_listAltistasyonlar", parameters);

            // Sonuçları ekrana yazdır
            dvg_istasyonlar.DataSource = result;
        }

        public void AddRowToDataGridView(DataGridViewRow row)
        {
            // Eğer DataGridView'in kolonları yoksa ekleme
            if (dvg_uretimistasyonplanlamalist.ColumnCount == 0)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    dvg_uretimistasyonplanlamalist.Columns.Add(cell.OwningColumn.Name, cell.OwningColumn.HeaderText);
                }
            }

            // Yeni satır ekle
            int rowIndex = dvg_uretimistasyonplanlamalist.Rows.Add();
            DataGridViewRow newRow = dvg_uretimistasyonplanlamalist.Rows[rowIndex];

            // Verileri kopyala
            for (int i = 0; i < row.Cells.Count; i++)
            {
                newRow.Cells[i].Value = row.Cells[i].Value;
            }
        }
        private void LoadSorumlular()
        {
            try
            {
                // Stored Procedure çalıştırarak DataTable al
                DataTable stoklar = dbHelper.ExecuteStoredProcedure(currentUserID, "sp_listSorumlukisi");

                // DataGridView'e verileri bağla
                dvg_sorumlular.DataSource = stoklar;

                // DataGridView sütun ayarlarını düzenle
                dvg_sorumlular.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stok listesi yüklenirken hata oluştu: {ex.Message}");
            }
        }

        private void UretimEmriTasiForm()
        {
            try
            {
                // Form1 örneğini bul
                Form1 sourceForm = Application.OpenForms["Form1"] as Form1;

                if (sourceForm == null)
                {
                    MessageBox.Show("Form1 açık değil!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                // Kaynak DataGridView'de satır olup olmadığını kontrol et
                if (sourceForm.dvg_uretimistasyonplanlamalist.Rows.Count>0)
                {
                    // İlk satırı al
                    DataGridViewRow firstRow = sourceForm.dvg_uretimistasyonplanlamalist.Rows[0];

                    // Hedef DataGridView'deki sütunları temizle
                    dvg_uretimemribilgi.Columns.Clear();
                    dvg_uretimemribilgi.DataSource = null;

                    // Hedef DataGridView'deki sütunları yeniden oluştur
                    if (dvg_uretimemribilgi.Columns.Count == 0)
                    {
                        foreach (DataGridViewColumn column in sourceForm.dvg_uretimistasyonplanlamalist.Columns)
                        {
                            dvg_uretimemribilgi.Columns.Add((DataGridViewColumn)column.Clone());
                        }
                    }

                    // Yeni bir satır oluştur ve verileri kopyala
                    DataGridViewRow newRow = new DataGridViewRow();
                    foreach (DataGridViewCell cell in firstRow.Cells)
                    {
                        DataGridViewCell newCell = (DataGridViewCell)cell.Clone();
                        newCell.Value = cell.Value;
                        newRow.Cells.Add(newCell);
                    }

                    // Hedef DataGridView'e ekle
                    dvg_uretimemribilgi.Rows.Add(newRow);
                    dvg_uretimemribilgi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }
                else
                {
                    MessageBox.Show("Kaynak DataGridView'de satır yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void istasyon_Load(object sender, EventArgs e)
        {
            LoadSorumlular();
            LoadAltistasyonlar("");
            UretimEmriTasiForm();
            dvg_uretimemribilgi.Visible = false;
        }

        private void dvg_istasyonlar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Çift tıklanan hücrenin satır ve sütun indekslerini kontrol et
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Çift tıklanan hücrenin değerini al
                    string cellValue = dvg_istasyonlar.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                    // Hücredeki değeri TextBox'a ata
                    txt_altistasyonata.Text = cellValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dvg_sorumlular_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Çift tıklanan hücrenin satır ve sütun indekslerini kontrol et
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Çift tıklanan hücrenin değerini al
                    string cellValue = dvg_sorumlular.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                    // Hücredeki değeri TextBox'a ata
                    txt_sorumluata.Text = cellValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task istasyonata(string alturetimemrino,string altistasyonadi,string sorumlu,string user)

        {

            // Veritabanı bağlantı dizesi
        
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Stored procedure için parametreler
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Alturetimno", SqlDbType.NVarChar) { Value = alturetimemrino },
                new SqlParameter("@Altistasyonadi", SqlDbType.NVarChar) { Value = altistasyonadi },
                new SqlParameter("@Sorumlukisiadi", SqlDbType.NVarChar) { Value = sorumlu },
                new SqlParameter("@createuse", SqlDbType.NVarChar) { Value = user }
            };

            try
            {
                // Stored procedure'ü çalıştır
                int rowsAffected = dbHelper.ExecuteNonQuery(currentUserID, "sp_altisyonata", parameters);

                // Sonuç bilgisi
                MessageBox.Show($"{rowsAffected} satır işlendi.");
            }
            catch (Exception ex)
            {
                // Hata mesajını yazdır
                MessageBox.Show($"Hata: {ex.Message}");
            }

        }
        private async Task GetUretimEmriDetay(string uretimemrino, string Alturetimno, AdvancedDataGridView dvg)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UretimEmriNo", SqlDbType.NVarChar) { Value = uretimemrino },
                new SqlParameter("@ALTUretimEmriNo", SqlDbType.NVarChar) { Value = Alturetimno },
            };
            // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
            DataTable result = dbHelper.ExecuteStoredProcedure(currentUserID, "sp_GetUretimEmriDetay", parameters);

            // Sonuçları ekrana yazdır
            dvg.DataSource = result;

        }

        private async void button11_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_altistasyonata.Text)|| string.IsNullOrWhiteSpace(txt_sorumluata.Text))
            {
                MessageBox.Show("Lütfen bir İstasyon ve Sorumlu Giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
               
            
            Form1 anaform = new Form1();
            string Alturetimerino = dvg_uretimistasyonplanlamalist.Rows[0].Cells["ALTUretimEmriNo"].Value?.ToString();
            await istasyonata(Alturetimerino, txt_altistasyonata.Text, txt_sorumluata.Text, "admin");

            dvg_uretimistasyonplanlamalist.Columns.Clear();
            anaform.dvg_alturetimemri.Columns.Clear();
            await GetUretimEmriDetay(txt_uretimemrino.Text, Alturetimerino, dvg_uretimistasyonplanlamalist);


                string UretimEmriIstasyon = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();

                await UpdateUEISTdurumguncelle(UretimEmriIstasyon, "ÜRETİM DEVAM");
                await UpdateUretimDurumuWithSP(txt_uretimemrino.Text, "ÜRETİM DEVAM");

                await UpdateUEISTbaslangictarih(UretimEmriIstasyon, DateTime.Now);


                await GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_uretimistasyonplanlamalist);
            }
        }

        private void istasyonForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 anaform = (Form1)Application.OpenForms["Form1"];
            //anaform.GetUretimEmriDetay(txt_uretimemrino.Text, "", dvg_uretimistasyonplanlamalist.Rows[0].Cells["Üretilecek_Ürünkodu"].Value.ToString(), Convert.ToInt32(dvg_uretimistasyonplanlamalist.Rows[0].Cells["PlananMiktar"].Value), anaform.dvg_alturetimemri);
            anaform.GetUretimEmriDetay(txt_uretimemrino.Text, dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimDepoTalepNo"].Value.ToString(), anaform.dvg_alturetimemri);
            string UretimEmriIstasyon = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();
            anaform.GetUretimistasyonEmriDetay(UretimEmriIstasyon, anaform.dvg_lotbarkod);


        }
        public async Task UpdateUEISTdurumguncelle(string UretimEmriIstasyon, string yeniDurum)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Parametreleri tanımlayın
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyon },
        new SqlParameter("@durum", SqlDbType.NVarChar) { Value = yeniDurum }
            };

            // Stored Procedure'ü çalıştır
            int rowsAffected = dbHelper.ExecuteNonQuery(currentUserID, "sp_UpdateUEISTdurumguncelle", parameters);
            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_uretimistasyonplanlamalist);

        }
        public async Task UpdateUretimDurumuWithSP(string uretimemrino, string yeniDurum)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Parametreleri tanımlayın
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UretimEmriNo", SqlDbType.NVarChar) { Value = uretimemrino },
        new SqlParameter("@UretimDurumu", SqlDbType.NVarChar) { Value = yeniDurum }
            };

            // Stored Procedure'ü çalıştır
            int rowsAffected = dbHelper.ExecuteNonQuery(currentUserID, "sp_UpdateUretimDurumu", parameters);

 
        }
        public async Task UpdateUEISTbaslangictarih(string UretimEmriIstasyon, DateTime BaslangicTarihi)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Parametreleri tanımlayın
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyon },
        new SqlParameter("@BaslangicTarihi", SqlDbType.DateTime) { Value = BaslangicTarihi }
            };

            // Stored Procedure'ü çalıştır
            int rowsAffected = dbHelper.ExecuteNonQuery(currentUserID, "sp_UpdateUEISTbaslangictarih", parameters);
            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_uretimistasyonplanlamalist);

        }
        public void UpdateUEISTbBitisTarihi(string UretimEmriIstasyon, DateTime bitistarih)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Parametreleri tanımlayın
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyon },
        new SqlParameter("@BitisTarihi", SqlDbType.DateTime) { Value = bitistarih }
            };

            // Stored Procedure'ü çalıştır
            int rowsAffected = dbHelper.ExecuteNonQuery(currentUserID, "sp_UpdateUEISTBitisTarihi", parameters);

            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_uretimistasyonplanlamalist);
        }

        public void UpdateUEISTUretilenmiktar(string UretimEmriIstasyon, int uretimmiktar)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Parametreleri tanımlayın
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyon },
        new SqlParameter("@uretimmiktar", SqlDbType.Int) { Value = uretimmiktar }
            };

            // Stored Procedure'ü çalıştır
            int rowsAffected = dbHelper.ExecuteNonQuery(currentUserID, "sp_UpdateUEISTUretilenmiktar", parameters);

            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_uretimistasyonplanlamalist);

        }
        public async Task GetUretimistasyonEmriDetay(string UretimEmriIstasyonNo, AdvancedDataGridView dvg)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);
            dvg.Columns.Clear();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyonNo },

            };
            // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
            DataTable result = dbHelper.ExecuteStoredProcedure(currentUserID, "sp_GetUretimistasyonEmriDetay", parameters);
            dvg.DataSource = result;
            // Sonuçları ekrana yazdır

        }

        private async void btn_uretimbasla_Click(object sender, EventArgs e)
        {
            string UretimEmriIstasyon = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();

            await UpdateUEISTdurumguncelle(UretimEmriIstasyon, "ÜRETİM DEVAM");
            await UpdateUretimDurumuWithSP(txt_uretimemrino.Text, "ÜRETİM DEVAM");

            await   UpdateUEISTbaslangictarih(UretimEmriIstasyon, DateTime.Now);

          
            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_uretimistasyonplanlamalist);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string UretimEmriIstasyon = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();
            //UpdateUEISTUretilenmiktar(UretimEmriIstasyon, Convert.ToInt32(txt_uretilenmiktar.Text));
        }

        private void btn_uretimbitir_Click(object sender, EventArgs e)
        {
            string UretimEmriIstasyon = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();

            UpdateUEISTdurumguncelle(UretimEmriIstasyon, "ÜRETİM BİTTİ");

            UpdateUEISTbBitisTarihi(UretimEmriIstasyon, DateTime.Now);


            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_uretimistasyonplanlamalist);

        }

        private void label4_Click(object sender, EventArgs e)
        {


        }

        private void dvg_istasyonlar_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Çift tıklanan hücrenin satır ve sütun indekslerini kontrol et
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Çift tıklanan hücrenin değerini al
                    string cellValue = dvg_istasyonlar.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                    // Hücredeki değeri TextBox'a ata
                    txt_altistasyonata.Text = cellValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dvg_sorumlular_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dvg_sorumlular_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Çift tıklanan hücrenin satır ve sütun indekslerini kontrol et
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Çift tıklanan hücrenin değerini al
                    string cellValue = dvg_sorumlular.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                    // Hücredeki değeri TextBox'a ata
                    txt_sorumluata.Text = cellValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
