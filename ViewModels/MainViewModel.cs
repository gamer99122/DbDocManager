using DbDocManager.Models;
using DbDocManager.Services;
using DbDocManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DbDocManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDataService _dataService;
        private readonly IExportService _exportService;

        private ObservableCollection<TableDescription> _tables = new();
        private ObservableCollection<ColumnDescription> _columns = new();
        private TableDescription? _selectedTable;
        private ColumnDescription? _selectedColumn;
        private string _searchKeyword = string.Empty;

        public MainViewModel(IDataService dataService, IExportService exportService)
        {
            _dataService = dataService;
            _exportService = exportService;

            // 初始化命令
            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());
            SearchCommand = new RelayCommand(async () => await SearchAsync());
            AddTableCommand = new RelayCommand(async () => await AddTableAsync());
            EditTableCommand = new RelayCommand(async () => await EditTableAsync(), () => SelectedTable != null);
            DeleteTableCommand = new RelayCommand(async () => await DeleteTableAsync(), () => SelectedTable != null);
            AddColumnCommand = new RelayCommand(async () => await AddColumnAsync(), () => SelectedTable != null);
            EditColumnCommand = new RelayCommand(async () => await EditColumnAsync(), () => SelectedColumn != null);
            DeleteColumnCommand = new RelayCommand(async () => await DeleteColumnAsync(), () => SelectedColumn != null);
            ExportTableCommand = new RelayCommand(async () => await ExportTableAsync(), () => SelectedTable != null);
            ExportAllCommand = new RelayCommand(async () => await ExportAllAsync());

            // 載入初始資料
            _ = LoadDataAsync();
        }

        // 屬性
        public ObservableCollection<TableDescription> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public ObservableCollection<ColumnDescription> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        public TableDescription? SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (SetProperty(ref _selectedTable, value))
                {
                    _ = LoadColumnsAsync();
                    ((RelayCommand)EditTableCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteTableCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)AddColumnCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)ExportTableCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ColumnDescription? SelectedColumn
        {
            get => _selectedColumn;
            set
            {
                if (SetProperty(ref _selectedColumn, value))
                {
                    ((RelayCommand)EditColumnCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteColumnCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string SearchKeyword
        {
            get => _searchKeyword;
            set => SetProperty(ref _searchKeyword, value);
        }

        // 命令
        public ICommand LoadDataCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddTableCommand { get; }
        public ICommand EditTableCommand { get; }
        public ICommand DeleteTableCommand { get; }
        public ICommand AddColumnCommand { get; }
        public ICommand EditColumnCommand { get; }
        public ICommand DeleteColumnCommand { get; }
        public ICommand ExportTableCommand { get; }
        public ICommand ExportAllCommand { get; }

        // 方法實現
        private async Task LoadDataAsync()
        {
            var tables = await _dataService.GetAllTablesAsync();
            Tables.Clear();
            foreach (var table in tables)
            {
                Tables.Add(table);
            }
        }

        private async Task LoadColumnsAsync()
        {
            if (SelectedTable == null) return;

            var columns = await _dataService.GetColumnsAsync(SelectedTable.TableName);
            Columns.Clear();
            foreach (var column in columns)
            {
                Columns.Add(column);
            }
        }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                await LoadDataAsync();
                return;
            }

            var tables = await _dataService.SearchTablesAsync(SearchKeyword);
            Tables.Clear();
            foreach (var table in tables)
            {
                Tables.Add(table);
            }
        }

        private async Task AddTableAsync()
        {
            var dialog = new TableEditDialog();
            if (dialog.ShowDialog() == true && dialog.TableDescription != null)
            {
                try
                {
                    await _dataService.CreateTableAsync(dialog.TableDescription);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"新增表格失敗: {ex.Message}", "錯誤",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async Task EditTableAsync()
        {
            if (SelectedTable == null) return;

            var dialog = new TableEditDialog(SelectedTable);
            if (dialog.ShowDialog() == true && dialog.TableDescription != null)
            {
                try
                {
                    await _dataService.UpdateTableAsync(dialog.TableDescription);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"更新表格失敗: {ex.Message}", "錯誤",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async Task DeleteTableAsync()
        {
            if (SelectedTable == null) return;

            var result = System.Windows.MessageBox.Show(
                $"確定要刪除表格 '{SelectedTable.TableName}' 及其所有欄位描述嗎？",
                "確認刪除",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await _dataService.DeleteTableAsync(SelectedTable.TableName);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"刪除表格失敗: {ex.Message}", "錯誤",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async Task AddColumnAsync()
        {
            if (SelectedTable == null) return;

            var dialog = new ColumnEditDialog(SelectedTable.TableName);
            if (dialog.ShowDialog() == true && dialog.ColumnDescription != null)
            {
                try
                {
                    await _dataService.CreateColumnAsync(dialog.ColumnDescription);
                    await LoadColumnsAsync();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"新增欄位失敗: {ex.Message}", "錯誤",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async Task EditColumnAsync()
        {
            if (SelectedColumn == null) return;

            var dialog = new ColumnEditDialog(SelectedColumn);
            if (dialog.ShowDialog() == true && dialog.ColumnDescription != null)
            {
                try
                {
                    await _dataService.UpdateColumnAsync(dialog.ColumnDescription);
                    await LoadColumnsAsync();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"更新欄位失敗: {ex.Message}", "錯誤",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async Task DeleteColumnAsync()
        {
            if (SelectedColumn == null) return;

            var result = System.Windows.MessageBox.Show(
                $"確定要刪除欄位 '{SelectedColumn.ColumnName}' 嗎？",
                "確認刪除",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await _dataService.DeleteColumnAsync(SelectedColumn.TableName, SelectedColumn.ColumnName);
                    await LoadColumnsAsync();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"刪除欄位失敗: {ex.Message}", "錯誤",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private async Task ExportTableAsync()
        {
            if (SelectedTable == null) return;

            try
            {
                var filePath = await _exportService.ExportTable(SelectedTable.TableName);
                System.Windows.MessageBox.Show($"匯出成功: {filePath}", "匯出完成",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"匯出失敗: {ex.Message}", "錯誤",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task ExportAllAsync()
        {
            try
            {
                var filePaths = await _exportService.ExportAll();
                System.Windows.MessageBox.Show($"匯出成功，共 {filePaths.Count} 個檔案", "匯出完成",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"匯出失敗: {ex.Message}", "錯誤",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        // INotifyPropertyChanged 實作
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
