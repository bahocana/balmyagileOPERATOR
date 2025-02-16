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
    public partial class sofor : Form
    {
        String connectionstring = "Server=10.41.17.2\\WINWIN_SQL;Initial Catalog=Balmy_Agile; Persist Security Info=True;User ID=sa;Password=SI&wrErItoVe";
        private DatabaseHelper dbHelper;
        private DataGridViewSearchHelper _searchHelper;
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        private Form1 _form1;

        int currentUserID = Form1.UserID; // LoginForm'dan UserID al
        int currentRoleID = Form1.RoleID; // LoginForm'dan RoleID al
        public sofor(Form1 form1)
        {
            InitializeComponent();
            _form1 = form1;
        }

        private void sofor_Load(object sender, EventArgs e)
        {
            LoadAltistasyonlar("arac");
        }

        private void LoadAltistasyonlar(string istasyontipi)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(connectionstring);


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@istasyontipi", SqlDbType.NVarChar) { Value = istasyontipi },

            };
            // Stored Procedure'ü çağır ve sonuçları DataTable olarak al
            DataTable result = dbHelper.ExecuteStoredProcedure(currentUserID,"sp_listAltistasyonlar", parameters);

            // Sonuçları ekrana yazdır
            dvg_soforlist.DataSource = result;
        }

        private void sofor_FormClosed(object sender, FormClosedEventArgs e)
        {
        

        }

        private void dvg_soforlist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Tıklanan hücredeki değeri al
                string selectedData = dvg_soforlist.Rows[e.RowIndex].Cells["Altisyonadi"].Value?.ToString();

                // Form1'deki TextBox'a bu değeri gönder
                _form1.setsoforbilgi(selectedData);

                // Fasonlist formunu kapat
                this.Close();
            }
        }
    }
}
