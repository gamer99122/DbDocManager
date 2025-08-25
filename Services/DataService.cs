// Services/DataService.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using DbDocManager.Models;

namespace DbDocManager.Services
{
    public interface IDataService
    {
        // 表格描述
        Task<IEnumerable<TableDescription>> GetAllTablesAsync();
        Task<TableDescription?> GetTableAsync(string tableName);
        Task<int> CreateTableAsync(TableDescription table);
        Task<bool> UpdateTableAsync(TableDescription table);
        Task<bool> DeleteTableAsync(string tableName);
        Task<IEnumerable<TableDescription>> SearchTablesAsync(string keyword);

        // 欄位描述
        Task<IEnumerable<ColumnDescription>> GetColumnsAsync(string tableName);
        Task<ColumnDescription?> GetColumnAsync(string tableName, string columnName);
        Task<int> CreateColumnAsync(ColumnDescription column);
        Task<bool> UpdateColumnAsync(ColumnDescription column);
        Task<bool> DeleteColumnAsync(string tableName, string columnName);
        Task<IEnumerable<ColumnDescription>> SearchColumnsAsync(string keyword);
    }

    public class DataService : IDataService
    {
        private readonly string _connectionString;

        public DataService(AppSettings appSettings)
        {
            _connectionString = appSettings.ConnectionStrings.DbDoc;
        }

        // 表格描述相關方法
        public async Task<IEnumerable<TableDescription>> GetAllTablesAsync()
        {
            const string sql = @"
                SELECT Id, TableName, [Description], ModifiedAt 
                FROM dbo.TableDescriptions 
                ORDER BY TableName";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TableDescription>(sql);
        }

        public async Task<TableDescription?> GetTableAsync(string tableName)
        {
            const string sql = @"
                SELECT Id, TableName, [Description], ModifiedAt 
                FROM dbo.TableDescriptions 
                WHERE TableName = @TableName";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<TableDescription>(sql, new { TableName = tableName });
        }

        public async Task<int> CreateTableAsync(TableDescription table)
        {
            const string sql = @"
                INSERT INTO dbo.TableDescriptions (TableName, [Description])
                VALUES (@TableName, @Description);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleAsync<int>(sql, table);
        }

        public async Task<bool> UpdateTableAsync(TableDescription table)
        {
            const string sql = @"
                UPDATE dbo.TableDescriptions 
                SET [Description] = @Description, ModifiedAt = SYSUTCDATETIME()
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync(sql, table);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteTableAsync(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();

            try
            {
                // 先刪除欄位描述
                const string deleteColumns = "DELETE FROM dbo.ColumnDescriptions WHERE TableName = @TableName";
                await connection.ExecuteAsync(deleteColumns, new { TableName = tableName }, transaction);

                // 再刪除表格描述
                const string deleteTable = "DELETE FROM dbo.TableDescriptions WHERE TableName = @TableName";
                var affectedRows = await connection.ExecuteAsync(deleteTable, new { TableName = tableName }, transaction);

                transaction.Commit();
                return affectedRows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<TableDescription>> SearchTablesAsync(string keyword)
        {
            const string sql = @"
                SELECT Id, TableName, [Description], ModifiedAt 
                FROM dbo.TableDescriptions 
                WHERE TableName LIKE @Keyword OR [Description] LIKE @Keyword
                ORDER BY TableName";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TableDescription>(sql, new { Keyword = $"%{keyword}%" });
        }

        // 欄位描述相關方法
        public async Task<IEnumerable<ColumnDescription>> GetColumnsAsync(string tableName)
        {
            const string sql = @"
                SELECT Id, TableName, ColumnName, [Description], DataType, IsNullable, 
                       Unit, Example, ConstraintsNote, ModifiedAt
                FROM dbo.ColumnDescriptions 
                WHERE TableName = @TableName 
                ORDER BY ColumnName";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ColumnDescription>(sql, new { TableName = tableName });
        }

        public async Task<ColumnDescription?> GetColumnAsync(string tableName, string columnName)
        {
            const string sql = @"
                SELECT Id, TableName, ColumnName, [Description], DataType, IsNullable, 
                       Unit, Example, ConstraintsNote, ModifiedAt
                FROM dbo.ColumnDescriptions 
                WHERE TableName = @TableName AND ColumnName = @ColumnName";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<ColumnDescription>(sql,
                new { TableName = tableName, ColumnName = columnName });
        }

        public async Task<int> CreateColumnAsync(ColumnDescription column)
        {
            const string sql = @"
                INSERT INTO dbo.ColumnDescriptions 
                (TableName, ColumnName, [Description], DataType, IsNullable, Unit, Example, ConstraintsNote)
                VALUES (@TableName, @ColumnName, @Description, @DataType, @IsNullable, @Unit, @Example, @ConstraintsNote);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleAsync<int>(sql, column);
        }

        public async Task<bool> UpdateColumnAsync(ColumnDescription column)
        {
            const string sql = @"
                UPDATE dbo.ColumnDescriptions 
                SET [Description] = @Description, DataType = @DataType, IsNullable = @IsNullable,
                    Unit = @Unit, Example = @Example, ConstraintsNote = @ConstraintsNote,
                    ModifiedAt = SYSUTCDATETIME()
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync(sql, column);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteColumnAsync(string tableName, string columnName)
        {
            const string sql = @"
                DELETE FROM dbo.ColumnDescriptions 
                WHERE TableName = @TableName AND ColumnName = @ColumnName";

            using var connection = new SqlConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync(sql, new { TableName = tableName, ColumnName = columnName });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<ColumnDescription>> SearchColumnsAsync(string keyword)
        {
            const string sql = @"
                SELECT Id, TableName, ColumnName, [Description], DataType, IsNullable, 
                       Unit, Example, ConstraintsNote, ModifiedAt
                FROM dbo.ColumnDescriptions 
                WHERE ColumnName LIKE @Keyword OR [Description] LIKE @Keyword
                ORDER BY TableName, ColumnName";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ColumnDescription>(sql, new { Keyword = $"%{keyword}%" });
        }
    }
}