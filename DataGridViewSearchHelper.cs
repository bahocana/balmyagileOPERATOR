using System;
using System.Windows.Forms;

public class DataGridViewSearchHelper
{
    private DataGridView _dataGridView;

    public DataGridViewSearchHelper(DataGridView dataGridView)
    {
        _dataGridView = dataGridView;
    }

    public void Search(string searchText, string columnName = null)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            // Eğer arama metni boşsa tüm satırları göster
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (!row.IsNewRow) // Yeni satır kontrolü
                {
                    row.Visible = true;
                }
            }
            return;
        }

        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            if (row.IsNewRow) // Yeni satırı kontrol et
                continue;

            bool rowVisible = false;

            // Eğer sütun adı belirtilmişse sadece o sütunu kontrol et
            if (!string.IsNullOrWhiteSpace(columnName) && row.Cells[columnName].Value != null)
            {
                rowVisible = row.Cells[columnName].Value.ToString().ToLower().Contains(searchText.ToLower());
            }
            else
            {
                // Eğer sütun adı belirtilmemişse tüm hücrelerde arama yap
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText.ToLower()))
                    {
                        rowVisible = true;
                        break;
                    }
                }
            }

            // Yeni satır kontrolü ekleniyor
            if (!row.IsNewRow)
            {
                row.Visible = rowVisible;
            }
        }
    }
}
