using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using ZXing;

public class BarkodPrinter
{
    private ListBox _listBox;
    private PrintDocument _printDocument;
    private Font _headerFont;
    private Font _bodyFont;
    private int _currentIndex; // Hangi öğenin yazdırılacağını takip eder
    private string _mamulKodu;
    private string _uretimEmriNo;
    private string _altIstasyonAdi;
    private string _operatorAdi;
    private string _tarih;
    private ComboBox _printerComboBox; // ComboBox referansı

    public BarkodPrinter(ListBox listBox, ComboBox printerComboBox)
    {
        _listBox = listBox;
        _printerComboBox = printerComboBox;

        // Mevcut yazıcıları ComboBox'a yükle
        LoadPrinters();

        // PrintDocument yapılandırma
        _printDocument = new PrintDocument();
        _printDocument.PrintPage += PrintDocument_PrintPage;

        _headerFont = new Font("Arial", 12, FontStyle.Bold);
        _bodyFont = new Font("Arial", 10, FontStyle.Regular);
        _currentIndex = 0;

        // ComboBox'tan yazıcı seçildiğinde güncelle
        _printerComboBox.SelectedIndexChanged += PrinterComboBox_SelectedIndexChanged;

        // Varsayılan yazıcıyı ComboBox'tan al ve ayarla
        if (_printerComboBox.Items.Count > 0)
        {
            _printerComboBox.SelectedItem = _printDocument.PrinterSettings.PrinterName;
            _printDocument.PrinterSettings.PrinterName = _printerComboBox.Text;
        }
    }

    // **Mevcut yazıcıları ComboBox'a yükler**
    private void LoadPrinters()
    {
        _printerComboBox.Items.Clear();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            _printerComboBox.Items.Add(printer);
        }

        // Varsayılan yazıcıyı seç
        if (_printerComboBox.Items.Count > 0)
        {
            _printerComboBox.SelectedIndex = 0;
        }
    }

    // **ComboBox'tan seçilen yazıcıyı PrinterName olarak ayarla**
    private void PrinterComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_printerComboBox.SelectedItem != null)
        {
            _printDocument.PrinterSettings.PrinterName = _printerComboBox.Text;
        }
    }

    // **Yazdırma işlemini başlatan metot**
    public void PrintEtiket(string mamulKodu, string uretimEmriNo, string altIstasyonAdi, string operatorAdi, string tarih)
    {
        if (_listBox.Items.Count == 0)
        {
            MessageBox.Show("Listede yazdırılacak bir QR kod yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _mamulKodu = mamulKodu;
        _uretimEmriNo = uretimEmriNo;
        _altIstasyonAdi = altIstasyonAdi;
        _operatorAdi = operatorAdi;
        _tarih = tarih;

        _currentIndex = 0; // Yazdırma işlemi baştan başlar
        _printDocument.Print();
    }

    // **Yazdırma işlemi**
    private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
        float yPos = e.MarginBounds.Top; // Sayfanın üst sınırı
        float xPos = e.MarginBounds.Left; // Sayfanın sol sınırı
        float etiketHeight = 220; // Her bir etiketin yüksekliği
        int etiketPerPage = (int)(e.MarginBounds.Height / etiketHeight); // Bir sayfaya kaç etiket sığar

        for (int i = 0; i < etiketPerPage && _currentIndex < _listBox.Items.Count; i++)
        {
            // Şu anki QR kod metni
            string qrText = _listBox.Items[_currentIndex].ToString();

            // Başlık yazdır
            e.Graphics.DrawString("Balmy", _headerFont, Brushes.Black, xPos, yPos + 15);

            // QR kod oluştur ve yazdır
            Bitmap qrImage = GenerateQRCode(qrText);
            if (qrImage != null)
            {
                e.Graphics.DrawImage(qrImage, xPos, yPos + 50, 150, 150); // QR kodun boyutu ve konumu
                qrImage.Dispose();
            }

            // Metin bilgilerini yazdır
            e.Graphics.DrawString($"LotBarkod : {qrText}", _bodyFont, Brushes.Black, 0, yPos + 200);
            e.Graphics.DrawString($"Mamül Kodu: {_mamulKodu}", _bodyFont, Brushes.Black, 0, yPos + 230);
            e.Graphics.DrawString($"Üretim Emri No: {_uretimEmriNo}", _bodyFont, Brushes.Black, 0, yPos + 260);
            e.Graphics.DrawString($"Alt İstasyon Adı: {_altIstasyonAdi}", _bodyFont, Brushes.Black, 0, yPos + 290);
            e.Graphics.DrawString($"Operatör: {_operatorAdi}", _bodyFont, Brushes.Black, 0, yPos + 320);
            e.Graphics.DrawString($"Tarih: {_tarih}", _bodyFont, Brushes.Black, 0, yPos + 350);

            // Sonraki etikete geçmek için y konumunu artır
            yPos += etiketHeight;
            _currentIndex++; // Bir sonraki öğeye geç
        }

        // Eğer tüm öğeler yazdırılmadıysa, yeni sayfa gereklidir
        e.HasMorePages = (_currentIndex < _listBox.Items.Count);
    }

    // **QR Kod Üretici Fonksiyon**
    private Bitmap GenerateQRCode(string text)
    {
        BarcodeWriter barcodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 100,  // QR Kod genişliği
                Height = 100, // QR Kod yüksekliği
                Margin = 10   // Kenar boşluğu
            }
        };

        return barcodeWriter.Write(text);
    }
}
