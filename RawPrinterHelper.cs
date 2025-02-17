using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

public class RawPrinterHelper
{
    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFOA pDocInfo);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class DOCINFOA
    {
        public string pDocName;
        public string pOutputFile;
        public string pDataType;
    }

    public static bool SendStringToPrinter(string printerName, string command)
    {
        IntPtr hPrinter;
        DOCINFOA di = new DOCINFOA
        {
            pDocName = "ZPL Print",
            pDataType = "RAW"
        };

        try
        {
            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
            {
                MessageBox.Show("Yazıcıya bağlanılamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!StartDocPrinter(hPrinter, 1, ref di))
            {
                ClosePrinter(hPrinter);
                return false;
            }

            if (!StartPagePrinter(hPrinter))
            {
                EndDocPrinter(hPrinter);
                ClosePrinter(hPrinter);
                return false;
            }

            byte[] commandBytes = Encoding.ASCII.GetBytes(command);
            IntPtr pBytes = Marshal.AllocHGlobal(commandBytes.Length);

            try
            {
                Marshal.Copy(commandBytes, 0, pBytes, commandBytes.Length);
                int written = 0;

                bool success = WritePrinter(hPrinter, pBytes, commandBytes.Length, out written);
                if (!success || written != commandBytes.Length)
                {
                    MessageBox.Show("Yazıcıya veri gönderme hatası!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pBytes);
            }

            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);
            ClosePrinter(hPrinter);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Yazdırma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
