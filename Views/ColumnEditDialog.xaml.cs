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
    /// ColumnEditDialog.xaml 的互動邏輯
    /// </summary>
    public partial class ColumnEditDialog : Window
    {
        public ColumnDescription? ColumnDescription { get; private set; }

        public ColumnEditDialog(string tableName)
        {
            InitializeComponent();
            ColumnDescription = new ColumnDescription { TableName = tableName };
            DataContext = ColumnDescription;
            Title = "新增欄位";
        }

        public ColumnEditDialog(ColumnDescription existingColumn)
        {
            InitializeComponent();
            ColumnDescription = new ColumnDescription
            {
                Id = existingColumn.Id,
                TableName = existingColumn.TableName,
                ColumnName = existingColumn.ColumnName,
                Description = existingColumn.Description,
                DataType = existingColumn.DataType,
                IsNullable = existingColumn.IsNullable,
                Unit = existingColumn.Unit,
                Example = existingColumn.Example,
                ConstraintsNote = existingColumn.ConstraintsNote,
                ModifiedAt = existingColumn.ModifiedAt
            };
            DataContext = ColumnDescription;
            Title = "編輯欄位";
            ColumnNameTextBox.IsEnabled = false; // 編輯時不允許修改欄位名稱
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ColumnDescription?.ColumnName))
            {
                MessageBox.Show("請輸入欄位名稱", "輸入錯誤",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ColumnNameTextBox.Focus();
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
