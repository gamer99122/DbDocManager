using DbDocManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DbDocManager.Views
{
    /// <summary>
    /// TableEditDialog.xaml 的互動邏輯
    /// </summary>
    public partial class TableEditDialog : Window
    {
        public TableDescription? TableDescription { get; private set; }

        public TableEditDialog()
        {
            InitializeComponent();
            TableDescription = new TableDescription();
            DataContext = TableDescription;
            Title = "新增表格";
        }

        public TableEditDialog(TableDescription existingTable)
        {
            InitializeComponent();
            TableDescription = new TableDescription
            {
                Id = existingTable.Id,
                TableName = existingTable.TableName,
                Description = existingTable.Description,
                ModifiedAt = existingTable.ModifiedAt
            };
            DataContext = TableDescription;
            Title = "編輯表格";
            TableNameTextBox.IsEnabled = false; // 編輯時不允許修改表格名稱
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TableDescription?.TableName))
            {
                MessageBox.Show("請輸入表格名稱", "輸入錯誤",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TableNameTextBox.Focus();
                return;
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
