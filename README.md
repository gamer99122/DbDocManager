# DbDocManager

這是一個使用 C# 和 WPF 技術開發的 Windows 桌面應用程式，主要功能是協助資料庫管理員或開發人員建立和維護資料庫的結構說明文件。

## 專案架構 (Project Architecture)

本專案採用了在 WPF 開發中非常經典且廣泛使用的 **MVVM (Model-View-ViewModel)** 設計模式，將應用程式分成三個主要部分，以降低耦合度、提高可測試性和可維護性。

### Model (模型層)
*   **位置**: `Models/` 資料夾
*   **檔案**: `TableDescription.cs`, `ColumnDescription.cs`, `AppSettings.cs`
*   **職責**: 代表應用程式的資料結構。它們是純粹的 C# 物件 (POCO)，只包含資料，不關心如何被顯示。
    *   `TableDescription`: 定義一個資料表的結構 (如：表名、描述)。
    *   `ColumnDescription`: 定義一個欄位的結構 (如：欄位名、資料型別、描述)。

### View (視圖層)
*   **位置**: `Views/` 資料夾 和根目錄的 `*.xaml` 檔案
*   **檔案**: `MainWindow.xaml`, `TableEditDialog.xaml`, `ColumnEditDialog.xaml`
*   **職責**: 使用 XAML 語言定義使用者介面 (UI) 的外觀和佈局。其 C# code-behind (`.xaml.cs`) 應盡可能地少，只處理純 UI 相關的邏輯。

### ViewModel (視圖模型層)
*   **位置**: `ViewModels/` 資料夾
*   **檔案**: `MainViewModel.cs`, `RelayCommand.cs`
*   **職責**: 作為 View 和 Model 之間的橋樑，是應用程式邏輯的核心。它從 Model 取得資料，並將其轉換成 View 可以輕易「繫結 (Binding)」的屬性和命令。
    *   `MainViewModel.cs`: `MainWindow.xaml` 的邏輯核心，處理使用者互動。
    *   `RelayCommand.cs`: `ICommand` 介面的實作，讓 View 的操作可以繫結到 ViewModel 的方法。

### Services (服務層)
*   **位置**: `Services/` 資料夾
*   **檔案**: `DataService.cs`, `ExportService.cs`
*   **職責**: 處理獨立的、可重用的功能，例如資料庫存取或檔案匯出，實現「關注點分離」。
    *   `DataService`: 負責與資料庫溝通，獲取結構資訊。
    *   `ExportService`: 負責將資料匯出成檔案。

## 學習路徑 (Learning Path)

若要深入了解此專案，建議遵循以下路徑：

1.  **從入口點開始**: 閱讀 `App.xaml.cs`，了解應用程式如何啟動並建立主視窗。
2.  **理解核心 View 和 ViewModel**: 同時檢視 `Views/MainWindow.xaml` 和 `ViewModels/MainViewModel.cs`，觀察 UI 如何透過資料繫結與應用程式邏輯互動。
3.  **檢視 Model**: 查看 `Models/` 中的類別，了解應用程式的核心資料結構。
4.  **深入 Service**: 在 ViewModel 中找到呼叫 `DataService` 或 `ExportService` 的地方，並追蹤進去，了解具體的實作細節。