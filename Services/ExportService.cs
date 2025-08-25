// Services/ExportService.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using DbDocManager.Models;

namespace DbDocManager.Services
{
    public interface IExportService
    {
        Task<string> ExportTable(string tableName);
        Task<IReadOnlyList<string>> ExportAll();
    }

    public sealed class ExportService : IExportService
    {
        private readonly string _cs;
        private readonly string _outDir;

        public ExportService(AppSettings appSettings)
        {
            _cs = appSettings.ConnectionStrings.DbDoc;
            _outDir = appSettings.Export.OutputDir ?? "docs";
            Directory.CreateDirectory(_outDir);
        }

        public async Task<IReadOnlyList<string>> ExportAll()
        {
            var tables = await GetAllTables();
            var paths = new List<string>();
            foreach (var t in tables)
            {
                paths.Add(await ExportTable(t.TableName));
            }
            return paths;
        }

        public async Task<string> ExportTable(string tableName)
        {
            var table = await GetTable(tableName);
            var columns = await GetColumns(tableName);
            var md = BuildMarkdown(table.TableName, table.Description, columns);
            var path = Path.Combine(_outDir, table.TableName + ".md");
            await File.WriteAllTextAsync(path, md, new UTF8Encoding(false));
            return path;
        }

        private async Task<(string TableName, string? Description)> GetTable(string tableName)
        {
            const string sql = "SELECT TableName, [Description] FROM dbo.TableDescriptions WHERE TableName=@tableName;";
            using var conn = new SqlConnection(_cs);
            return await conn.QuerySingleAsync<(string, string?)>(sql, new { tableName });
        }

        private async Task<List<(string ColumnName, string? Description, string? DataType, bool? IsNullable, string? Unit, string? Example, string? ConstraintsNote)>> GetColumns(string tableName)
        {
            const string sql = "SELECT ColumnName, [Description], DataType, IsNullable, Unit, Example, ConstraintsNote FROM dbo.ColumnDescriptions WHERE TableName=@tableName ORDER BY ColumnName;";
            using var conn = new SqlConnection(_cs);
            var rows = await conn.QueryAsync<(string, string?, string?, bool?, string?, string?, string?)>(sql, new { tableName });
            return rows.ToList();
        }

        private async Task<List<(string TableName, string? Description)>> GetAllTables()
        {
            const string sql = "SELECT TableName, [Description] FROM dbo.TableDescriptions ORDER BY TableName;";
            using var conn = new SqlConnection(_cs);
            var rows = await conn.QueryAsync<(string, string?)>(sql);
            return rows.ToList();
        }

        private static string BuildMarkdown(string tableName, string? description, List<(string ColumnName, string? Description, string? DataType, bool? IsNullable, string? Unit, string? Example, string? ConstraintsNote)> columns)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {tableName} — 資料字典");
            sb.AppendLine($"**表格用途**：{description}");
            sb.AppendLine($"**最後更新**：{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine();
            sb.AppendLine("| 欄位 | 說明 | 型別 | 可空 | 單位 | 範例 | 限制 |");
            sb.AppendLine("|---|---|---:|:--:|---|---|---|");

            foreach (var c in columns)
            {
                var nullable = c.IsNullable == true ? "✔" : "✘";
                sb.AppendLine($"| {c.ColumnName} | {c.Description} | {c.DataType} | {nullable} | {c.Unit} | {c.Example} | {c.ConstraintsNote} |");
            }

            return sb.ToString();
        }
    }
}