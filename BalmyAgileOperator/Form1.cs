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
using System.Drawing.Printing;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlTypes;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using QRCoder;
using System.Windows.Controls;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using static iText.Svg.SvgConstants;
using System.Net.Mail;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Reflection.Emit;



namespace BalmyAgilev1
{
    public partial class Form1 : Form
    {
        private DatabaseHelper dbHelper;
        private DataGridViewSearchHelper _searchHelper;
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

        private int rowIndex = 0; // Hangi satırdan başlandığını takip eder
        String connectionstring = "Server=10.41.17.2\\WINWIN_SQL;Initial Catalog=Balmy_Agile; Persist Security Info=True;User ID=sa;Password=SI&wrErItoVe";

        private int currentItemIndex = 0;
        private static string sessionID;
        private BarkodPrinter _barkodPrinter;
        public static int UserID { get; private set; } // Static UserID
        public static int RoleID { get; private set; } // Static RoleID
        public Form1()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper(connectionstring);

            _searchHelper = new DataGridViewSearchHelper(dvg_urunliste);
            UserID = 5;
            dvg_listuretimemri.RowPostPaint += dvg_listuretimemri_RowPostPaint;
            dvg_alturetimemri.RowPostPaint += dvg_alturetimemri_RowPostPaint_1;
            //dvg_uretimtalepmrp_RowPostPaint
            dvg_uretimtalepmrp.RowPostPaint += dvg_uretimtalepmrp_RowPostPaint;

            //printDocument.PrintPage += PrintDocument_PrintPage;
            // Form yüklendiğinde stok listesini yükleyin
            //LoadStokListesi();
            _barkodPrinter = new BarkodPrinter(list_lotbarkod,combo_printerlotbarkod); // ListBox'ınızı buraya ekleyin

            dvgsearch_stokgiris.SetColumns(dvg_stoklistgiris.Columns);

        }
        private void Login()
        {

            string username = txt_login.Text;
            string password = txt_pass.Text;

            LoginService loginService = new LoginService(connectionstring);

            // Login işlemi
            var (userID, roleID) = loginService.Login(username, password);

            if (userID != -1)
            {
                // Kullanıcı bilgilerini static değişkenlere atama
                UserID = userID; // int -> string dönüştürme
                RoleID = roleID;

                MessageBox.Show($"Giriş başarılı! UserID: {UserID}, RoleID: {RoleID}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Giriş başarılı olduğunda yapılacak işlemler
                // Örneğin, ana menüyü aç veya başka bir formu göster

                GoToTabPageana("AnaForm");
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadistasyonlarToDataGridView(5, dvg_istasyonsec, "sp_GetIstasyonListesi", "");
            timer1.Start();
            System.Windows.Forms.ComboBox comboBox1 = new System.Windows.Forms.ComboBox();
        
            // ComboBox'a metin öğeleri ekleme
            comboBox1.Items.Add("Sipariş");
            comboBox1.Items.Add("Stok");
            comboBox1.Items.Add("Seçenek 3");
        }
        public void LotBarkodPrinter()
        {


            // Print Document
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPageLOTBARKOD;

        }

        private void PrintDocument_PrintPageLOTBARKOD(object sender, PrintPageEventArgs e)
        {
            // Get the current LotBarkod
            if (currentItemIndex >= list_lotbarkod.Items.Count)
            {
                e.HasMorePages = false;
                return;
            }

            string lotBarkod = list_lotbarkod.Items[currentItemIndex].ToString();

            // Create QR Code
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(lotBarkod, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(5);

                    // Draw QR Code on Print Page
                    e.Graphics.DrawImage(qrCodeImage, 50, 50, 150, 150);

                    // Draw LotBarkod Text
                    e.Graphics.DrawString($"LotBarkod: {lotBarkod}", new Font("Arial", 12), Brushes.Black, new PointF(50, 220));
                }
            }

            // Check if there are more items to print
            currentItemIndex++;
            e.HasMorePages = currentItemIndex < list_lotbarkod.Items.Count;
        }
        public void LoadAltistasyonlarToDataGridView(int userId, DataGridView dataGridView, string spisim, string istasyonTipi)
        {
            try
            {
                // AdvancedDataGridView kontrolünü bulun
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // SQL parametresini oluştur
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@IstasyonTipi", SqlDbType.NVarChar) { Value = istasyonTipi }
                };

                // Stored Procedure çalıştır ve sonucu DataTable olarak al
                DataTable altistasyonlar = dbHelper.ExecuteStoredProcedure(userId, spisim, parameters);

                // DataGridView'a verileri bağla
                dataGridView.DataSource = altistasyonlar;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Yetki Hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadistasyonlarToDataGridView(int userId, DataGridView dataGridView, string spisim, string istasyonTipi)
        {
            try
            {
                // DataGridView'in otomatik sütun genişliği ayarını yap
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // Parametre olmadan Stored Procedure çalıştır
                DataTable altistasyonlar = dbHelper.ExecuteStoredProcedure(userId, spisim);

                // DataGridView'e verileri bağla
                dataGridView.DataSource = altistasyonlar;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Yetki Hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadOperatorlerToDataGridView(int userId, DataGridView dataGridView)
        {
            try
            {
                // AdvancedDataGridView kontrolünü bulun
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // sp_getAltistasyonlist prosedürünü çalıştır
                DataTable altistasyonlar = dbHelper.ExecuteStoredProcedure(userId, "sp_getOperatorlist");

                // DataGridView'a verileri bağla
                dataGridView.DataSource = altistasyonlar;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Yetki Hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void UpdateUretimMiktari()
        {
            try
            {
                DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);
                // Stored Procedure'ü çalıştır
                int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_UpdateUretimmiktariAll");


            }
            catch (Exception ex)
            {
                 MessageBox.Show($"Hata: {ex.Message}");
            }
        }
        private void listUretimemri()
        {
            try
            {
                // Stored Procedure çalıştırarak DataTable al
                DataTable uretimemri = dbHelper.ExecuteStoredProcedure(UserID, "sp_listuretimemri");

                // DataGridView'e verileri bağla
                dvg_listuretimemri.DataSource = uretimemri;

                // DataGridView sütun ayarlarını düzenle
                //dvg_listuretimemri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dvg_listuretimemri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stok listesi yüklenirken hata oluştu: {ex.Message}");
            }
        }

        private void LoadStokKartlariToDataGridView(int userId, string procedureName, DataGridView targetDataGridView)
        {
            try
            {
                // Stored Procedure çalıştırarak DataTable al
                DataTable stokKartlari = dbHelper.ExecuteStoredProcedure(userId, procedureName);
                // DataGridView için sütunları manuel olarak ayarla
                targetDataGridView.AutoGenerateColumns = false;
                targetDataGridView.Columns.Clear();

                targetDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "StokKodu", HeaderText = "StokKodu", DataPropertyName = "StokKodu" });
                targetDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "StokAdi", HeaderText = "StokAdi", DataPropertyName = "StokAdi" });
                targetDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "GrupKodu", HeaderText = "GrupKodu", DataPropertyName = "GrupKodu" });
                targetDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tur", HeaderText = "Tur", DataPropertyName = "Tur" });
                targetDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "AnaBirim", HeaderText = "AnaBirim", DataPropertyName = "AnaBirim" });

                // Verileri bağla
                targetDataGridView.DataSource = stokKartlari;
                // DataGridView'e verileri bağla
                targetDataGridView.DataSource = stokKartlari;

                // DataGridView sütun ayarlarını düzenle
                targetDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stok listesi yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadStokListesi()
        {
            try
            {
                // Stored Procedure çalıştırarak DataTable al
                DataTable stoklar = dbHelper.ExecuteStoredProcedure(UserID, "sp_StokListele");

                // DataGridView'e verileri bağla
                Dvg_Stoklist.DataSource = stoklar;

                // DataGridView sütun ayarlarını düzenle
                Dvg_Stoklist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stok listesi yüklenirken hata oluştu: {ex.Message}");
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            int yPos = e.MarginBounds.Top;
            int leftMargin = 10;

            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font dataFont = new Font("Arial", 10);
            Font gridFont = new Font("Arial", 9);

            try
            {
                // Veritabanı bağlantısı
                DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

                // Header bilgilerini çekmek için Stored Procedure çağrılır
                DataTable headerTable = dbHelper.ExecuteStoredProcedure(UserID, "sp_GetLatestUretimDepoTalepHeader");

                if (headerTable.Rows.Count > 0)
                {
                    DataRow headerRow = headerTable.Rows[0];

                    string talepNo = headerRow["UretimDepoTalepNo"].ToString();
                    string talepEden = headerRow["TalepEden"].ToString();
                    string createDateTime = Convert.ToDateTime(headerRow["CreateDateTime"]).ToString("dd/MM/yyyy");

                    // Üretim Talep Emri Bilgilerini Yazdır
                    e.Graphics.DrawString("ÜRETİM DEPO TALEP FORMU", headerFont, Brushes.Black, leftMargin, yPos);
                    yPos += 25;

                    e.Graphics.DrawString("Uretim Talep No: " + talepNo, dataFont, Brushes.Black, leftMargin, yPos);
                    yPos += 20;

                    e.Graphics.DrawString("Talep Eden: " + talepEden, dataFont, Brushes.Black, leftMargin, yPos);
                    yPos += 20;

                    e.Graphics.DrawString("Talep Tarihi: " + createDateTime, dataFont, Brushes.Black, leftMargin, yPos);
                    yPos += 40; // Ekstra boşluk bırak
                }
                else
                {
                    // Eğer header bilgisi bulunamazsa hata mesajı yazdır
                    e.Graphics.DrawString("Header bilgisi bulunamadı.", headerFont, Brushes.Red, leftMargin, yPos);
                    return;
                }

                // Line bilgilerini çekmek için Stored Procedure çağrılır
                DataTable lineTable = dbHelper.ExecuteStoredProcedure(UserID, "sp_GetLatestUretimDepoTalepLine");

                if (lineTable.Rows.Count > 0)
                {
                    // Başlıkları yazdır
                    e.Graphics.DrawString("Stok Kodu", gridFont, Brushes.Black, leftMargin, yPos);
                    e.Graphics.DrawString("Stok Açıklama", gridFont, Brushes.Black, leftMargin + 100, yPos);
                    e.Graphics.DrawString("Karsılama Türü", gridFont, Brushes.Black, leftMargin + 300, yPos);
                    e.Graphics.DrawString("Gereken Miktar", gridFont, Brushes.Black, leftMargin + 500, yPos);
                    e.Graphics.DrawString("Kalan Miktar", gridFont, Brushes.Black, leftMargin + 700, yPos);
                    yPos += 20;

                    // Satır verilerini yazdır
                    foreach (DataRow lineRow in lineTable.Rows)
                    {
                        e.Graphics.DrawString(lineRow["StokKodu"].ToString(), dataFont, Brushes.Black, leftMargin, yPos);
                        e.Graphics.DrawString(lineRow["StokAciklama"].ToString(), dataFont, Brushes.Black, leftMargin + 100, yPos);
                        e.Graphics.DrawString(lineRow["KarsilamaTuru"].ToString(), dataFont, Brushes.Black, leftMargin + 300, yPos);
                        e.Graphics.DrawString(lineRow["GerekenMiktar"].ToString(), dataFont, Brushes.Black, leftMargin + 500, yPos);
                        e.Graphics.DrawString(lineRow["KalanMiktar"].ToString(), dataFont, Brushes.Black, leftMargin + 700, yPos);

                        yPos += 20; // Her satırdan sonra boşluk bırak
                    }
                }
                else
                {
                    // Eğer line bilgisi bulunamazsa bilgi mesajı yazdır
                    e.Graphics.DrawString("Line bilgisi bulunamadı.", dataFont, Brushes.Red, leftMargin, yPos);
                }

                // Yazdırma işlemi tamamlandı
                e.HasMorePages = false;
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajını yazdır
                e.Graphics.DrawString("Yazdırma sırasında bir hata oluştu: " + ex.Message, dataFont, Brushes.Red, leftMargin, yPos);
            }
        }



        private void UretimEmriPlanlamaolustur()
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);


            // Stored procedure parametrelerini tanımla
            SqlParameter[] parameters = new SqlParameter[]
            {

                new SqlParameter("@UretimTipi", SqlDbType.NVarChar) { Value = combo_uretimtipi.Text},
                new SqlParameter("@SiparisNo", SqlDbType.NVarChar) { Value = txt_siparisno.Text },
                new SqlParameter("@Musteri", SqlDbType.NVarChar) { Value = txt_musteriadi.Text },
                new SqlParameter("@UretilecekUrunKodu", SqlDbType.NVarChar) { Value = txt_uretilecekurun.Text },
                new SqlParameter("@AsortiUrunuMu", SqlDbType.NVarChar) { Value = "Evet" },
                new SqlParameter("@SiparisAdeti", SqlDbType.Int) { Value = Convert.ToInt64(txt_siparisadet.Text)},
                new SqlParameter("@KoliIciAdeti", SqlDbType.Int) { Value = Convert.ToInt64(combo_koliiciadet.Text) },
                new SqlParameter("@KoliTipi", SqlDbType.NVarChar) { Value = combo_kolitipiseçiniz.Text },
                new SqlParameter("@PlanlananMiktar", SqlDbType.Int) { Value = Convert.ToInt64(txt_planlananadet.Text) },
                new SqlParameter("@UretilenMiktar", SqlDbType.Int) { Value = 0 },
                new SqlParameter("@SevkSekli", SqlDbType.NVarChar) { Value = "ARAÇ İLE" },
                new SqlParameter("@UretimBaslangicTarihi", SqlDbType.Date) { Value = date_uretimbaslangictarihi.Value },
                new SqlParameter("@TerminTarihi", SqlDbType.DateTime) { Value =  date_termintarihi.Value},
                new SqlParameter("@SevkTarihi", SqlDbType.DateTime) { Value = date_sevktarihi.Value },
                new SqlParameter("@UretimDurumu", SqlDbType.NVarChar) { Value = combo_uretimdurumu.Text },
                new SqlParameter("@MRPCalistir", SqlDbType.NVarChar) { Value = "Evet" },
                new SqlParameter("@CreateUser", SqlDbType.NVarChar) { Value = "admin" }
            };

            try
            {
                // Stored Procedure çağır
                object newID = dbHelper.ExecuteScalar(UserID, "sp_InsertUretimEmri", parameters);

                if (newID != null)
                {
                    MessageBox.Show("Üretim Planlama Emri Oluşturuldu. Üretim Planlama Emri No:" + newID.ToString());
                }
                else
                {
                    MessageBox.Show("Yeni üretim emri eklenirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                // Hata mesajı
                MessageBox.Show($"Hata: {ex.Message}");
            }

        }
        private void Mrpcalistir(string uretilecekurun, int uretilecekadet, string depoadi, AdvancedDataGridView dvg)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ustrecetekodu", SqlDbType.NVarChar) { Value = uretilecekurun },
                new SqlParameter("@UretilecekMiktar", SqlDbType.Decimal) { Value =  uretilecekadet },
                 new SqlParameter("@DepoAdi", SqlDbType.NVarChar) { Value =  depoadi }
            };
            // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
            DataTable result = dbHelper.ExecuteStoredProcedure(UserID, "sp_MRPCHECK", parameters);

            // Sonuçları ekrana yazdır
            dvg.DataSource = result;

        }


        private void CheckAndInsertAltUretimEmri(string uretimemrino)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Uretimplanlamaemrino", SqlDbType.NVarChar) { Value = uretimemrino },

            };
            // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_CheckAndInsertAltUretimEmri", parameters);



        }
        public async Task GetUretimEmriDetay(string uretimEmriNo, string UretimDepoTalepNo, AdvancedDataGridView dvg)
        {
            try
            {
                // Veritabanı bağlantısını başlat
                DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

                // Stored Procedure için parametreleri oluştur
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@UretimEmriNo", SqlDbType.NVarChar) { Value = uretimEmriNo ?? (object)DBNull.Value },
            new SqlParameter("@UretimDepoTalepNo", SqlDbType.NVarChar) { Value = UretimDepoTalepNo ?? (object)DBNull.Value },
                };

                // Stored Procedure çağır ve sonucu al
                DataTable result = await Task.Run(() => dbHelper.ExecuteStoredProcedure(UserID, "sp_GetUretimDepoTalepDurum", parameters));

                // Sonuçları AdvancedDataGridView'e aktar
                if (result != null && result.Rows.Count > 0)
                {
                    dvg.DataSource = result;
                }
                else
                {
                    MessageBox.Show("Kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Hataları kullanıcıya göster
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public async Task GetUretimEmriIstasyonDetailstotree(string UretimEmriIstasyonNo)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyonNo },

            };
            // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
            DataTable result = dbHelper.ExecuteStoredProcedure(UserID, "sp_GetUretimEmriIstasyonDetailstotree", parameters);

            // Sonuçları ekrana yazdır

            // Sonuç kontrolü
            if (result == null || result.Rows.Count == 0)
            {
                //MessageBox.Show("Yükleme işlemi başarısız. Herhangi bir veri bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemden çık
            }

            // Sonuçları ekrana yazdır
            LoadMainCategoriesToTreeView(result, tree_lotbarkod);
        }



        public async Task GetUretimistasyonEmriDetay(string UretimEmriIstasyonNo, AdvancedDataGridView dvg)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyonNo },

            };
            // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
            DataTable result = dbHelper.ExecuteStoredProcedure(UserID, "sp_GetUretimistasyonEmriDetay", parameters);
            dvg.DataSource = result;
            // Sonuçları ekrana yazdır

        }

        private void LoadStokListesibymamul()
        {
            try
            {
                // Stored Procedure çalıştırarak DataTable al
                DataTable stoklar = dbHelper.ExecuteStoredProcedure(UserID, "sp_StokListelebymamul");

                // DataGridView'e verileri bağla
                dvg_urunliste.DataSource = stoklar;

                // DataGridView sütun ayarlarını düzenle
                Dvg_Stoklist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stok listesi yüklenirken hata oluştu: {ex.Message}");
            }
        }
        private void txt_Fisno_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbl_uretimisemrino_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Tree_anamenu_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void l_Click(object sender, EventArgs e)
        {

        }



        private void dvg_listuretimemri_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridViewRow row = dvg_listuretimemri.Rows[e.RowIndex];

            // "UretimDurumu" kolonunu kontrol et
            string durum = row.Cells["UretimDurumu"].Value?.ToString();

            if (durum == "PLANLANDI")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            else if (durum == "ÜRETİM DEVAM")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightSkyBlue;
            }
            else if (durum == "DEPO TALEBİ OLUŞTU")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightPink;
            }
            else if (durum == "TAMAMLANDI")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightSeaGreen;
            }
            else if (durum == "İPTAL")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            }
        }


        private async void Page_takimduzenle_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check which TabPage is selected
            if (Page_ANAMENU.SelectedTab == Tabpage_UretimPlanlama)
            {
                // Load parameters into ComboBox for TabPage1
                LoadComboBox(combo_uretimtipi, new List<string> { "Siparis", "Stoklu" });
                LoadComboBox(combo_koliiciadet, new List<string> { "12", "15", "24" });
                LoadComboBox(combo_kolitipiseçiniz, new List<string> { "TEK DALGA-İÇ KOLİ", "ÇİFT DALGA-DIŞ KOLİ" });
                LoadComboBox(combo_uretimdurumu, new List<string> { "PLANLANDI" });
                LoadDepolarToComboBox(combo_mrpcalistirdepo);
            }
            if (Page_ANAMENU.SelectedTab == tabPage_mrp)
            {
                LoadDepolarToComboBox(combo_mrpcalistirdepo);
            }
            else if (Page_ANAMENU.SelectedTab == tabPage_uretimoperasyon)
            {
                listUretimemri();
                //LoadDepolarToComboBox(combo_uretimdepotalepdepo);
            }
            else if (Page_ANAMENU.SelectedTab == Tab_depoUretim)
            {
                LoadUretimDepoTalepHeaderToGrid(UserID, dvg_depouretimtaleplist);

            }
            else if (Page_ANAMENU.SelectedTab == tab_istasyonekrani)
            {
                //LoadAltistasyonlarToDataGridView(5, dvg_altistasyonsec, "sp_getAltistasyonlist");
                //LoadOperatorlerToDataGridView(5,dvg_operatorlist);
            }
            else if (Page_ANAMENU.SelectedTab == Page_StokGiris)
            {
                LoadDepolarToComboBox(combo_depogiris);
                LoadHareketNedenleriToComboBox(combo_hareketNedenleri);
                LoadComboBox(ComboDepo_Hareketi, new List<string> { "", "Giris", "Cikis" });
                LoadStokKartlariToDataGridView(UserID, "sp_ListStokKartlari", dvg_stoklistgiris);
                // AdvancedDataGridView ve SearchToolbar bağlantısı
               


            }
            else if (Page_ANAMENU.SelectedTab == tab_depoUretimdetay)
            {
                LoadDepolarToComboBox(combo_kaynakdepo);
                LoadDepolarToComboBox(combo_hedefdepo);

            }
            else if (Page_ANAMENU.SelectedTab == tabPage_lotbarkod)
            {
                tree_lotbarkod.Nodes.Clear();
                string UretimEmriIstasyon = dvg_lotbarkod.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();

                await GetUretimEmriIstasyonDetailstotree(UretimEmriIstasyon);
            }



        }

        private void GoToTabPageana(string tabPageName)
        {
            foreach (TabPage tab in Tab_anaform.TabPages)
            {
                if (tab.Text == tabPageName)
                {
                    Tab_anaform.SelectedTab = tab;
                    return;
                }
            }
            MessageBox.Show("TabPage not found.");
        }
        private void GoToTabPage(string tabPageName)
        {
            foreach (TabPage tab in Page_ANAMENU.TabPages)
            {
                if (tab.Text == tabPageName)
                {
                    Page_ANAMENU.SelectedTab = tab;
                    return;
                }
            }
            MessageBox.Show("TabPage not found.");
        }
        private void LoadComboBox(System.Windows.Forms.ComboBox comboBox, List<string> items)
        {
            // Clear existing items
            comboBox.Items.Clear();

            // Add new items
            foreach (var item in items)
            {
                comboBox.Items.Add(item);
            }

            // Optional: Set the first item as selected
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }



        private void dvg_deneme_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Tab_anaform_SelectedIndexChanged(object sender, EventArgs e)
        {
            GoToTabPage("istasyonekran");
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void combo_uretimtipi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_3(object sender, EventArgs e)
        {

        }

        private void label1_Click_4(object sender, EventArgs e)
        {

        }

        private void txt_planlananadet_TextChanged(object sender, EventArgs e)
        {

        }

        private void combo_kolitipiseçiniz_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbl_sevksekli_Click(object sender, EventArgs e)
        {

        }

        private void combo_sevksekli_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_5(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void txt_urunara_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_urunlistesi_Click(object sender, EventArgs e)
        {
            LoadStokListesibymamul();

            GoToTabPage("Ürün Liste");
        }
        private void SetRowVisibility(DataGridView dataGridView, int rowIndex, bool visible)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[dataGridView.DataSource];
            currencyManager.SuspendBinding(); // Bağlantıyı askıya al

            dataGridView.Rows[rowIndex].Visible = visible;

            currencyManager.ResumeBinding(); // Bağlantıyı devam ettir
        }
        private void txt_urunara_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void btn_urunlistesiara_Click(object sender, EventArgs e)
        {
            string searchText = txt_urunara.Text; // Arama metni
            string columnName = "StokKodu"; // İsteğe bağlı: Belirli bir sütun adı (örn: "Name")

            DataGridViewSearchHelper searchHelper = new DataGridViewSearchHelper(Dvg_Stoklist);
            searchHelper.Search(searchText, columnName);
        }

        private void dvg_urunliste_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0) // Geçerli bir satırda işlem yapıldığından emin olun
            {
                // Hedef kolonun değerini al (örneğin, "Name" kolonu)
                string selectedName = dvg_urunliste.Rows[e.RowIndex].Cells["StokKodu"].Value.ToString();

                txt_uretilecekurun.Text = selectedName;
                txt_urunkodu.Text = selectedName;
            }
            GoToTabPage("Üretim Planlama");
        }

        private void button39_Click(object sender, EventArgs e)
        {
            //LoadAltistasyonlarToDataGridView(5, dvg_altistasyonsec, "sp_getAltistasyonlist");
            //LoadOperatorlerToDataGridView(5, dvg_operatorlist);
            //GoToTabPageana("AnaForm");

            MessageBox.Show("LÜTFEN EKRANIN İSTASYON TİPİ Nİ SEÇİNİZ");
        }

        private void btn_mrpcalistir_Click(object sender, EventArgs e)
        {
         
            GoToTabPage("MRP");
        }


        private void btn_uretimplaniolustur_Click(object sender, EventArgs e)
        {
            UretimEmriPlanlamaolustur();
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
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_UpdateUretimDurumu", parameters);


        }
        public async Task UpdateUretimDurumuwithTalep(string uretimemrino, string yeniDurum)
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
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_UpdateUretimDurumu", parameters);


        }
        private void satirtasi(AdvancedDataGridView sourcedvg, AdvancedDataGridView targetdvg)
        {
            targetdvg.Columns.Clear();
            if (sourcedvg.SelectedRows.Count > 0)
            {

                targetdvg.Columns.Add("UretimEmriNo", "UretimEmriNo");
                targetdvg.Columns.Add("UretimTipi", "UretimTipi");
                targetdvg.Columns.Add("SiparisNo", "SiparisNo");
                targetdvg.Columns.Add("Musteri", "Musteri");
                targetdvg.Columns.Add("UretilecekUrunKodu", "UretilecekUrunKodu");
                targetdvg.Columns.Add("UretimDurumu", "UretimDurumu");
                //targetdvg.Columns.Add("AsortiUrunuMu", "AsortiUrunuMu");
                targetdvg.Columns.Add("SiparisAdeti", "SiparisAdeti");
                targetdvg.Columns.Add("PlanlananMiktar", "PlanlananMiktar");
                targetdvg.Columns.Add("UretilenMiktar", "UretilenMiktar");
                targetdvg.Columns.Add("UretimBaslangicTarihi", "UretimBaslangicTarihi");
                targetdvg.Columns.Add("SevkSekli", "SevkSekli");
                targetdvg.Columns.Add("TerminTarihi", "TerminTarihi");
                targetdvg.Columns.Add("SevkTarihi", "SevkTarihi");
                targetdvg.Columns.Add("KoliIciAdeti", "KoliIciAdeti");
                targetdvg.Columns.Add("KoliTipi", "KoliTipi");
                //targetdvg.Columns.Add("CreateUser", "CreateUser");


                // Seçilen satırı al
                DataGridViewRow selectedRow = dvg_listuretimemri.SelectedRows[0];

                // Hedef DataGridView'e ekle
                targetdvg.Rows.Add(
                    selectedRow.Cells["UretimEmriNo"].Value,
                    selectedRow.Cells["UretimTipi"].Value,
                    selectedRow.Cells["SiparisNo"].Value,
                    selectedRow.Cells["Musteri"].Value,
                    selectedRow.Cells["UretilecekUrunKodu"].Value,
                    //selectedRow.Cells["AsortiUrunuMu"].Value,
                    selectedRow.Cells["SiparisAdeti"].Value,
                    selectedRow.Cells["PlanlananMiktar"].Value,
                    selectedRow.Cells["UretilenMiktar"].Value,
                    selectedRow.Cells["SevkSekli"].Value,
                    selectedRow.Cells["UretimBaslangicTarihi"].Value,
                    selectedRow.Cells["TerminTarihi"].Value,
                    selectedRow.Cells["SevkTarihi"].Value,
                    selectedRow.Cells["UretimDurumu"].Value,
                    //selectedRow.Cells["CreateUser"].Value

                    selectedRow.Cells["KoliIciAdeti"].Value,
                    selectedRow.Cells["KoliTipi"].Value


                );



            }
        }


        private void btn_uretimtalebiolustur_Click(object sender, EventArgs e)
        {
            LoadDepolarToComboBox(Combo_üretimdepotalep);
            LoadDepolarToComboBox(combo_hammaddeDepoTalep);
            //if (string.IsNullOrWhiteSpace(combo_uretimdepotalepdepo.Text))
            //{
            //    MessageBox.Show("Lütfen Depo Seçiniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return; // İşlem durdurulur
            //}
            //satirtasi(dvg_listuretimemri, dvg_uretimtaleplist);
            uretimemritasi(dvg_listuretimemri, dvg_uretimtaleplist, "Üretim Depo Talep");
            //////////////try
            //////////////{
            //////////////    // DataGridView'deki ilk satır ve belirtilen kolon adına göre değer al
            //////////////    if (dvg_uretimtaleplist.Rows.Count > 0)
            //////////////    {

            //////////////        string uretilecekurun = dvg_uretimtaleplist.Rows[0].Cells["UretilecekUrunKodu"].Value?.ToString();

            //////////////        string Planlananmiktar = dvg_uretimtaleplist.Rows[0].Cells["PlanlananMiktar"].Value?.ToString();


            //////////////        Mrpcalistir(uretilecekurun, (int)Convert.ToInt64(Planlananmiktar), combo_uretimdepotalepdepo.Text, dvg_uretimtalepmrp);
            //////////////    }
            //////////////    else
            //////////////    {
            //////////////        MessageBox.Show("DataGridView'de satır yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //////////////    }
            //////////////}
            //////////////catch (Exception ex)
            //////////////{
            //////////////    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //////////////}

            //GoToTabPage("Üretim Depo Talep");


        }

        private async void button13_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(combo_hammaddeDepoTalep.Text) ||
          string.IsNullOrWhiteSpace(Combo_üretimdepotalep.Text))
            {
                MessageBox.Show("Hammadde Depo ve Üretim Depoyu seçmelisiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi sonlandır
            }


            string uretimemrino = dvg_uretimtaleplist.Rows[0].Cells["UretimEmriNo"].Value?.ToString();

            string uretilecekurun = dvg_uretimtaleplist.Rows[0].Cells["UretilecekUrunKodu"].Value?.ToString();

            int Planlananmiktar = Convert.ToInt32(dvg_uretimtaleplist.Rows[0].Cells["PlanlananMiktar"].Value?.ToString());

            await CreateUretimDepoTalepEmriAsync(
    talepEden: "Üretim Departmanı",
    kaynakdepo: combo_hammaddeDepoTalep.Text,
    talepEdilenDepo: Combo_üretimdepotalep.Text,
    uretimEmriNo: uretimemrino,
    ustReceteKodu: uretilecekurun,
    uretilecekMiktar: Planlananmiktar
);

            UpdateUretimDurumuWithSP(uretimemrino, "DEPO TALEBİ OLUŞTU");
            // Yazıcı ön izlemesini göster


            //printDocument.DefaultPageSettings.Landscape = true;
            //printPreviewDialog.Document = printDocument;
            //printPreviewDialog.ShowDialog();



        }

        private void btn_istasyonplanla_Click(object sender, EventArgs e)
        {
            if (dvg_listuretimemri.SelectedRows.Count > 0)
            {
                // Eğer DataGridView'de satır yoksa veya seçili satır yoksa uyarı ver
                if (dvg_listuretimemri.SelectedRows.Count == 0 || dvg_listuretimemri.CurrentRow == null)
                {
                    MessageBox.Show("Lütfen önce bir satır seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // Seçili satırdan "DepoTalepNo" değerini al
                string Depotaleno = dvg_listuretimemri.CurrentRow.Cells["DepoTalepNo"].Value?.ToString();

                // Eğer DepoTalepNo boş veya null ise uyarı ver
                if (string.IsNullOrWhiteSpace(Depotaleno))
                {
                    MessageBox.Show("Öncelikle Depo Talebi açmanız gerekiyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi sonlandır
                }
                uretimemritasi(dvg_listuretimemri, dvg_uretimistasyonplanlamalist, "İstasyon Planlama");

                string uretimemrino = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriNo"].Value?.ToString();
                string UretimDepoTalepNo = dvg_uretimistasyonplanlamalist.Rows[0].Cells["DepoTalepNo"].Value?.ToString();



                CheckAndInsertAltUretimEmri(uretimemrino);
                txt_uretimemrinoGLOBAL.Text = uretimemrino;

                GetUretimEmriDetay(uretimemrino, UretimDepoTalepNo,dvg_alturetimemri);
                dvg_alturetimemri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            }
            else
            {
                MessageBox.Show("Lütfen bir ÜRETİM EMRİ SEÇİN !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //GoToTabPage("İstasyon Planlama");
        }




            private void dvg_alturetimemri_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {


            try
            {
                // Seçili satır olup olmadığını kontrol et
                if (dvg_alturetimemri.SelectedRows.Count > 0)
                {
                    // Seçili satırı al
                    DataGridViewRow selectedRow = dvg_alturetimemri.SelectedRows[0];

                    // İlgili hücrenin değerini al
                    string selectedData = selectedRow.Cells["ALTUretimEmriNo"].Value.ToString();

                    // Yeni bir form oluştur ve seçili satırı gönder
                    istasyonForm secondForm = new istasyonForm();

                    // Seçilen satırı ikinci forma taşı
                    secondForm.AddRowToDataGridView(selectedRow);
                    string uretimemrino = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriNo"].Value?.ToString();


                    secondForm.txt_uretimemrino.Text = uretimemrino;
                    // İkinci formu göster
                    secondForm.Show();
                    UretimEmriTasiForm(dvg_uretimistasyonplanlamalist);

                    //UretimEmriTasiForm(dvg_uretimistasyonplanlamalist);
                }
                else
                {
                    MessageBox.Show("Lütfen bir satır seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void LoadMainCategoriesToTreeView(DataTable dataTable, System.Windows.Forms.TreeView treeView)
        {
            treeView.Nodes.Clear(); // Mevcut düğümleri temizle

            // Sadece ana kategorileri (ParentID == NULL) ekle
            foreach (DataRow row in dataTable.Rows)
            {

                TreeNode rootNode = new TreeNode(row["LotNumarasi"].ToString()); // 'Name' sütunu düğüm ismi için kullanılır
                                                                                 //rootNode.Tag = row["ID"]; // Düğümle ID değerini ilişkilendir
                treeView.Nodes.Add(rootNode);

            }
        }

        private void uretimemritasi2(AdvancedDataGridView sourcedvg, AdvancedDataGridView targetdvg, string menugit)
        {
            try
            {
                // Kaynak DataGridView'de en az bir satır olup olmadığını kontrol et
                if (sourcedvg.Rows.Count > 0)
                {
                    targetdvg.Columns.Clear();
                    targetdvg.DataSource = null;

                    // İlk satırı al (SelectedRows yerine Rows[0] kullanılıyor)
                    DataGridViewRow firstRow = sourcedvg.Rows[0];

                    // Hedef DataGridView'deki sütunların hazır olduğundan emin olun
                    if (targetdvg.Columns.Count == 0)
                    {
                        foreach (DataGridViewColumn column in sourcedvg.Columns)
                        {
                            targetdvg.Columns.Add((DataGridViewColumn)column.Clone());
                        }
                    }

                    // Yeni satır oluştur ve verileri kopyala
                    DataGridViewRow newRow = (DataGridViewRow)firstRow.Clone();
                    for (int i = 0; i < firstRow.Cells.Count; i++)
                    {
                        newRow.Cells[i].Value = firstRow.Cells[i].Value;
                    }

                    // Hedef DataGridView'e ekle
                    targetdvg.Rows.Add(newRow);
                    targetdvg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                    // Menü yönlendirme
                    if (string.IsNullOrWhiteSpace(menugit))
                    {
                        Console.WriteLine("String null, boş veya sadece boşluk içeriyor!");
                    }
                    else
                    {
                        GoToTabPage(menugit);
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen en az bir satır ekleyin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button11_Click(object sender, EventArgs e)
        {
           
            uretimemritasi2(dvg_uretimistasyonplanlamalist, dvg_lotbarkod_UEP, "");

            uretimemritasi(dvg_alturetimemri, dvg_lotbarkod, "Lot Barkod");


            string Alturetimerino = dvg_lotbarkod.Rows[0].Cells["ALTUretimEmriNo"].Value?.ToString();
            await istasyonata(Alturetimerino, lbl_altistasyonata.Text, lbl_operastorata.Text, "admin");
            //GoToTabPage("Lot Barkod");
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
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_UpdateUEISTdurumguncelle", parameters);


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
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_UpdateUEISTbaslangictarih", parameters);


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
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_UpdateUEISTBitisTarihi", parameters);


        }

        public async Task CreatLotUret(string UretimEmriIstasyon, int Cuvaliciadet, string user)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Parametreleri tanımlayın
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyon },
        new SqlParameter("@Miktar", SqlDbType.Int) { Value = Cuvaliciadet },
        new SqlParameter("@CreateUser", SqlDbType.NVarChar) { Value = user }
            };

            // Stored Procedure'ü çalıştır
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_CreateLotUret", parameters);


        }


        public async Task Deletelot(string UretimEmriIstasyon, string Lotnumarasi)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Parametreleri tanımlayın
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = UretimEmriIstasyon },
        new SqlParameter("@lotnumarasi", SqlDbType.Int) { Value = Lotnumarasi }
            };

            // Stored Procedure'ü çalıştır
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_DeleteLot", parameters);


        }
        public async Task DeleteLotsFromList(string uretimEmriIstasyon, System.Windows.Forms.ListBox list_lotbarkod)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // ListBox'taki her bir öğeyi işleyin
            foreach (var item in list_lotbarkod.Items)
            {
                try
                {
                    // Lot numarasını al
                    string lotnumarasi = item.ToString();

                    // Parametreleri tanımlayın
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = uretimEmriIstasyon },
                new SqlParameter("@lotnumarasi", SqlDbType.NVarChar) { Value = lotnumarasi }
                    };

                    // Stored Procedure'ü çalıştır
                    int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_DeleteLot", parameters);

                    // İşlem sonucu kontrol
                    if (rowsAffected > 0)
                    {
                       MessageBox.Show($"Lot {lotnumarasi} başarıyla silindi.");
                    }
                    else
                    {
                         MessageBox.Show($"Lot {lotnumarasi} silinirken bir sorun oluştu veya zaten mevcut değil.");
                    }
                }
                catch (Exception ex)
                {
                     MessageBox.Show($"Lot {item} silinirken hata oluştu: {ex.Message}");
                }
            }
        }


        private void btn_uretimbasla_Click(object sender, EventArgs e)
        {
            string UretimEmriIstasyon = dvg_lotbarkod.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();

            UpdateUEISTdurumguncelle(UretimEmriIstasyon, "ÜRETİM DEVAM");
            UpdateUretimDurumuWithSP(txt_uretimemrinoGLOBAL.Text, "ÜRETİM DEVAM");

            UpdateUEISTbaslangictarih(UretimEmriIstasyon, DateTime.Now);

            dvg_lotbarkod.Columns.Clear();
            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_lotbarkod);
            dvg_alturetimemri.Columns.Clear();
            GetUretimEmriDetay(txt_uretimemrinoGLOBAL.Text, dvg_lotbarkod.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString(), dvg_alturetimemri);
        }

        private async void button16_Click(object sender, EventArgs e)
        {
            string UretimEmriIstasyon = txt_globaluretimemriistasyon.Text;

            string Uretimdepotalepno = dvg_lotbarkod.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(UretimEmriIstasyon))
            {
                // Eğer değer boş veya sadece boşluk içeriyorsa uyarı ver
                MessageBox.Show("Lütfen öncelikle İstasyon atasayınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Kullanıcıyı alanı doldurması için odakla

            }
            else
            {
                // Eğer değer geçerli ise işlemler devam edebilir

                await CreatLotUret(UretimEmriIstasyon, (int)Convert.ToInt64(txt_cuvalmiktar.Text), "admin");

                await GetUretimEmriIstasyonDetailstotree(UretimEmriIstasyon);


                await UpdateUEISTdurumguncelle(UretimEmriIstasyon, "ÜRETİM DEVAM");
                await UpdateUretimDurumuWithSP(txt_uretimemrinoGLOBAL.Text, "ÜRETİM DEVAM");

                await UpdateUEISTbaslangictarih(UretimEmriIstasyon, DateTime.Now);

                dvg_lotbarkod.Columns.Clear();
                await GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_lotbarkod);
                dvg_alturetimemri.Columns.Clear();
                await GetUretimEmriDetay(txt_uretimemrinoGLOBAL.Text, Uretimdepotalepno, dvg_alturetimemri);
                //MessageBox.Show("İşlem başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }


        }

        public async Task ExecuteProcedureWithAuthorization(int userId)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UserID", SqlDbType.Int) { Value = userId }
            };

            try
            {
                DataTable result = dbHelper.ExecuteStoredProcedure(UserID, "sp_ExampleProcedure", parameters);
                 MessageBox.Show("İşlem başarılı.");
            }
            catch (SqlException ex)
            {
                 MessageBox.Show("Hata: " + ex.Message);
            }
        }

        public void GetCheckedNodes1(TreeNodeCollection nodes, System.Windows.Forms.ListBox listBox)
        {
            listBox.Items.Clear();
            foreach (System.Windows.Forms.TreeNode aNode in nodes)
            {
                //edit
                if (aNode.Checked)
                {

                    listBox.Items.Add(aNode.Text);
                }
                //if (aNode.Nodes.Count != 0)
                //    GetCheckedNodes(aNode.Nodes);
            }
        }
        private void tree_lotbarkod_AfterCheck(object sender, TreeViewEventArgs e)
        {
            GetCheckedNodes1(tree_lotbarkod.Nodes, list_lotbarkod);
        }

        private void BtnCreatePdf_Click(object sender, EventArgs e)
        {
            //BarkodPrinter printer = new BarkodPrinter(list_lotbarkod);
            //printer.PrintLotBarkodlari();
            //list_lotbarkod.Items.Clear();
            //RawPrinterHelper.SendStringToPrinter("4BARCODE 4B-2054K", "^XA^FO100,100^BQN,2,5^FDLA,1234567890^FS^XZ");

            // BarkodPrinter sınıfını oluştur
            BarkodPrinter printer = new BarkodPrinter(list_lotbarkod,combo_printerlotbarkod);

            // Parametreleri ayarla ve yazdırmayı başlat
            printer.PrintEtiket(
                 mamulKodu: dvg_lotbarkod_UEP.Rows[0].Cells["UretilecekUrunKodu"].Value?.ToString(),
                uretimEmriNo: dvg_lotbarkod_UEP.Rows[0].Cells["UretimEmriNo"].Value?.ToString(),
                altIstasyonAdi: dvg_lotbarkod.Rows[0].Cells["Altisyonadi"].Value?.ToString(),
                operatorAdi: dvg_lotbarkod.Rows[0].Cells["Sorumlukisiadi"].Value?.ToString(),
                tarih: DateTime.Now.ToString("dd.MM.yyyy HH:mm")
            );

        }

        private void button17_Click(object sender, EventArgs e)
        {
            UpdateUretimMiktari();
            dvg_alturetimemri.Columns.Clear();
            string Uretimdepotalepno = dvg_lotbarkod.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();
            GetUretimEmriDetay(txt_uretimemrinoGLOBAL.Text, Uretimdepotalepno, dvg_alturetimemri);
            GoToTabPage("İstasyon Planlama");
        }

        private void btn_uretimbitir_Click(object sender, EventArgs e)
        {
            string UretimEmriIstasyon = dvg_lotbarkod.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();

            UpdateUEISTdurumguncelle(UretimEmriIstasyon, "ÜRETİM BİTTİ");

            UpdateUEISTbBitisTarihi(UretimEmriIstasyon, DateTime.Now);

            dvg_lotbarkod.Columns.Clear();
            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_lotbarkod);
            dvg_alturetimemri.Columns.Clear();
            string Uretimdepotalepno = dvg_lotbarkod.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();
            GetUretimEmriDetay(txt_uretimemrinoGLOBAL.Text, Uretimdepotalepno, dvg_alturetimemri);
        }

        private void button18_Click(object sender, EventArgs e)
        {

        }

        private void tabPage_istasyonolustur_Click(object sender, EventArgs e)
        {

        }
        public void GetLastProcessForLotAndAddToDataGridView(string lotNumarasi, AdvancedDataGridView dataGridView)
        {
            try
            {
                // Eğer DataGridView'de sütun yoksa, gerekli sütunları ekle
                if (dataGridView.Columns.Count == 0)
                {
                    dataGridView.Columns.Add("LotNumarasi", "Lot Numarası");
                    dataGridView.Columns.Add("Surec", "Süreç");
                    dataGridView.Columns.Add("Fasonisim", "Fason İsim");
                    dataGridView.Columns.Add("YuklemeTarihi", "Yükleme Tarihi");
                }

                // DataGridView'de aynı LotNumarasi olup olmadığını kontrol et
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells["LotNumarasi"].Value != null && row.Cells["LotNumarasi"].Value.ToString() == lotNumarasi)
                    {
                        MessageBox.Show("Aynı Lot barkodunu tekrar ekleyemezsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // İşlemi durdur
                    }
                }

                // Stored Procedure parametrelerini oluştur
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@LotNumarasi", SqlDbType.NVarChar) { Value = lotNumarasi }
                };

                // Stored Procedure çağır ve sonucu al
                DataTable resultTable = dbHelper.ExecuteStoredProcedure(UserID, "sp_GetLastProcessForLot", parameters);

                if (resultTable.Rows.Count > 0)
                {
                    // İlk satırdaki verileri al
                    string lotNumber = resultTable.Rows[0]["LotNumarasi"].ToString();
                    string surec = resultTable.Rows[0]["Surec"].ToString();
                    string fasonisim = resultTable.Rows[0]["Fasonisim"].ToString();
                    string yuklemeTarihi = resultTable.Rows[0]["YuklemeTarihi"].ToString();

                    // DataGridView'e yeni bir satır ekle
                    dataGridView.Rows.Add(lotNumber, surec, fasonisim, yuklemeTarihi);
                }
                else
                {
                    MessageBox.Show("BARKOD BULUNAMADI !! LOT BARKODU ÜRETİN !!!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Hatası: " + sqlEx.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UretimEmriTasiForm(AdvancedDataGridView sourceDgv)
        {
            try
            {
                // Yeni bir form oluştur
                istasyonForm secondForm = new istasyonForm();

                // Kaynak DataGridView'de satır olup olmadığını kontrol et
                if (sourceDgv.Rows.Count > 0)
                {
                    // SecondForm'daki hedef DataGridView'i al


                    // Hedef DataGridView'deki sütunları temizle
                    secondForm.dvg_uretimemribilgi.Columns.Clear();
                    secondForm.dvg_uretimemribilgi.DataSource = null;

                    // İlk satırı al
                    DataGridViewRow firstRow = sourceDgv.Rows[0];

                    // Hedef DataGridView'deki sütunların hazır olduğundan emin olun
                    if (secondForm.dvg_uretimemribilgi.Columns.Count == 0)
                    {
                        foreach (DataGridViewColumn column in sourceDgv.Columns)
                        {
                            secondForm.dvg_uretimemribilgi.Columns.Add((DataGridViewColumn)column.Clone());
                        }
                    }

                    // Yeni bir satır oluştur ve verileri kopyala
                    DataGridViewRow newRow = (DataGridViewRow)firstRow.Clone();
                    for (int i = 0; i < firstRow.Cells.Count; i++)
                    {
                        newRow.Cells[i].Value = firstRow.Cells[i].Value;
                    }

                    // Hedef DataGridView'e ekle
                    secondForm.dvg_uretimemribilgi.Rows.Add(newRow);
                    secondForm.dvg_uretimemribilgi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                    //// İkinci formu göster
                    //secondForm.Show();
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


        private void uretimemritasi(AdvancedDataGridView sourcedvg, AdvancedDataGridView targetdvg, string menugit)
        {

            try
            {


                // Seçili satır olup olmadığını kontrol et
                if (sourcedvg.SelectedRows.Count > 0)
                {
                    targetdvg.Columns.Clear();
                    targetdvg.DataSource = null;

                    // Seçili satırı al
                    DataGridViewRow selectedRow = sourcedvg.SelectedRows[0];

                    // Hedef DataGridView'deki sütunların hazır olduğundan emin olun
                    if (targetdvg.Columns.Count == 0)
                    {
                        foreach (DataGridViewColumn column in sourcedvg.Columns)
                        {
                            targetdvg.Columns.Add((DataGridViewColumn)column.Clone());
                        }
                    }


                    DataGridViewRow newRow = (DataGridViewRow)selectedRow.Clone();
                    for (int i = 0; i < selectedRow.Cells.Count; i++)
                    {
                        newRow.Cells[i].Value = selectedRow.Cells[i].Value;
                    }

                    // Hedef DataGridView'e ekle
                    targetdvg.Rows.Add(newRow);
                    targetdvg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    GoToTabPage(menugit);
                }
                else
                {
                    MessageBox.Show("Lütfen bir satır seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void StokSec(AdvancedDataGridView sourcedvg, AdvancedDataGridView targetdvg, string menugit)
        {
            try
            {
                // Seçili satır olup olmadığını kontrol et
                if (sourcedvg.SelectedRows.Count > 0)
                {
                    // Hedef DataGridView'deki sütunların hazır olduğundan emin olun
                    EnsureColumnsExistAndEditable(targetdvg);

                    // Seçili satırı al
                    DataGridViewRow selectedRow = sourcedvg.SelectedRows[0];

                    // Yeni bir satır oluştur ve verileri ekle
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(targetdvg);

                    for (int i = 0; i < selectedRow.Cells.Count; i++)
                    {
                        newRow.Cells[i].Value = selectedRow.Cells[i].Value;
                    }

                    // LotNumarasi, SeriNumarasi ve Miktar sütunları için değer ekleme
                    int lotNumarasiIndex = targetdvg.Columns["LotNumarasi"].Index;
                    int seriNumarasiIndex = targetdvg.Columns["SeriNumarasi"].Index;
                    int miktarIndex = targetdvg.Columns["Miktar"].Index;

                    if (lotNumarasiIndex != -1) newRow.Cells[lotNumarasiIndex].Value = DBNull.Value;
                    if (seriNumarasiIndex != -1) newRow.Cells[seriNumarasiIndex].Value = DBNull.Value;
                    if (miktarIndex != -1) newRow.Cells[miktarIndex].Value = 0;

                    // Hedef DataGridView'e yeni satırı ekle
                    targetdvg.Rows.Add(newRow);
                    targetdvg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                    // Belirtilen sayfaya geç
                    GoToTabPage(menugit);
                }
                else
                {
                    MessageBox.Show("Lütfen bir satır seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Kolonları kontrol edip gerekirse ekler
        private void EnsureColumnsExistAndEditable(DataGridView targetGrid)
        {
            // LotNumarasi sütununu ekle
            if (!targetGrid.Columns.Contains("LotNumarasi"))
            {
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LotNumarasi",
                    HeaderText = "Lot Numarası",
                    DefaultCellStyle = { BackColor = Color.LightBlue },
                    ReadOnly = false
                });
            }

            // SeriNumarasi sütununu ekle
            if (!targetGrid.Columns.Contains("SeriNumarasi"))
            {
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SeriNumarasi",
                    HeaderText = "Seri Numarası",
                    DefaultCellStyle = { BackColor = Color.LightBlue },
                    ReadOnly = false
                });
            }

            // Miktar sütununu ekle
            if (!targetGrid.Columns.Contains("Miktar"))
            {
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Miktar",
                    HeaderText = "Miktar",
                    DefaultCellStyle = { BackColor = Color.LightBlue },
                    ReadOnly = false
                });
            }
        }




        private void btn_fasonyonetim_Click(object sender, EventArgs e)
        {




            GoToTabPage("Fason Yönetim");




        }

        private void button26_Click(object sender, EventArgs e)
        {

            GetLastProcessForLotAndAddToDataGridView(txt_lotbarkodoku.Text, dvg_fasonlotbarkod);

        }

        private void tree_fasonlotbarkod_AfterCheck(object sender, TreeViewEventArgs e)
        {

        }

        private void button27_Click(object sender, EventArgs e)
        {
            dvg_fasonlotbarkod.Rows.Clear();
        }
        public async Task ExecuteFasonislem(System.Windows.Forms.ListBox listBoxLotBarkod, string aracPlaka, string sofor, string surec, string isim)
        {
            try
            {
                // LineDetails için bir DataTable oluştur
                DataTable lineDetailsTable = new DataTable();
                lineDetailsTable.Columns.Add("Lotbarkod", typeof(string));

                // ListBox'taki tüm Lot Barkodlarını DataTable'a ekle
                foreach (var item in listBoxLotBarkod.Items)
                {
                    lineDetailsTable.Rows.Add(item.ToString());
                }

                // Stored Procedure parametrelerini ayarla
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@Aracplaka", SqlDbType.NVarChar) { Value = aracPlaka },
                new SqlParameter("@Sofor", SqlDbType.NVarChar) { Value = sofor },
                new SqlParameter("@Surec", SqlDbType.NVarChar) { Value = surec },
                new SqlParameter("@isim", SqlDbType.NVarChar) { Value = isim },
                new SqlParameter("@YuklemeTarihi", SqlDbType.DateTime) { Value = DateTime.Now },
                new SqlParameter("@LineDetails", SqlDbType.Structured)
                {
                    TypeName = "dbo.FasonAracYuklemeLineType", // SQL'deki tablo türü
                    Value = lineDetailsTable
                }
                };

                // Stored Procedure'ü çalıştır
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    using (SqlCommand command = new SqlCommand("sp_InsertFasonislem", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(parameters);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Fason işlem başarıyla tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ExecuteSpAltIstasyonAtaV2(System.Windows.Forms.ListBox list_fasonbarkod, string altIstasyonAdi, string sorumluKisiAdi, string createUser)
        {


        }


        public void ExecuteFasonIslemleri(
     string altIstasyonAdi,
     string sorumluKisiAdi,
     string createUser,
     string aracPlaka,
     string sofor,
     string surec,
     string isim,
     DateTime yuklemeTarihi,
     System.Windows.Forms.DataGridView dataGridView)
        {

            // İşlem sırasını tanımla
            Dictionary<string, int> surecSiralama = new Dictionary<string, int>
        {
            { "ARACAYUKLE", 1 },
            { "FASONATESLIMET", 2 },
            { "FASONDANTESLIMAL", 3 },
            { "FASONKABUL", 4 }
        };

            // İşlem sırasını kontrol et
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["Surec"].Value != null)
                {
                    string mevcutSurec = row.Cells["Surec"].Value.ToString();

                    if (surecSiralama.ContainsKey(mevcutSurec) && surecSiralama.ContainsKey(surec))
                    {
                        int mevcutSurecSirasi = surecSiralama[mevcutSurec];
                        int yapmakIstenenSurecSirasi = surecSiralama[surec];

                        // Eğer yapmak istenen işlem mevcut süreçten önceyse hata ver
                        if (yapmakIstenenSurecSirasi != mevcutSurecSirasi + 1)
                        {
                            string lotNumarasi = row.Cells["LotNumarasi"].Value?.ToString();
                            MessageBox.Show(
                                $"Lot Barkodu '{lotNumarasi}' için bu işlemi yapamazsınız. Mevcut süreç: {mevcutSurec}, Beklenen süreç: {GetSurecBySirasi(mevcutSurecSirasi + 1)}",
                                "Hata",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );

                            return;
                        }
                    }
                }
            }



            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();

                // Create a DataTable to represent LotDetails
                DataTable lotDetailsTable = new DataTable();
                lotDetailsTable.Columns.Add("LotNumarasi", typeof(string));

                // Populate DataTable with LotNumarasi column values from DataGridView
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells["LotNumarasi"].Value != null) // Ensure the value is not null
                    {
                        lotDetailsTable.Rows.Add(row.Cells["LotNumarasi"].Value.ToString());
                    }
                }

                // Check if there are any rows in the DataTable
                if (lotDetailsTable.Rows.Count == 0)
                {
                    MessageBox.Show("Lütfen en az bir lot numarası ekleyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Execute the first stored procedure: sp_altistasyonataV2c
                using (SqlCommand cmd = new SqlCommand("sp_altistasyonataV2c", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Altistasyonadi", altIstasyonAdi);
                    cmd.Parameters.AddWithValue("@Sorumlukisiadi", sorumluKisiAdi);
                    cmd.Parameters.AddWithValue("@CreateUser", createUser);
                    cmd.Parameters.AddWithValue("@LotDetails", lotDetailsTable);

                    cmd.ExecuteNonQuery();
                }

                // Execute the second stored procedure: sp_InsertFasonislemV2
                using (SqlCommand cmd = new SqlCommand("sp_InsertFasonislemV2", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Aracplaka", aracPlaka);
                    cmd.Parameters.AddWithValue("@Sofor", sofor);
                    cmd.Parameters.AddWithValue("@Surec", surec);
                    cmd.Parameters.AddWithValue("@isim", isim);
                    cmd.Parameters.AddWithValue("@YuklemeTarihi", yuklemeTarihi);
                    cmd.Parameters.AddWithValue("@LotDetails", lotDetailsTable);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Fason işlemleri başarıyla tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }







        public void ExecuteAltIstasyonata(System.Windows.Forms.ListBox list_fasonbarkod, string fasonIsim, string sorumluKisi, string createUser)
        {
            try
            {
                // Barkodları kontrol et
                if (list_fasonbarkod.Items.Count == 0)
                {
                    MessageBox.Show("Listede herhangi bir barkod bulunamadı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Barkodları DataTable'a dönüştür
                DataTable lotDetailsTable = new DataTable();
                lotDetailsTable.Columns.Add("LotNumarasi", typeof(string));

                foreach (var item in list_fasonbarkod.Items)
                {
                    lotDetailsTable.Rows.Add(item.ToString());
                }

                // Stored Procedure parametrelerini hazırla
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Fasonisim", SqlDbType.NVarChar) { Value = fasonIsim },
                new SqlParameter("@Sorumlukisi", SqlDbType.NVarChar) { Value = sorumluKisi },
                new SqlParameter("@CreateUser", SqlDbType.NVarChar) { Value = createUser },
                new SqlParameter("@LotDetails", SqlDbType.Structured)
                    {
                        TypeName = "dbo.LotDetailsType", // TVP (Table-Valued Parameter) tipi
                        Value = lotDetailsTable
                    }
                };

                // SP'yi çalıştır
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    using (SqlCommand command = new SqlCommand("sp_altistasyonataLOTDANV1", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(parameters);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                        MessageBox.Show("Lotlar başarıyla istasyonlara atandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task AssignLotsToStation(System.Windows.Forms.ListBox listBoxLotBarkod, string altUretimEmriNo, string fasonisim, string sorumlukisi, string createUser)
        {
            try
            {
                // LotDetails için bir DataTable oluştur
                DataTable lotDetailsTable = new DataTable();
                lotDetailsTable.Columns.Add("LotNumarasi", typeof(string));

                // ListBox'taki tüm Lot Barkodlarını DataTable'a ekle
                foreach (var item in listBoxLotBarkod.Items)
                {
                    lotDetailsTable.Rows.Add(item.ToString());
                }

                // Stored Procedure parametrelerini ayarla
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@AltUretimEmriNo", SqlDbType.NVarChar) { Value = altUretimEmriNo },
                new SqlParameter("@Fasonisim", SqlDbType.NVarChar) { Value = fasonisim },
                new SqlParameter("@Sorumlukisi", SqlDbType.NVarChar) { Value = sorumlukisi },
                new SqlParameter("@CreateUser", SqlDbType.NVarChar) { Value = createUser },
                new SqlParameter("@LotDetails", SqlDbType.Structured)
                {
                    TypeName = "dbo.LotDetailsType", // SQL'deki tablo türü
                    Value = lotDetailsTable
                }
                };

                // Stored Procedure'ü çalıştır
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    using (SqlCommand command = new SqlCommand("sp_altistasyonataLOTDAN", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(parameters);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Lot barkodları başarıyla istasyona atandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task istasyonata()
        {
            var formClosedCompletion = new TaskCompletionSource<bool>();
            try
            {
                // Seçili satır olup olmadığını kontrol et
                if (dvg_alturetimemri.SelectedRows.Count > 0)
                {
                    // Seçili satırı al
                    DataGridViewRow selectedRow = dvg_alturetimemri.SelectedRows[0];

                    // İlgili hücrenin değerini al
                    string selectedData = selectedRow.Cells["ALTUretimEmriNo"].Value.ToString();

                    // Yeni bir form oluştur ve seçili satırı gönder
                    var istasyonForm = new istasyonForm();
                    istasyonForm.FormClosed += (s, e) => formClosedCompletion.SetResult(true);

                    // Seçilen satırı ikinci forma taşı
                    istasyonForm.AddRowToDataGridView(selectedRow);
                    string uretimemrino = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriNo"].Value?.ToString();

                    istasyonForm.txt_uretimemrino.Text = uretimemrino;
                    // İkinci formu göster
                    istasyonForm.Show();
                }
                else
                {
                    MessageBox.Show("Lütfen bir satır seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            await formClosedCompletion.Task;
        }


        private async void button23_Click(object sender, EventArgs e)
        {




            //ExecuteSpAltIstasyonAtaV2(list_fasonbarkod, altIstasyonAdi, sorumluKisiAdi, createUser, aracplaka, sofor, surec, yuklemetarihi);


            ////dvg_fasonyonetimistasyon.Columns.Clear();
            //GetUretimEmriDetay(txt_uretimemrinoGLOBAL.Text, altUretimEmriNo, dvg_fasonyonetimistasyon);


            //await AssignLotsToStation(list_fasonbarkod, altUretimEmriNo, fasonisim, sorumlukisi, createUser);




            //UpdateUEISTdurumguncelle(UretimEmriIstasyonNo, "ARACAYUKLE");

        }

        private void btn_Fasonliste_Click(object sender, EventArgs e)
        {
            fasonlist fasonForm = new fasonlist(this); // Bu formu (Form1) referans olarak gönder
            fasonForm.ShowDialog(); // Fasonlist formunu modal olarak aç
        }

        private void button28_Click(object sender, EventArgs e)
        {

            try
            {
                // DataGridView'in ilk satırını kontrol et
                if (dvg_alturetimemri.Rows.Count > 0)
                {
                    // İlk satırı al
                    DataGridViewRow firstRow = dvg_lotbarkod.Rows[0];

                    // İlgili hücrenin değerini al
                    string selectedData = firstRow.Cells["ALTUretimEmriNo"].Value?.ToString();

                    // Yeni bir form oluştur ve ilk satırı gönder
                    istasyonForm secondForm = new istasyonForm();

                    // İlk satırı ikinci forma taşı
                    secondForm.AddRowToDataGridView(firstRow);

                    // Ana DataGridView'in ilk satırındaki "UretimEmriNo" değerini al
                    string uretimemrino = dvg_uretimistasyonplanlamalist.Rows[0].Cells["UretimEmriNo"].Value?.ToString();

                    // İkinci formun TextBox'ına bu değeri ata
                    secondForm.txt_uretimemrino.Text = uretimemrino;

                    // İkinci formu göster
                    secondForm.Show();
                }
                else
                {
                    MessageBox.Show("Listede herhangi bir satır yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void ExecuteFasonIslem(
      string aracplaka,
      string sofor,
      string surec,
      string isim,
      DateTime yuklemeTarihi,
      System.Windows.Forms.DataGridView dataGridViewLotNumbers)
        {

            // İşlem sırasını tanımla
            Dictionary<string, int> surecSiralama = new Dictionary<string, int>
        {
            { "ARACAYUKLE", 1 },
            { "FASONATESLIMET", 2 },
            { "FASONDANTESLIMAL", 3 },
            { "FASONKABUL", 4 }
        };

            // İşlem sırasını kontrol et
            foreach (DataGridViewRow row in dataGridViewLotNumbers.Rows)
            {
                if (row.Cells["Surec"].Value != null)
                {
                    string mevcutSurec = row.Cells["Surec"].Value.ToString();

                    if (surecSiralama.ContainsKey(mevcutSurec) && surecSiralama.ContainsKey(surec))
                    {
                        int mevcutSurecSirasi = surecSiralama[mevcutSurec];
                        int yapmakIstenenSurecSirasi = surecSiralama[surec];

                        // Eğer yapmak istenen işlem mevcut süreçten önceyse hata ver
                        if (yapmakIstenenSurecSirasi != mevcutSurecSirasi + 1)
                        {
                            string lotNumarasi = row.Cells["LotNumarasi"].Value?.ToString();
                            MessageBox.Show(
                                $"Lot Barkodu '{lotNumarasi}' için bu işlemi yapamazsınız. Mevcut süreç: {mevcutSurec}, Beklenen süreç: {GetSurecBySirasi(mevcutSurecSirasi + 1)}",
                                "Hata",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );

                            return;
                        }
                    }
                }
            }

            // Check if required fields are provided
            if (string.IsNullOrEmpty(aracplaka) || string.IsNullOrEmpty(sofor) || string.IsNullOrEmpty(isim))
            {
                MessageBox.Show("Araç plakası, şoför bilgisi ve isim boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create a DataTable to represent @LotDetails
            DataTable lotDetailsTable = new DataTable();
            lotDetailsTable.Columns.Add("LotNumarasi", typeof(string));

            // Add items from the DataGridView's "LotNumarasi" column to the DataTable
            foreach (DataGridViewRow row in dataGridViewLotNumbers.Rows)
            {
                if (row.Cells["LotNumarasi"].Value != null) // Ensure the cell is not null
                {
                    string lotNumarasi = row.Cells["LotNumarasi"].Value.ToString();
                    lotDetailsTable.Rows.Add(lotNumarasi);
                }
            }

            // Check if there are any lot numbers
            if (lotDetailsTable.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir lot numarası seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Open SQL connection
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();

                    // Create a SqlCommand to execute the stored procedure
                    using (SqlCommand command = new SqlCommand("sp_InsertFasonislemV2", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        command.Parameters.AddWithValue("@Aracplaka", aracplaka);
                        command.Parameters.AddWithValue("@Sofor", sofor);
                        command.Parameters.AddWithValue("@Surec", surec);
                        command.Parameters.AddWithValue("@isim", isim);
                        command.Parameters.AddWithValue("@YuklemeTarihi", yuklemeTarihi);

                        // Add the table-valued parameter
                        SqlParameter lotDetailsParameter = command.Parameters.AddWithValue("@LotDetails", lotDetailsTable);
                        lotDetailsParameter.SqlDbType = SqlDbType.Structured;
                        lotDetailsParameter.TypeName = "dbo.LotDetailsType";

                        // Execute the stored procedure
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Stored procedure executed successfully.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {

            if (dvg_fasonlotbarkod.Rows.Count == 0)
            {
                // Eğer satır yoksa uyarı mesajı göster
                MessageBox.Show("Fason lot barkod tablosunda işlem yapmak için en az bir satır ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }
            if (string.IsNullOrWhiteSpace(txt_soforbilgi.Text) ||
       string.IsNullOrWhiteSpace(txt_aracplaka.Text) ||
       string.IsNullOrWhiteSpace(txt_Fasonisim.Text))
            {
                // Uyarı mesajı göster
                MessageBox.Show("Şoför bilgisi, araç plakası veya fason isim bilgisi boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }


            string altIstasyonAdi = txt_Fasonisim.Text;
            string sorumluKisiAdi = txt_Fasonisim.Text;
            string createUser = "admin";
            string aracplaka = txt_aracplaka.Text;
            string sofor = txt_soforbilgi.Text;
            string surec = "FASONATESLIMET";
            string fasonisim = txt_Fasonisim.Text;
            DateTime yuklemetarihi = DateTime.Now;

            ExecuteFasonIslem(
    aracplaka,
    sofor,
    surec,
    fasonisim,
    yuklemetarihi,
    dvg_fasonlotbarkod // Your actual ListBox control
);
        }

        private async void button24_Click(object sender, EventArgs e)
        {

            if (dvg_fasonlotbarkod.Rows.Count == 0)
            {
                // Eğer satır yoksa uyarı mesajı göster
                MessageBox.Show("Fason lot barkod tablosunda işlem yapmak için en az bir satır ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }
            if (string.IsNullOrWhiteSpace(txt_soforbilgi.Text) ||
       string.IsNullOrWhiteSpace(txt_aracplaka.Text) ||
       string.IsNullOrWhiteSpace(txt_Fasonisim.Text))
            {
                // Uyarı mesajı göster
                MessageBox.Show("Şoför bilgisi, araç plakası veya fason isim bilgisi boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }


            string altIstasyonAdi = txt_Fasonisim.Text;
            string sorumluKisiAdi = txt_Fasonisim.Text;
            string createUser = "admin";
            string aracplaka = txt_aracplaka.Text;
            string sofor = txt_soforbilgi.Text;
            string surec = "FASONDANTESLIMAL";
            string fasonisim = txt_Fasonisim.Text;
            DateTime yuklemetarihi = DateTime.Now;

            ExecuteFasonIslem(
    aracplaka,
    sofor,
    surec,
    fasonisim,
    yuklemetarihi,
    dvg_fasonlotbarkod // Your actual ListBox control
);
        }

        private async void button25_Click(object sender, EventArgs e)
        {
            if (dvg_fasonlotbarkod.Rows.Count == 0)
            {
                // Eğer satır yoksa uyarı mesajı göster
                MessageBox.Show("Fason lot barkod tablosunda işlem yapmak için en az bir satır ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }
            if (string.IsNullOrWhiteSpace(txt_soforbilgi.Text) ||
       string.IsNullOrWhiteSpace(txt_aracplaka.Text) ||
       string.IsNullOrWhiteSpace(txt_Fasonisim.Text))
            {
                // Uyarı mesajı göster
                MessageBox.Show("Şoför bilgisi, araç plakası veya fason isim bilgisi boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }

            string altIstasyonAdi = txt_Fasonisim.Text;
            string sorumluKisiAdi = txt_Fasonisim.Text;
            string createUser = "admin";
            string aracplaka = txt_aracplaka.Text;
            string sofor = txt_soforbilgi.Text;
            string surec = "FASONKABUL";
            string fasonisim = txt_Fasonisim.Text;
            DateTime yuklemetarihi = DateTime.Now;

            ExecuteFasonIslem(
    aracplaka,
    sofor,
    surec,
    fasonisim,
    yuklemetarihi,
    dvg_fasonlotbarkod // Your actual ListBox control
);



        }

        private void dvg_alturetimemri_RowPostPaint_1(object sender, DataGridViewRowPostPaintEventArgs e)
        {

            DataGridViewRow row = dvg_alturetimemri.Rows[e.RowIndex];

            // "UretimDurumu" kolonunu kontrol et
            string durum = row.Cells["IstasyonDurumu"].Value?.ToString();

            if (durum == "PLANLANDI")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            else if (durum == "ÜRETİM DEVAM")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightSkyBlue;
            }
            else if (durum == "ARACAYUKLE")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCyan;
            }
            else if (durum == "FASONATESLIMET")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightSalmon;
            }
            else if (durum == "FASONDANTESLIMAL")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
            }
            else if (durum == "FASONKABUL")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightSeaGreen;
            }

            else if (durum == "ÜRETİM BİTTİ")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightSeaGreen;
            }
            else if (durum == "İPTAL")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
            }
        }

        private void btn_stoklist_Click(object sender, EventArgs e)
        {

        }
        public void settxt_Fasonisim(string value)
        {
            txt_Fasonisim.Text = value; // Form1'deki TextBox
        }

        public void setsoforbilgi(string value)
        {
            txt_soforbilgi.Text = value; // Form1'deki TextBox
        }
        private void button9_Click(object sender, EventArgs e)
        {
            sofor soforform = new sofor(this); // Bu formu (Form1) referans olarak gönder
            soforform.ShowDialog(); // Fasonlist formunu modal olarak aç
        }

        private void button10_Click(object sender, EventArgs e)
        {

            if (dvg_listuretimemri.SelectedRows.Count == 0 || dvg_listuretimemri.CurrentRow == null)
            {
                MessageBox.Show("Lütfen önce bir satır seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }

            // Seçili satırdan "DepoTalepNo" değerini al
            string Depotaleno = dvg_listuretimemri.CurrentRow.Cells["DepoTalepNo"].Value?.ToString();

            // Eğer DepoTalepNo boş veya null ise uyarı ver
            if (string.IsNullOrWhiteSpace(Depotaleno))
            {
                MessageBox.Show("Öncelikle Depo Talebi açmanız gerekiyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi sonlandır
            }

            GoToTabPage("Fason Yönetim");
        }






        // İşlem sırasına göre süreç ismini bulma
        private string GetSurecBySirasi(int sirasi)
        {
            switch (sirasi)
            {
                case 1: return "ARACAYUKLE";
                case 2: return "FASONATESLIMET";
                case 3: return "FASONDANTESLIMAL";
                case 4: return "FASONKABUL";
                default: return "Bilinmeyen Süreç";
            }
        }








        private void button20_Click(object sender, EventArgs e)
        {
            if (dvg_fasonlotbarkod.Rows.Count == 0)
            {
                // Eğer satır yoksa uyarı mesajı göster
                MessageBox.Show("Fason lot barkod tablosunda işlem yapmak için en az bir satır ekleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }
            if (string.IsNullOrWhiteSpace(txt_soforbilgi.Text) ||
       string.IsNullOrWhiteSpace(txt_aracplaka.Text))
            {
                // Uyarı mesajı göster
                MessageBox.Show("Şoför bilgisi veya araç plakası bilgisi boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi durdur
            }


            ExecuteFasonIslemleri(
        altIstasyonAdi: "FASONDAGITIM",
        sorumluKisiAdi: "FASONDAGITIM",
        createUser: "admin",
        aracPlaka: "34ABC123",
        sofor: "Akbilek",
        surec: "ARACAYUKLE",
        isim: "ARAC",
        yuklemeTarihi: DateTime.Now,
        dataGridView: dvg_fasonlotbarkod // Pass the ListBox control
    );
        }

        private void txt_lotbarkodoku_KeyPress(object sender, KeyPressEventArgs e)
        {


            if (e.KeyChar == (char)Keys.Enter)
            {
                string barkod = txt_lotbarkodoku.Text.Trim();

                if (!string.IsNullOrEmpty(barkod))
                {
                    // Barkod bilgisi işleniyor

                    GetLastProcessForLotAndAddToDataGridView(txt_lotbarkodoku.Text, dvg_fasonlotbarkod);
                    // İşlem tamamlandıktan sonra TextBox'ı temizle ve fokus yap
                    txt_lotbarkodoku.Clear();
                    txt_lotbarkodoku.Focus();
                }
            }
        }

        private void dvg_fasonlotbarkod_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Eğer tıklanan satır geçerliyse (örneğin sütun başlığına tıklanmadıysa)
                if (e.RowIndex >= 0)
                {
                    // Kullanıcıdan onay iste
                    DialogResult result = MessageBox.Show(
                        "Bu satırı silmek istediğinize emin misiniz?",
                        "Satır Sil",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        // Satırı kaldır
                        dvg_fasonlotbarkod.Rows.RemoveAt(e.RowIndex);


                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya bilgi ver
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            GoToTabPage("Üretim Planlama");
        }

        private void button22_Click(object sender, EventArgs e)
        {
            GoToTabPage("Üretim Planlama");
        }
        public class DataGridHelper
        {
            /// <summary>
            /// JSON yanıtını DataGridView'e doldurur.
            /// </summary>
            /// <param name="jsonResponse">JSON yanıtı</param>
            /// <param name="dataGridView">DataGridView nesnesi</param>
            public static void FillDataGridView(string jsonResponse, DataGridView dataGridView)
            {
                try
                {
                    // JSON verisini parse et
                    JObject json = JObject.Parse(jsonResponse);

                    // "result" alanını liste olarak al
                    JArray results = (JArray)json["result"];
                    if (results == null || results.Count == 0)
                    {
                        MessageBox.Show("Sonuç bulunamadı.");
                        return;
                    }

                    // DataTable oluştur ve sütun başlıklarını ekle
                    DataTable dataTable = new DataTable();

                    foreach (JProperty property in results[0])
                    {
                        dataTable.Columns.Add(property.Name, typeof(string));
                    }

                    // Satırları doldur
                    foreach (JObject item in results)
                    {
                        DataRow row = dataTable.NewRow();
                        foreach (JProperty property in item.Properties())
                        {
                            row[property.Name] = property.Value.ToString();
                        }
                        dataTable.Rows.Add(row);
                    }

                    // DataGridView'e bağla
                    dataGridView.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void btn_siparislistesi_Click(object sender, EventArgs e)
        {

            // Giriş isteği (Login)
            sessionID = ApiHelper.SendRequest("https://balmy.ws.dia.com.tr/api/v3/sis/json", @"
            {
                ""login"": {
                    ""username"": ""pixa"",
                    ""password"": ""241225"",
                    ""disconnect_same_user"": ""true"",
                    ""lang"": ""tr"",
                    ""params"": {
                        ""apikey"": ""634c363a-b8f4-4cbb-8677-78d76a73a5a6""
                    }
                }
            }");


            var apiRequest = new ApiRequestModel(sessionID);

            //Sipariş listeleme isteği
            string siparisRequestBody = apiRequest.SiparisListeledetail();
            string siparisresponse = ApiHelper.SendRequest(apiRequest.Url, siparisRequestBody);
            DataGridHelper.FillDataGridView(siparisresponse, dvg_siparisliste); // myDataGridView, formdaki DataGridView nesnesi

            GoToTabPage("Sipariş Liste");
        }

        private void dvg_uretimtalepmrp_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridViewRow row = dvg_uretimtalepmrp.Rows[e.RowIndex];

            // "UretimDurumu" kolonunu kontrol et
            string durum = row.Cells["Durum"].Value?.ToString();

            if (durum == "YETERLİ")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
            }
            else if (durum == "STOK YETERSİZ")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightPink;
            }

        }
        public void LoadHareketNedenleriToComboBox(System.Windows.Forms.ComboBox combo_hareketNedenleri)
        {
            try
            {
                // DatabaseHelper sınıfını kullanarak bağlantıyı oluştur
                DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

                // Stored Procedure'ü çalıştır ve sonuçları al
                DataTable resultTable = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetHareketNedenleri", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }

                // ComboBox'ı temizle ve varsayılan boş öğeyi ekle
                combo_hareketNedenleri.Items.Clear();
                combo_hareketNedenleri.Items.Add(string.Empty); // Varsayılan boş öğe

                // Sonuçlardan verileri yükle
                foreach (DataRow row in resultTable.Rows)
                {
                    combo_hareketNedenleri.Items.Add(row["HareketNedeni"].ToString());
                }

                // İlk öğeyi (boş öğe) seçili hale getir
                combo_hareketNedenleri.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DepoAdlarını ComboBox'a yükleyen fonksiyon
        public void LoadDepolarToComboBox(System.Windows.Forms.ComboBox combo_depolar)
        {
            try
            {
                // DatabaseHelper sınıfını kullanarak bağlantıyı oluştur
                DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

                // Depo sorgusu için SQL komutu
                string query = "SELECT [DepoAdi] FROM [Balmy_Agile].[dbo].[Depolar]";

                // SQL komutunu çalıştır ve sonuçları al
                DataTable resultTable = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }

                // ComboBox'ı temizle ve varsayılan boş öğeyi ekle
                combo_depolar.Items.Clear();
                combo_depolar.Items.Add(string.Empty); // Varsayılan boş öğe

                // Sonuçlardan verileri yükle
                foreach (DataRow row in resultTable.Rows)
                {
                    combo_depolar.Items.Add(row["DepoAdi"].ToString());
                }

                // İlk öğeyi (boş öğe) seçili hale getir
                //combo_depolar.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task CreateUretimDepoTalepEmriAsync(string talepEden,string kaynakdepo,string talepEdilenDepo, string uretimEmriNo, string ustReceteKodu, decimal uretilecekMiktar)
        {
            try
            {
                // DatabaseHelper sınıfını kullanarak bağlantıyı oluştur
                DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

                // Stored procedure için gerekli parametreleri hazırla
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@TalepEden", SqlDbType.NVarChar) { Value = talepEden },
            new SqlParameter("@KaynakDepo", SqlDbType.NVarChar) { Value = kaynakdepo },
            new SqlParameter("@TalepEdilenDepo", SqlDbType.NVarChar) { Value = talepEdilenDepo },
            new SqlParameter("@UretimEmriNo", SqlDbType.NVarChar) { Value = uretimEmriNo },
            new SqlParameter("@ustrecetekodu", SqlDbType.NVarChar) { Value = ustReceteKodu },
            new SqlParameter("@UretilecekMiktar", SqlDbType.Decimal) { Value = uretilecekMiktar }
                };

                // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
                DataTable result = dbHelper.ExecuteStoredProcedure(UserID, "sp_CreateUretimDepoTalepEmri", parameters);

                // Sonuçları kontrol et
                if (result.Rows.Count > 0)
                {
                     MessageBox.Show("Üretim Depo Talep Emri oluşturuldu.");

                    //foreach (DataRow row in result.Rows)
                    //{
                    //     MessageBox.Show($"Talep No: {row["UretimDepoTalepNo"]}, Stok Kodu: {row["StokKodu"]}, Durum: {row["Durum"]}");
                    //}
                }
                else
                {
                     MessageBox.Show("Üretim Depo Talep Emri oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }
        public async Task CreateUretimDepoTalepEmriAsync_EK(string talepEden, string kaynakdepo, string talepEdilenDepo, string uretimEmriNo, DataGridView dvg_uretimtalepmrp)
        {
            try
            {
                // DatabaseHelper örneği (veritabanı bağlantısı için)
                DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

                // User-Defined Table için DataTable oluştur
                DataTable mrpEktalepTable = new DataTable();
                mrpEktalepTable.Columns.Add("ReceteID", typeof(int));
                mrpEktalepTable.Columns.Add("StokKodu", typeof(string));
                mrpEktalepTable.Columns.Add("KarsilamaTuru", typeof(string));
                mrpEktalepTable.Columns.Add("ToplamUretilecekMiktar", typeof(decimal));
                mrpEktalepTable.Columns.Add("GerekenMiktar", typeof(decimal));
                mrpEktalepTable.Columns.Add("Birim", typeof(string));

                bool hasSelectedRows = false;

                // Seçili satırları kontrol et ve tabloya ekle
                foreach (DataGridViewRow row in dvg_uretimtalepmrp.SelectedRows)
                {
                    if (row.Cells["ReceteID"].Value != null)
                    {
                        hasSelectedRows = true; // Seçili satır var
                        mrpEktalepTable.Rows.Add(
                            Convert.ToInt32(row.Cells["ReceteID"].Value),
                            row.Cells["StokKodu"].Value?.ToString(),
                            row.Cells["KarsilamaTuru"].Value?.ToString(),
                            Convert.ToDecimal(row.Cells["ToplamUretilecekMiktar"].Value),
                            Convert.ToDecimal(row.Cells["GerekenMiktar"].Value),
                            row.Cells["Birim"].Value?.ToString()
                        );
                    }
                }

                // Eğer seçili satır yoksa uyarı mesajı göster
                if (!hasSelectedRows)
                {
                    MessageBox.Show("Lütfen bir satır seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Stored Procedure parametrelerini ayarla
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@TalepEden", SqlDbType.NVarChar) { Value = talepEden },
            new SqlParameter("@KaynakDepo", SqlDbType.NVarChar) { Value = kaynakdepo },
            new SqlParameter("@TalepEdilenDepo", SqlDbType.NVarChar) { Value = talepEdilenDepo },
            new SqlParameter("@UretimEmriNo", SqlDbType.NVarChar) { Value = uretimEmriNo },
            new SqlParameter("@MRPektalep", SqlDbType.Structured)
            {
                TypeName = "dbo.MRPektalep",
                Value = mrpEktalepTable
            }
                };

                // Stored Procedure'ü çağır
                DataTable result = dbHelper.ExecuteStoredProcedure(UserID, "sp_CreateUretimDepoTalepEmri_EK", parameters);

                // Sonuçları kontrol et veya ekrana yazdır
                if (result.Rows.Count > 0)
                {
                    MessageBox.Show("Üretim depo talep emri başarıyla oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    onaymailat("talepno yazılacak");
                }
                else
                {
                    MessageBox.Show("Üretim depo talep emri oluşturulamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadUretimDepoTalepHeaderToGrid(int userId, DataGridView dataGridView)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            try
            {
                // sp_GetUretimDepoTalepHeader prosedürünü çalıştır
                DataTable result = dbHelper.ExecuteStoredProcedure(userId, "sp_GetUretimDepoTalepHeader");

                // Sonuçları DataGridView'e bağla
                if (result.Rows.Count > 0)
                {
                    dataGridView.DataSource = result;

                    // Otomatik sütun boyutlandırmayı etkinleştir
                    dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                else
                {
                    MessageBox.Show("Uygun veri bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Yetkilendirme hatası: {ex.Message}", "Yetkilendirme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL hatası: {ex.Message}", "SQL Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void button29_Click(object sender, EventArgs e)
        {


        }

        private async void button14_Click(object sender, EventArgs e)
        {
            string UretimEmriIstasyon = dvg_lotbarkod.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();
            DeleteLotsFromList(UretimEmriIstasyon, list_lotbarkod);
            await GetUretimEmriIstasyonDetailstotree(UretimEmriIstasyon);

            list_lotbarkod.Items.Clear();

            dvg_lotbarkod.Columns.Clear();
            await GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_lotbarkod);
            dvg_alturetimemri.Columns.Clear();
            string Uretimdepotalepno = dvg_lotbarkod.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();
            await GetUretimEmriDetay(txt_uretimemrinoGLOBAL.Text, Uretimdepotalepno, dvg_alturetimemri);
            //MessageBox.Show("İşlem başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            int rowsAffected = dbHelper.ExecuteNonQuery(UserID, "sp_UpdateUEISTUretilenmiktar", parameters);

            GetUretimistasyonEmriDetay(UretimEmriIstasyon, dvg_uretimistasyonplanlamalist);

        }
        public void LoadLotBarkodlariToDataGridView(string connectionString, string uretimEmriIstasyonNo, DataGridView dataGridView)
        {
            // İstasyon numarası boş veya null ise uyarı ver ve işlemi sonlandır
            if (string.IsNullOrWhiteSpace(uretimEmriIstasyonNo))
            {
                MessageBox.Show("Lütfen öncelikle istasyon atama yapınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Bağlantıyı aç
                    connection.Open();

                    // Stored procedure için komut oluştur
                    using (SqlCommand command = new SqlCommand("sp_GetLotBarkodlariByIstasyonNo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parametreleri ekle
                        command.Parameters.AddWithValue("@UretimEmriIstasyonNo", uretimEmriIstasyonNo);

                        // DataTable oluştur ve verileri doldur
                        DataTable resultTable = new DataTable();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(resultTable);
                        }

                        // DataGridView'e verileri ata
                        dataGridView.DataSource = resultTable;

                        // Sütunları otomatik boyutlandır
                        dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunda mesaj göster
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button29_Click_1(object sender, EventArgs e)
        {
            //uretimemritasi(dvg_alturetimemri, dvg_istasyonyonetlist, "istasyon yönetimi");

            //string uretimEmriIstasyonNo = dvg_istasyonyonetlist.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();

            //// DataGridView'e verileri yükle
            //LoadLotBarkodlariToDataGridView(connectionstring, uretimEmriIstasyonNo, dvg_lotisleyen);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            //string UretimEmriIstasyon = dvg_istasyonyonetlist.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();
            //UpdateUEISTUretilenmiktar(UretimEmriIstasyon, Convert.ToInt32(txt_uretilenmiktar.Text));
        }
        public void GetLastStationForLot(int userId, string connectionString, string lotNumarasi, DataGridView dvg_lotokut)
        {
            // DatabaseHelper oluştur
            DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

            // Stored Procedure parametrelerini tanımlayın
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@LotNumarasi", SqlDbType.NVarChar) { Value = lotNumarasi }
            };

            try
            {
                // Stored Procedure'ü çalıştır
                DataTable result = dbHelper.ExecuteStoredProcedure(userId, "sp_GetLastStationForLot", parameters);

                // DataGridView'ın sütunlarını kontrol et
                if (dvg_lotokut.Columns.Count == 0)
                {
                    dvg_lotokut.Columns.Add("LotNumarasi", "LotNumarasi");
                    dvg_lotokut.Columns.Add("Altistasyonadi", "İstasyon Numarası");
                    dvg_lotokut.Columns.Add("IstasyonAdi", "İstasyon Adı");
                    dvg_lotokut.Columns.Add("Sorumlu", "Sorumlu");
                }

                // Sonuçları kontrol edin
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        string Altistasyonadi = row["Altistasyonadi"].ToString();
                        string istasyonAdi = row["IstasyonAdi"].ToString();
                        string Sorumlu = row["Sorumlu"].ToString();

                        // DataGridView'a yeni satır ekle
                        dvg_lotokut.Rows.Add(lotNumarasi, Altistasyonadi, istasyonAdi, Sorumlu);
                    }
                }
                else
                {
                    MessageBox.Show("Lot numarası için istasyon bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter)
            {
                string barkod = txt_lotokut.Text.Trim();

                if (!string.IsNullOrEmpty(barkod))
                {
                    // **DataGridView içinde barkod var mı kontrol et**
                    bool barkodVar = false;

                    foreach (DataGridViewRow row in dvg_lotokut.Rows)
                    {
                        if (row.Cells["LotNumarasi"].Value != null &&
                            row.Cells["LotNumarasi"].Value.ToString().Trim() == barkod)
                        {
                            barkodVar = true;
                            break;
                        }
                    }

                    if (barkodVar)
                    {
                        MessageBox.Show("Bu Barkod zaten Eklenmiş!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txt_lotokut.Clear();
                        txt_lotokut.Focus();
                        return; // İşlemi sonlandır
                    }

                    // **Barkod bilgisi işleniyor**
                    GetLastStationForLot(UserID, connectionstring, barkod, dvg_lotokut);

                    // **İşlem tamamlandıktan sonra TextBox'ı temizle ve fokus yap**
                    txt_lotokut.Clear();
                    txt_lotokut.Focus();
                }
            }
        }
        public void ExecuteCreateOrUpdateLotUretForAllRows(
    int userId,
    string uretimEmriIstasyonNo,
    int miktar,
    string createUser,
    DataGridView dvgLotOkut,
    string connectionString)
        {
            try
            {
                // DatabaseHelper örneği oluştur
                DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

                foreach (DataGridViewRow row in dvgLotOkut.Rows)
                {
                    if (row.Cells["LotNumarasi"].Value != null) // "Lot Numarası" hücresi boş değilse
                    {
                        string lotNumarasi = row.Cells["LotNumarasi"].Value.ToString();

                        // Stored procedure parametrelerini oluştur
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                    new SqlParameter("@UretimEmriIstasyonNo", SqlDbType.NVarChar) { Value = uretimEmriIstasyonNo },
                    new SqlParameter("@LotNumarasi", SqlDbType.NVarChar) { Value = lotNumarasi },
                    new SqlParameter("@Miktar", SqlDbType.Decimal) { Value = miktar },
                    new SqlParameter("@CreateUser", SqlDbType.NVarChar) { Value = createUser }
                        };

                        // Stored procedure'ü çalıştır
                        dbHelper.ExecuteNonQuery(userId, "sp_CreateOrUpdateLotUret", parameters);
                    }
                }

                MessageBox.Show("Tüm lot numaraları için işlem tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task istasyonata(string alturetimemrino, string altistasyonadi, string sorumlu, string user)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            // Stored procedure için parametreler
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Alturetimno", SqlDbType.NVarChar) { Value = alturetimemrino },
        new SqlParameter("@Altistasyonadi", SqlDbType.NVarChar) { Value = altistasyonadi },
        new SqlParameter("@Sorumlukisiadi", SqlDbType.NVarChar) { Value = sorumlu },
        new SqlParameter("@CreateUser", SqlDbType.NVarChar) { Value = user }
            };

            try
            {
                // Stored procedure'ü çalıştır ve dönen değeri al
                DataTable result = dbHelper.ExecuteStoredProcedure(5, "sp_altisyonataTEST", parameters);

                if (result != null && result.Rows.Count > 0)
                {
                    // Dönen ilk satırdaki "UretimEmriIstasyonNo" değerini al
                    string uretimEmriIstasyonNo = result.Rows[0]["UretimEmriIstasyonNo"].ToString();

                    // Eğer değer boş değilse, TextBox'a ata
                    if (!string.IsNullOrWhiteSpace(uretimEmriIstasyonNo))
                    {
                        txt_globaluretimemriistasyon.Text = uretimEmriIstasyonNo;
                        MessageBox.Show($"İşlem başarılı! Üretim Emri İstasyon No: {uretimEmriIstasyonNo}");
                    }
                    else
                    {
                        MessageBox.Show("Hata: Üretim Emri İstasyon No alınamadı.");
                    }
                }
                else
                {
                    MessageBox.Show("Hata: Stored Procedure boş sonuç döndürdü.");
                }
            }
            catch (Exception ex)
            {
                // Hata mesajını yazdır
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        public async Task LoadLotByIstasyonToGrid(int userId, string istasyonAdi, string altIstasyonAdi, string sorumluKisiAdi, DataGridView dataGridView)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            try
            {
                // Stored Procedure için parametreler
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@IstasyonAdi", SqlDbType.NVarChar) { Value = istasyonAdi },
            new SqlParameter("@AltIstasyonAdi", SqlDbType.NVarChar) { Value = altIstasyonAdi },
            new SqlParameter("@SorumluKisiAdi", SqlDbType.NVarChar) { Value = sorumluKisiAdi }
                };

                // sp_getlotbyistasyon prosedürünü çalıştır
                DataTable result = dbHelper.ExecuteStoredProcedure(userId, "sp_getlotbyistasyon", parameters);

                // Eğer sonuç döndüyse DataGridView'a ata
                if (result.Rows.Count > 0)
                {
                    dataGridView.DataSource = result;
                    dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları dolduracak şekilde ayarla
                }
                else
                {
                    //MessageBox.Show("Kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Yetkilendirme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public async Task LoadLotByIstasyonToGriddurum(int userId, string istasyonAdi, string altIstasyonAdi, DataGridView dataGridView)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            try
            {
                // Stored Procedure için parametreler
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@IstasyonAdi", SqlDbType.NVarChar) { Value = istasyonAdi },
            new SqlParameter("@AltIstasyonAdi", SqlDbType.NVarChar) { Value = altIstasyonAdi }
                };

                // sp_getlotbyistasyon prosedürünü çalıştır
                DataTable result = dbHelper.ExecuteStoredProcedure(userId, "sp_getlotbyistasyondurum", parameters);

                // Eğer sonuç döndüyse DataGridView'a ata
                if (result.Rows.Count > 0)
                {
                    dataGridView.DataSource = result;
                    dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları dolduracak şekilde ayarla
                }
                else
                {
                    MessageBox.Show("Kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Yetkilendirme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private async void button30_Click(object sender, EventArgs e)
        {
            if (dvg_lotokut.Rows.Count <= 0)
            {
                MessageBox.Show("Lütfen Lot barkod no giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Fonksiyondan çık
            }

            int userId = 5; // Mevcut kullanıcının ID'si
            //string uretimEmriIstasyonNo = dvg_istasyonyonetlist.Rows[0].Cells["UretimEmriIstasyonNo"].Value?.ToString();
            //// Üretim Emri İstasyon Numarası
            int miktar = 200; // Miktar
            string createUser = "admin"; // İşlemi yapan kullanıcı

            //string lotNumarasi = dvg_lotokut.Rows[0].Cells["LotNumarasi"].Value?.ToString();
            string istasyonAdi = lbl_istasyonata.Text;
            string altIstasyonAdi = lbl_altistasyonata.Text;
            string sorumluKisiAdi = lbl_operastorata.Text;
          
            LotUretimManager manager = new LotUretimManager(new DatabaseHelper(connectionstring));

            //string Alturetimerino = dvg_lotbarkod.Rows[0].Cells["ALTUretimEmriNo"].Value?.ToString();
            //await istasyonata(Alturetimerino, lbl_altistasyonata.Text, lbl_operastorata.Text, "admin");



            foreach (DataGridViewRow row in dvg_lotokut.Rows) // dvg_lotokut içindeki tüm satırları dön
            {
                // Eğer satır boş değilse ve "LotNumarasi" sütunu boş değilse işlemi yap
                if (row.Cells["LotNumarasi"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["LotNumarasi"].Value.ToString()))
                {
                    string lotNumarasi = row.Cells["LotNumarasi"].Value.ToString();

                    // Lot numarasını kullanarak fonksiyonları sırayla çağır
                    manager.IstasyonAtaByLotNumarasi(lotNumarasi, istasyonAdi, altIstasyonAdi, sorumluKisiAdi, createUser);
                    manager.CreateOrUpdateLotUretByLotno(lotNumarasi, istasyonAdi, altIstasyonAdi, sorumluKisiAdi, miktar, createUser);
                }
            }
            dvg_lotokut.Rows.Clear();
            dvg_lotokut.Columns.Clear();
            dvg_lotokut.DataSource = null;

            await LoadLotByIstasyonToGrid(userId, istasyonAdi, altIstasyonAdi, sorumluKisiAdi, dvg_lotisleyen);
            // DataGridView'e verileri yükle
            //LoadLotBarkodlariToDataGridView(connectionstring, uretimEmriIstasyonNo, dvg_lotisleyen);



        }

        public async Task LoadUretimDepoTalepDetailsToGrid(int userId, string uretimDepoTalepNo, DataGridView dataGridView)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

            try
            {
                // Stored Procedure için parametreler
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@UretimDepoTalepNo", SqlDbType.NVarChar) { Value = uretimDepoTalepNo }
                };

                // sp_GetUretimDepoTalepDetails prosedürünü çalıştır
                DataTable result = dbHelper.ExecuteStoredProcedure(userId, "sp_GetUretimDepoTalepDetails", parameters);

                // Eğer sonuç döndüyse DataGridView'a ata
                if (result.Rows.Count > 0)
                {
                    dataGridView.DataSource = result;
                    dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları dolduracak şekilde ayarla
                }
                else
                {
                    MessageBox.Show("Kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Yetkilendirme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void button33_Click(object sender, EventArgs e)
        {
            if (dvg_depouretimtaleplist.SelectedRows.Count > 0)
            {

                uretimemritasi(dvg_depouretimtaleplist, dvg_depouretimdetaybaslik, "depoUretimDetay");
                string Uretimdepotalep = dvg_depouretimdetaybaslik.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();


                LoadUretimDepoTalepDetailsToGrid(userId: UserID, uretimDepoTalepNo: Uretimdepotalep, dataGridView: dvg_depouretimdetaylist);
                ////Mrpcalistir(dvg_depouretimdetaybaslik.Rows[0].Cells["UretilecekUrunKodu"].Value?.ToString(), (int)Convert.ToInt64(dvg_depouretimdetaybaslik.Rows[0].Cells["PlanlananMiktar"].Value?.ToString()), dvg_depouretimdetaybaslik.Rows[0].Cells["TalepEdilenDepo"].Value?.ToString(), dvg_mrp);

                combo_kaynakdepo.Text = dvg_depouretimdetaybaslik.Rows[0].Cells["KaynakDepo"].Value?.ToString();
                combo_hedefdepo.Text = dvg_depouretimdetaybaslik.Rows[0].Cells["TalepEdilenDepo"].Value?.ToString();
            }
            else
            {
                MessageBox.Show("Lütfen bir ÜRETİM EMRİ SEÇİN !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //GoToTabPage("İstasyon Planlama");
        }
        public void InsertDepoHareketi(
     int userId,
     string connectionString,
     string kaynakDepoAdi,
     string hedefDepoAdi,
     DateTime stokGirisTarihi,
     string belgeNo,
     string satinAlmaBelgeNo,
     string tedarikciKodu,
     string hareketNedeni,
     string aciklama,
     AdvancedDataGridView dvg_movelistgiris,
     string Harekettipi)
     
        {
            try
            {
                // Kaynak ve hedef depo adları kontrolü
                if (string.IsNullOrWhiteSpace(kaynakDepoAdi))
                {
                    MessageBox.Show("Kaynak depo seçilmelidir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(Harekettipi))
                {
                    MessageBox.Show("Hareket Tipi Seçilmelidir !", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }
                if (string.IsNullOrWhiteSpace(satinAlmaBelgeNo))
                {
                    MessageBox.Show("Lütfen Satınalma Belge Noyu Satınalmadan öğrenerek Satınalma Belge Noya giriniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }
           

                // Detay tablosunu oluştur
                DataTable detaylar = new DataTable();
                detaylar.Columns.Add("StokID", typeof(string));
                detaylar.Columns.Add("LotNumarasi", typeof(string));
                detaylar.Columns.Add("SeriNumarasi", typeof(string));
                detaylar.Columns.Add("Miktar", typeof(decimal));
                detaylar.Columns.Add("Birim", typeof(string));
                detaylar.Columns.Add("StokTur", typeof(string));

                // DataGridView'deki tüm satırları işleme dahil et
                foreach (DataGridViewRow row in dvg_movelistgiris.Rows)
                {
                    if (row.IsNewRow) continue; // Yeni eklenen boş satırları atla

                    string stokKodu = row.Cells["StokKodu"].Value?.ToString();
                    string lotNumarasi = row.Cells["LotNumarasi"].Value?.ToString();
                    string seriNumarasi = row.Cells["SeriNumarasi"].Value?.ToString();
                    decimal miktar = row.Cells["Miktar"].Value != null ? Convert.ToDecimal(row.Cells["Miktar"].Value) : 0;
                    string birim = row.Cells["AnaBirim"].Value?.ToString();
                    string stokTur = row.Cells["Tur"].Value?.ToString();

                    if (!string.IsNullOrWhiteSpace(stokKodu))
                    {
                        detaylar.Rows.Add(stokKodu, lotNumarasi, seriNumarasi, miktar, birim, stokTur);
                    }
                }

                if (detaylar.Rows.Count == 0)
                {
                    MessageBox.Show("Detay satırı eklenmelidir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Stored Procedure parametrelerini tanımla
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@HareketTipi", SqlDbType.NVarChar) { Value = Harekettipi },
            new SqlParameter("@DepoAdi", SqlDbType.NVarChar) { Value = kaynakDepoAdi },
            new SqlParameter("@HedefDepoAdi", SqlDbType.NVarChar) { Value = hedefDepoAdi },
            new SqlParameter("@HareketTarihi", SqlDbType.DateTime) { Value = stokGirisTarihi },
            new SqlParameter("@BelgeNo", SqlDbType.NVarChar) { Value = belgeNo },
            new SqlParameter("@Satinalmabelgeno", SqlDbType.NVarChar) { Value = satinAlmaBelgeNo },
            new SqlParameter("@Tedarikcikodu", SqlDbType.NVarChar) { Value = tedarikciKodu },
            new SqlParameter("@HareketNedeni", SqlDbType.NVarChar) { Value = hareketNedeni },
            new SqlParameter("@Aciklama", SqlDbType.NVarChar) { Value = aciklama },
            new SqlParameter("@CreatedUser", SqlDbType.NVarChar) { Value = "admin" },
            new SqlParameter("@Detaylar", SqlDbType.Structured)
            {
                TypeName = "dbo.DepoMovLineType",
                Value = detaylar
            }
                };

                // DatabaseHelper örneği oluştur ve prosedürü çalıştır
                DatabaseHelper dbHelper = new DatabaseHelper(connectionString);
                dbHelper.ExecuteNonQuery(userId, "sp_InsertDepoHareketi", parameters);

                MessageBox.Show("Depo hareketi başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            InsertDepoHareketi(
    userId: UserID,
    connectionString: connectionstring,
    kaynakDepoAdi: combo_depogiris.Text,
    hedefDepoAdi: "",
    stokGirisTarihi: dtp_Stokgiris.Value,
    belgeNo: txt_irsbelgeno.Text,
    satinAlmaBelgeNo: txt_satinalmabelgeno.Text,
    tedarikciKodu: txt_tedarikci_stokgiris.Text,
    hareketNedeni: combo_hareketNedenleri.Text,
    aciklama: txt_Aciklama_Stokgiris.Text,
    dvg_movelistgiris: dvg_movelistgiris,
    Harekettipi: ComboDepo_Hareketi.Text
);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //uretimemritasi(dvg_stoklistgiris, dvg_movelistgiris, "Stok  Giriş");

             
        }
        public async Task DepoTransfer(int userId, DataGridView dvg_depouretimdetaylist, DataGridView dvg_depouretimdetaybaslik, string combo_kaynakdepo, string combo_hedefdepo)
        {
            try
            {
                // DatabaseHelper örneği
                DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);

                // Kaynak depo ve hedef depo adları kontrolü
                if (string.IsNullOrWhiteSpace(combo_kaynakdepo) || string.IsNullOrWhiteSpace(combo_hedefdepo))
                {
                    MessageBox.Show("Kaynak ve hedef depo seçilmelidir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kaynak ve hedef depo aynı mı kontrol et
                if (combo_kaynakdepo == combo_hedefdepo)
                {
                    MessageBox.Show("Depolar aynı! Lütfen kaynak ve hedef depoyu kontrol ediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Rezervasyon numarası
                string rezervasyonNo = dvg_depouretimdetaybaslik.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();
 
                if (string.IsNullOrWhiteSpace(rezervasyonNo))
                {
                    MessageBox.Show("Rezervasyon numarası boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Detaylar DataTable oluştur
                DataTable detaylar = new DataTable();
                detaylar.Columns.Add("StokKodu", typeof(string));
                detaylar.Columns.Add("Rezervasyonno", typeof(string));
                detaylar.Columns.Add("Rezervasyonid", typeof(int));
                detaylar.Columns.Add("Miktar", typeof(decimal));
                detaylar.Columns.Add("Birim", typeof(string));
                detaylar.Columns.Add("StokTur", typeof(string));

                int rezervasyonid = 0;

                // Sadece seçili satırlarla işlem yap
                foreach (DataGridViewRow selectedRow in dvg_depouretimdetaylist.SelectedRows)
                {
                    string durum = selectedRow.Cells["Durum"].Value?.ToString();

                    if (durum == "STOK YETERSİZ")
                    {
                        MessageBox.Show("Yetersiz stok! Öncelikle hammadde depoya giriş yapınız.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string stokKodu = selectedRow.Cells["StokKodu"].Value?.ToString();
                    decimal miktar = Convert.ToDecimal(selectedRow.Cells["GerekenMiktar"].Value);
                    string birim = selectedRow.Cells["Birim"].Value?.ToString();
                    string stokTur = "HMD";
                    rezervasyonid = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                    if (!string.IsNullOrWhiteSpace(stokKodu))
                    {
                        detaylar.Rows.Add(stokKodu, rezervasyonNo, rezervasyonid, miktar, birim, stokTur);
                    }
                }

                if (detaylar.Rows.Count == 0)
                {
                    MessageBox.Show("Transfer için seçilen stok bulunmamaktadır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Stored Procedure parametrelerini tanımla
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@KaynakDepoAdi", SqlDbType.NVarChar) { Value = combo_kaynakdepo },
            new SqlParameter("@HedefDepoAdi", SqlDbType.NVarChar) { Value = combo_hedefdepo },
            new SqlParameter("@HareketNedeni", SqlDbType.NVarChar) { Value = "Transfer" },
            new SqlParameter("@Rezervasyonno", SqlDbType.NVarChar) { Value = rezervasyonNo },
            new SqlParameter("@Rezervasyonid", SqlDbType.Int) { Value = rezervasyonid },
            new SqlParameter("@Aciklama", SqlDbType.NVarChar) { Value = "Stok transferi işlemi" },
            new SqlParameter("@CreatedUser", SqlDbType.NVarChar) { Value = "admin" },
            new SqlParameter("@Detaylar", SqlDbType.Structured)
            {
                TypeName = "dbo.DepoMovLineType",
                Value = detaylar
            }
                };

                // Stored Procedure çağır
                await Task.Run(() => dbHelper.ExecuteNonQuery(userId, "sp_TransferStockBetweenDepots", parameters));

                MessageBox.Show("Stok transferi başarıyla tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button34_Click(object sender, EventArgs e)
        {



            // Kaynak ve hedef depo adlarını al
            string kaynakDepoAdi = combo_kaynakdepo.Text;
            string hedefDepoAdi = combo_hedefdepo.Text;
            string Uretimdepotalep = dvg_depouretimdetaybaslik.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();
            // Depo transfer işlemini çağır


            await DepoTransfer(UserID, dvg_depouretimdetaylist, dvg_depouretimdetaybaslik, kaynakDepoAdi, hedefDepoAdi);
            await LoadUretimDepoTalepDetailsToGrid(userId: UserID, uretimDepoTalepNo: Uretimdepotalep, dataGridView: dvg_depouretimdetaylist);

        }

        private void Combo_depolar_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }
        // LotNumarasi, SeriNumarasi, Miktar ve diğer kolonların kontrolünü yapan ve ekleyen fonksiyon
        private void EnsureColumnsExistAndEditable(AdvancedDataGridView targetGrid)
        {
            // Gerekli kolonlar yoksa ekle
            if (!targetGrid.Columns.Contains("StokKodu"))
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "StokKodu", HeaderText = "StokKodu" });

            if (!targetGrid.Columns.Contains("StokAdi"))
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "StokAdi", HeaderText = "Stok Adı" });

            if (!targetGrid.Columns.Contains("GrupKodu"))
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "GrupKodu", HeaderText = "Grup Kodu" });

            if (!targetGrid.Columns.Contains("Tur"))
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tur", HeaderText = "Tür" });

            if (!targetGrid.Columns.Contains("AnaBirim"))
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "AnaBirim", HeaderText = "Ana Birim" });

            if (!targetGrid.Columns.Contains("LotNumarasi"))
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LotNumarasi",
                    HeaderText = "Lot Numarası",
                    DefaultCellStyle = { BackColor = Color.LightBlue },
                    ReadOnly = false // Düzenlenebilir yap
                });

            if (!targetGrid.Columns.Contains("SeriNumarasi"))
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SeriNumarasi",
                    HeaderText = "Seri Numarası",
                    DefaultCellStyle = { BackColor = Color.LightBlue },
                    ReadOnly = false // Düzenlenebilir yap
                });

            if (!targetGrid.Columns.Contains("Miktar"))
                targetGrid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Miktar",
                    HeaderText = "Miktar",
                    DefaultCellStyle = { BackColor = Color.LightBlue },
                    ReadOnly = false // Düzenlenebilir yap
                });

            // Kolonları renklendirme
            HighlightColumns(targetGrid);
        }

        // Belirli kolonları renklendiren fonksiyon
        private void HighlightColumns(DataGridView targetGrid)
        {
            foreach (DataGridViewColumn column in targetGrid.Columns)
            {
                if (column.Name == "LotNumarasi" || column.Name == "SeriNumarasi" || column.Name == "Miktar")
                {
                    column.DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
        }
        private void dvg_stoklistgiris_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Kaynak ve hedef DataGridView'leri tanımlayın
                AdvancedDataGridView sourcedvg = sender as AdvancedDataGridView;
                AdvancedDataGridView targetdvg = dvg_movelistgiris; // Hedef DataGridView'i uygun şekilde tanımlayın

                // Seçili satır olup olmadığını kontrol et
                if (e.RowIndex >= 0 && sourcedvg != null)
                {
                    // Hedef DataGridView'deki sütunların hazır olduğundan emin olun
                    EnsureColumnsExistAndEditable(targetdvg);

                    // Çift tıklanan satırı al
                    DataGridViewRow selectedRow = sourcedvg.Rows[e.RowIndex];

                    // Yeni bir satır oluştur ve verileri ekle
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(targetdvg);

                    for (int i = 0; i < selectedRow.Cells.Count; i++)
                    {
                        newRow.Cells[i].Value = selectedRow.Cells[i].Value;
                    }

                    // LotNumarasi, SeriNumarasi ve Miktar sütunları için değer ekleme
                    int lotNumarasiIndex = targetdvg.Columns["LotNumarasi"].Index;
                    int seriNumarasiIndex = targetdvg.Columns["SeriNumarasi"].Index;
                    int miktarIndex = targetdvg.Columns["Miktar"].Index;

                    if (lotNumarasiIndex != -1) newRow.Cells[lotNumarasiIndex].Value = DBNull.Value;
                    if (seriNumarasiIndex != -1) newRow.Cells[seriNumarasiIndex].Value = DBNull.Value;
                    if (miktarIndex != -1) newRow.Cells[miktarIndex].Value = 0;

                    // Hedef DataGridView'e yeni satırı ekle
                    targetdvg.Rows.Add(newRow);
                    targetdvg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                    // Gerekirse bir sayfaya geçiş yapın (menugit kullanımı isteğe bağlıdır)
                    string menugit = "TabName"; // Sayfa adı
                    //GoToTabPage(menugit);
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir satır seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        private void combo_kaynakdepo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ////if (string.IsNullOrWhiteSpace(combo_kaynakdepo.Text))
            ////{
            ////    MessageBox.Show("Lütfen Bir Sonraki Ekranda Kaynak Depo Seçiniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ////    return; // İşlem durdurulur
            ////}
            ////Mrpcalistir(dvg_depouretimdetaybaslik.Rows[0].Cells["UretilecekUrunKodu"].Value?.ToString(), (int)Convert.ToInt64(dvg_depouretimdetaybaslik.Rows[0].Cells["PlanlananMiktar"].Value?.ToString()), combo_kaynakdepo.Text, dvg_depouretimdetaylist);
            ////combo_hedefdepo.Text = dvg_depouretimdetaybaslik.Rows[0].Cells["TalepEdilenDepo"].Value?.ToString();
        }

        private void button36_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(combo_hammaddeDepoTalep.Text))
            {
                MessageBox.Show("Lütfen Depo Seçiniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlem durdurulur
            }
            //satirtasi(dvg_listuretimemri, dvg_uretimtaleplist);


            try
            {
                // DataGridView'deki ilk satır ve belirtilen kolon adına göre değer al
                if (dvg_uretimtaleplist.Rows.Count > 0)
                {

                    string uretilecekurun = dvg_uretimtaleplist.Rows[0].Cells["UretilecekUrunKodu"].Value?.ToString();

                    string Planlananmiktar = dvg_uretimtaleplist.Rows[0].Cells["PlanlananMiktar"].Value?.ToString();


                    Mrpcalistir(uretilecekurun, (int)Convert.ToInt64(Planlananmiktar), combo_hammaddeDepoTalep.Text, dvg_uretimtalepmrp);
                }
                else
                {
                    MessageBox.Show("DataGridView'de satır yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void UpdateUretimDepoTalepEmri(int userId, string connectionString, DataGridView dvg_depouretimdetaybaslik)
        {
            try
            {
                // Formdan verileri kontrol et ve parametreleri al
                if (dvg_depouretimdetaybaslik.Rows.Count == 0)
                {
                    MessageBox.Show("Detay tablosu boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string uretimDepoTalep = dvg_depouretimdetaybaslik.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();
                string talepEden = "Üretim Departmanı";
                string kaynakDepo = dvg_depouretimdetaybaslik.Rows[0].Cells["KaynakDepo"].Value?.ToString();
                string talepEdilenDepo = dvg_depouretimdetaybaslik.Rows[0].Cells["TalepEdilenDepo"].Value?.ToString();
                string uretimEmriNo = dvg_depouretimdetaybaslik.Rows[0].Cells["UretimEmriNo"].Value?.ToString();
                string ustReceteKodu = dvg_depouretimdetaybaslik.Rows[0].Cells["UretilecekUrunKodu"].Value?.ToString();
                decimal uretilecekMiktar;

                if (!decimal.TryParse(dvg_depouretimdetaybaslik.Rows[0].Cells["PlanlananMiktar"].Value?.ToString(), out uretilecekMiktar))
                {
                    MessageBox.Show("Planlanan miktar geçerli bir sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parametreleri tanımla
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@UretimDepoTalep", SqlDbType.NVarChar) { Value = uretimDepoTalep },
            new SqlParameter("@TalepEden", SqlDbType.NVarChar) { Value = talepEden },
            new SqlParameter("@KaynakDepo", SqlDbType.NVarChar) { Value = kaynakDepo },
            new SqlParameter("@TalepEdilenDepo", SqlDbType.NVarChar) { Value = talepEdilenDepo },
            new SqlParameter("@UretimEmriNo", SqlDbType.NVarChar) { Value = uretimEmriNo },
            new SqlParameter("@ustrecetekodu", SqlDbType.NVarChar) { Value = ustReceteKodu },
            new SqlParameter("@UretilecekMiktar", SqlDbType.Decimal) { Value = uretilecekMiktar }
                };

                // DatabaseHelper sınıfını kullanarak Stored Procedure çağır
                DatabaseHelper dbHelper = new DatabaseHelper(connectionString);
                dbHelper.ExecuteNonQuery(userId, "sp_UPDATEretimDepoTalepEmri", parameters);

                // Başarılı işlem mesajı
                MessageBox.Show("Üretim depo talep emri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj göster
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            //UpdateUretimDepoTalepEmri(UserID, connectionstring, dvg_depouretimdetaybaslik);
            //LoadUretimDepoTalepDetailsToGrid(userId: UserID, uretimDepoTalepNo: dvg_depouretimdetaybaslik.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString(), dataGridView: dvg_depouretimdetaylist);

            string Uretimdepotalep = dvg_depouretimdetaybaslik.Rows[0].Cells["UretimDepoTalepNo"].Value?.ToString();


            LoadUretimDepoTalepDetailsToGrid(userId: UserID, uretimDepoTalepNo: Uretimdepotalep, dataGridView: dvg_depouretimdetaylist);

          
        }

        private void button18_Click_2(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(combo_mrpcalistirdepo.Text))
            {
                MessageBox.Show("Lütfen Depo Seçiniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlem durdurulur
            }
            Mrpcalistir(txt_uretilecekurun.Text, (int)Convert.ToInt64(txt_planlananadet.Text), combo_mrpcalistirdepo.Text, dvg_mrp);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            dvg_movelistgiris.Rows.Clear();
            dvg_movelistgiris.Columns.Clear();
            dvg_movelistgiris.DataSource = null;


        }

        private void advancedDataGridViewSearchToolBar1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            dvg_stoklistgiris.TriggerFilterStringChanged();
        }

        private void advancedDataGridViewSearchToolBar1_Search(object sender, AdvancedDataGridViewSearchToolBarSearchEventArgs e)
        {
            bool restartsearch = true;
            int startColumn = 0;
            int startRow = 0;
            if (!e.FromBegin)
            {
                bool endcol = dvg_stoklistgiris.CurrentCell.ColumnIndex + 1 >= dvg_stoklistgiris.ColumnCount;
                bool endrow = dvg_stoklistgiris.CurrentCell.RowIndex + 1 >= dvg_stoklistgiris.RowCount;

                if (endcol && endrow)
                {
                    startColumn = dvg_stoklistgiris.CurrentCell.ColumnIndex;
                    startRow = dvg_stoklistgiris.CurrentCell.RowIndex;
                }
                else
                {
                    startColumn = endcol ? 0 : dvg_stoklistgiris.CurrentCell.ColumnIndex + 1;
                    startRow = dvg_stoklistgiris.CurrentCell.RowIndex + (endcol ? 1 : 0);
                }
            }
            DataGridViewCell c = dvg_stoklistgiris.FindCell(
                e.ValueToSearch,
                e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                startRow,
                startColumn,
                e.WholeWord,
                e.CaseSensitive);
            if (c == null && restartsearch)
                c = dvg_stoklistgiris.FindCell(
                    e.ValueToSearch,
                    e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                    0,
                    0,
                    e.WholeWord,
                    e.CaseSensitive);
            if (c != null)
                dvg_stoklistgiris.CurrentCell = c;
        }

        private void dvg_stoklistgiris_FilterStringChanged(object sender, AdvancedDataGridView.FilterEventArgs e)
        {
         
        }

        private void dvg_stoklistgiris_SortStringChanged(object sender, AdvancedDataGridView.SortEventArgs e)
        {

            
        }

        private void dvgsearch_depotalep_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dvgsearch_depotalep_Search(object sender, AdvancedDataGridViewSearchToolBarSearchEventArgs e)
        {
            bool restartsearch = true;
            int startColumn = 0;
            int startRow = 0;
            if (!e.FromBegin)
            {
                bool endcol = dvg_uretimtalepmrp.CurrentCell.ColumnIndex + 1 >= dvg_uretimtalepmrp.ColumnCount;
                bool endrow = dvg_uretimtalepmrp.CurrentCell.RowIndex + 1 >= dvg_uretimtalepmrp.RowCount;

                if (endcol && endrow)
                {
                    startColumn = dvg_uretimtalepmrp.CurrentCell.ColumnIndex;
                    startRow = dvg_uretimtalepmrp.CurrentCell.RowIndex;
                }
                else
                {
                    startColumn = endcol ? 0 : dvg_uretimtalepmrp.CurrentCell.ColumnIndex + 1;
                    startRow = dvg_uretimtalepmrp.CurrentCell.RowIndex + (endcol ? 1 : 0);
                }
            }
            DataGridViewCell c = dvg_uretimtalepmrp.FindCell(
                e.ValueToSearch,
                e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                startRow,
                startColumn,
                e.WholeWord,
                e.CaseSensitive);
            if (c == null && restartsearch)
                c = dvg_uretimtalepmrp.FindCell(
                    e.ValueToSearch,
                    e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                    0,
                    0,
                    e.WholeWord,
                    e.CaseSensitive);
            if (c != null)
                dvg_uretimtalepmrp.CurrentCell = c;
        }

        private async void button35_Click(object sender, EventArgs e)
        {
            string uretimemrino = dvg_uretimtaleplist.Rows[0].Cells["UretimEmriNo"].Value?.ToString();

            string uretilecekurun = dvg_uretimtaleplist.Rows[0].Cells["UretilecekUrunKodu"].Value?.ToString();

            int Planlananmiktar = Convert.ToInt32(dvg_uretimtaleplist.Rows[0].Cells["PlanlananMiktar"].Value?.ToString());

            await CreateUretimDepoTalepEmriAsync_EK(
    talepEden: "Üretim Departmanı",
    kaynakdepo: combo_hammaddeDepoTalep.Text,
    talepEdilenDepo: Combo_üretimdepotalep.Text,
    uretimEmriNo: uretimemrino,
    dvg_uretimtalepmrp: dvg_uretimtalepmrp
);

            UpdateUretimDurumuWithSP(uretimemrino, "DEPO TALEBİ OLUŞTU");
            // Yazıcı ön izlemesini göster


            //printDocument.DefaultPageSettings.Landscape = true;
            //printPreviewDialog.Document = printDocument;
            //printPreviewDialog.ShowDialog();


        }


public void SendApprovalEmail(string recipientEmail, string uretimDepoTalepNo, string onayLink, string redLink)
    {
        try
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient("smtp.abkcore.com");

            mail.From = new MailAddress("info@abkcore.com");
            mail.To.Add(recipientEmail);
            mail.Subject = "Üretim Depo Talep Onayı";
            mail.Body = $@"
            <p>Merhaba,</p>
            <p>Üretim depo talebi için onayınızı bekliyoruz:</p>
            <p>Üretim Depo Talep No: {uretimDepoTalepNo}</p>
            <a href='{onayLink}' style='background-color:green;color:white;padding:10px 20px;text-decoration:none;'>Onayla</a>
            <a href='{redLink}' style='background-color:red;color:white;padding:10px 20px;text-decoration:none;'>Reddet</a>
        ";
            mail.IsBodyHtml = true;

            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential("info@abkcore.com", "Info1234.");
            smtp.EnableSsl = false;

            smtp.Send(mail);

            MessageBox.Show("Onay maili başarıyla gönderildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mail gönderim hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

        private void button1_Click_1(object sender, EventArgs e)
        {
        }

        private void onaymailat(string talepno)
        { 
            // Onay maili gönderimi
            string recipientEmail = "mihrimah.yayman@balmy.com.tr"; //"bahadir.kumrallar@peksglobal.com";
            string uretimDepoTalepNo = talepno; // Dinamik olarak alınmalı
            string onayLink = $"https://api.abkcore.com/api/Uretimdepotaleponayi/onayla?uretimDepoTalepNo={uretimDepoTalepNo}"+"&apikey=5jH2nMk0Wy7aPnRTBx3F8L9qhGvJ1V8C";
            string redLink = $"https://api.abkcore.com/api/Uretimdepotaleponayi/reddet?uretimDepoTalepNo={uretimDepoTalepNo}" + "&apikey=5jH2nMk0Wy7aPnRTBx3F8L9qhGvJ1V8C"; ;

            SendApprovalEmail(recipientEmail, uretimDepoTalepNo, onayLink, redLink);

            SendApprovalEmail("bahadir.kumrallar@peksglobal.com", "2313", "ejkjkjdkf", "lkdflksdf");
        }

        private void button40_Click(object sender, EventArgs e)
        {
            //            string printerName = "4BARCODE 4B-2054K"; // 🖨️ Barkod yazıcınızın adı
            //            string lotBarkod = "123456789"; // 🔖 Barkod verisi

            //            // 📌 ZPL komutlarını oluştur
            //            string zplData = $@"
            //^XA
            //^FO50,50^BQN,2,10^FDQA,{lotBarkod}^FS
            //^FO50,220^A0N,30,30^FDLOT NO: {lotBarkod}^FS
            //^XZ";

            //            // 🖨️ Yazıcıya doğrudan ZPL komutlarını gönder
            //            RawPrinterHelper.SendStringToPrinter(printerName, zplData);


 
        }

        static void PrintPage(object sender, PrintPageEventArgs e)
        {
            // Örnek barkod çıktısı
            Font font = new Font("Arial", 12);
            e.Graphics.DrawString("SEZGİN ABİ SÜPERSİN :))) ", font, Brushes.Black, 10, 10);
        }

        private void button41_Click(object sender, EventArgs e)
        {
            GoToTabPage("istasyonekran");
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void dvg_istasyonsec_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Hücrenin geçerli olup olmadığını kontrol edin
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Sadece veri hücrelerini hedefler
            {
                // Tıklanan hücrenin değerini alın
                object cellValueistasyon = dvg_altistasyonsec[0, e.RowIndex].Value;
                object cellValueAltistasyon = dvg_altistasyonsec[1, e.RowIndex].Value;
                // Label'in text özelliğine atayın
                if (cellValueAltistasyon != null)
                {
                    lbl_Altistasyonsec.Text = cellValueAltistasyon.ToString(); // myLabel, etiketin ismi
                    lbl_istasyonsec.Text = cellValueistasyon.ToString();
                }
                else
                {
                    lbl_Altistasyonsec.Text = "İSTASYON SEÇ";
                    lbl_istasyonsec.Text = "İSTASYON SEÇ";
                }
            }
        }

        private void dvg_operatorlist_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Hücrenin geçerli olup olmadığını kontrol edin
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Sadece veri hücrelerini hedefler
            {
                // Tıklanan hücrenin değerini alın
                object cellValue = dvg_operatorlist[e.ColumnIndex, e.RowIndex].Value;

                // Label'in text özelliğine atayın
                if (cellValue != null)
                {
                    lbl_operatorsec.Text = cellValue.ToString(); // myLabel, etiketin ismi
                }
                else
                {
                    lbl_operatorsec.Text = "İSTASYON SEÇ";
                }
            }
        }
        public void BaslatVardiya(int userId, string sorumluAdi)
        {
            try
            {
             
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@SorumluAdi", sorumluAdi)
                };

                // Procedure çalıştır ve dönüş mesajını al
                object result = dbHelper.ExecuteScalar(userId, "sp_BaslatVardiya", parameters);

                if (result != null)
                {
                    string message = result.ToString();
                    MessageBox.Show(message, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Vardiya başlatılırken bir sorun oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Yetkilendirme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void KontrolVeOnayla()
        {
            // Seçimlerin boş olup olmadığını kontrol et
            if (string.IsNullOrWhiteSpace(lbl_istasyonsec.Text) ||
                string.IsNullOrWhiteSpace(lbl_Altistasyonsec.Text) ||
                string.IsNullOrWhiteSpace(lbl_operatorsec.Text))
            {
                MessageBox.Show("Lütfen İstasyon / Altistasyon / Operatör Seçiniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlem durdurulur
            }

            // Tüm seçimler yapılmış, onaylama mesajı göster
            DialogResult result = MessageBox.Show(
                $"Seçilen İstasyon: {lbl_istasyonsec.Text}\n" +
                $"Seçilen Alt İstasyon: {lbl_Altistasyonsec.Text}\n" +
                $"Seçilen Operatör: {lbl_operatorsec.Text}\n\n" +
                "Bu seçimlerle devam etmek istiyor musunuz?",
                "Onaylama Gerekli",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Kullanıcı onayladı, işlemi devam ettir
                DevamEt();
            }
            else
            {
                // Kullanıcı redetti, tekrar seçim yapması için ekrana yönlendir
                MessageBox.Show("Seçim ekranına geri dönüyorsunuz. Lütfen istasyon ve operatör seçiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
           
            }
        }

        // Devam işlemleri buraya yazılır
        private void DevamEt()
        {
            MessageBox.Show("İşlem devam ediyor...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Burada gerekli işlemler devam edebilir...
        }

        // Kullanıcı seçim ekranına döner
     

        private async void button42_Click(object sender, EventArgs e)
        {
            // Seçimlerin boş olup olmadığını kontrol et
            if (string.IsNullOrWhiteSpace(lbl_istasyonsec.Text) ||
                string.IsNullOrWhiteSpace(lbl_Altistasyonsec.Text) ||
                string.IsNullOrWhiteSpace(lbl_operatorsec.Text))
            {
                MessageBox.Show("Lütfen İstasyon / Altistasyon / Operatör Seçiniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlem durdurulur
            }

            // Tüm seçimler yapılmış, onaylama mesajı göster
            DialogResult result = MessageBox.Show(
                $"Seçilen İstasyon: {lbl_istasyonsec.Text}\n" +
                $"Seçilen Alt İstasyon: {lbl_Altistasyonsec.Text}\n" +
                $"Seçilen Operatör: {lbl_operatorsec.Text}\n\n" +
                "Bu seçimlerle devam etmek istiyor musunuz?",
                "Onaylama Gerekli",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Kullanıcı onayladı, işlemi devam ettir
                DevamEt();
            }
            else
            {
                // Kullanıcı redetti, tekrar seçim yapması için ekrana yönlendir
                MessageBox.Show("Seçim ekranına geri dönüyorsunuz. Lütfen istasyon ve operatör seçiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            BaslatVardiya(5,lbl_operatorsec.Text);
            lbl_operastorata.Text = lbl_operatorsec.Text;
            lbl_altistasyonata.Text = lbl_Altistasyonsec.Text;
            lbl_istasyonata.Text = lbl_istasyonsec.Text;
            dvg_lotisleyen.Rows.Clear();
            dvg_lotisleyen.Columns.Clear();
            dvg_lotbarkod.DataSource = null;
            await LoadLotByIstasyonToGrid(UserID, lbl_istasyonata.Text, lbl_altistasyonata.Text, lbl_operastorata.Text, dvg_lotisleyen);
            GoToTabPage("istasyon yönetimi");
        }

        public void BitirVardiya(int userId, string sorumluAdi)
        {
            try
            {
                // Stored procedure için gerekli parametreler
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@SorumluAdi", sorumluAdi)
                };

                // sp_BitirVardiya prosedürünü çalıştır
                int rowsAffected = dbHelper.ExecuteNonQuery(userId, "sp_BitirVardiya", parameters);

                // Başarı mesajı
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"{sorumluAdi} için vardiya başarıyla bitirildi.");
                }
                else
                {
                    Console.WriteLine("Vardiya bitirilemedi. Açık bir vardiya bulunamadı.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Yetkilendirme hatası
                Console.WriteLine($"Yetki hatası: {ex.Message}");
            }
            catch (SqlException ex)
            {
                // SQL hatası
                Console.WriteLine($"SQL hatası: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Beklenmeyen hata
                Console.WriteLine($"Beklenmeyen bir hata oluştu: {ex.Message}");
            }
        }

        private void button43_Click(object sender, EventArgs e)
        {
            string sorumluAdi = lbl_operatorsec.Text; // Operatör adı, UI üzerinden alınır
            int userId = 5; // Kullanıcı kimliği (yetkilendirme için)

            try
            {
                BitirVardiya(userId, sorumluAdi);
                MessageBox.Show($"{sorumluAdi} için vardiya başarıyla bitirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_saat.Text = DateTime.Now.ToLongTimeString();
            lbl_tarih.Text = DateTime.Now.ToLongDateString();
            lbl_saat2.Text = DateTime.Now.ToLongTimeString();
            lbl_tarih2.Text = DateTime.Now.ToLongDateString();
        }

        private async void button32_Click(object sender, EventArgs e)
        {
            string lotNumarasi = dvg_lotokut.Rows[0].Cells["LotNumarasi"].Value?.ToString();
            string istasyonAdi = lbl_istasyonata.Text;
            string altIstasyonAdi = lbl_altistasyonata.Text;
            string sorumluKisiAdi = lbl_operastorata.Text;
            // Güncelleme işlemini yap
            LotUretimManager manager = new LotUretimManager(new DatabaseHelper(connectionstring));
            string sonuc = manager.UpdateLotBitisTarihiByLotNumarasi(lotNumarasi);
 
            // Kullanıcıya mesaj göster
            MessageBox.Show(sonuc, "Sonuç", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadLotByIstasyonToGrid(UserID, istasyonAdi, altIstasyonAdi, sorumluKisiAdi, dvg_lotisleyen);
            dvg_lotokut.Rows.Clear();
            dvg_lotokut.Columns.Clear();
            dvg_lotokut.DataSource = null;
        }

        private void button44_Click(object sender, EventArgs e)
        {
            lbl_altistasyondurum.Text = lbl_altistasyonata.Text;
            LoadLotByIstasyonToGriddurum(5, lbl_istasyonata.Text, lbl_altistasyondurum.Text, dvg_altistasyondurum);
            GoToTabPage("istasyondurum");
        }

        private void button45_Click(object sender, EventArgs e)
        {
            GoToTabPage("istasyon yönetimi");
        }

        private void button46_Click(object sender, EventArgs e)
        {
            GoToTabPage("istasyonekran");
        }

        private void button47_Click(object sender, EventArgs e)
        {


            GoToTabPage("Üretim Operasyon");
        }

        private void button49_Click(object sender, EventArgs e)
        {
            GoToTabPage("istasyon yönetimi");
        }

        private void dvg_istasyonsec_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {

            // Eğer geçerli bir satır seçilmişse devam et
            if (e.RowIndex >= 0)
            {
                // Seçili satırın "IstasyonTuru" sütunundaki değeri al
                string istasyontipi = dvg_istasyonsec.Rows[e.RowIndex].Cells["IstasyonAdi"].Value?.ToString();

                // Eğer değer null değilse, alt istasyonları yükle
                if (!string.IsNullOrEmpty(istasyontipi))
                {
                    LoadAltistasyonlarToDataGridView(5, dvg_altistasyonsec, "sp_getAltistasyonlist", istasyontipi);
                }

                // Operatörleri yükle
                LoadOperatorlerToDataGridView(5, dvg_operatorlist);

                // Ana Form'a git
                GoToTabPageana("AnaForm");
            }
        }

        private void button51_Click(object sender, EventArgs e)
        {
            GoToTabPage("istasyonekran");
        }

        private void button56_Click(object sender, EventArgs e)
        {
            GoToTabPageana("Login");
        }

        private void button55_Click(object sender, EventArgs e)
        {

        }
    }
}   
