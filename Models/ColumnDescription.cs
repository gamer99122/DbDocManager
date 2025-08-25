using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbDocManager.Models
{
    public class ColumnDescription : INotifyPropertyChanged
    {
        private int _id;
        private string _tableName = string.Empty;
        private string _columnName = string.Empty;
        private string? _description;
        private string? _dataType;
        private bool? _isNullable;
        private string? _unit;
        private string? _example;
        private string? _constraintsNote;
        private DateTime _modifiedAt;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string TableName
        {
            get => _tableName;
            set => SetProperty(ref _tableName, value);
        }

        public string ColumnName
        {
            get => _columnName;
            set => SetProperty(ref _columnName, value);
        }

        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string? DataType
        {
            get => _dataType;
            set => SetProperty(ref _dataType, value);
        }

        public bool? IsNullable
        {
            get => _isNullable;
            set => SetProperty(ref _isNullable, value);
        }

        public string? Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        public string? Example
        {
            get => _example;
            set => SetProperty(ref _example, value);
        }

        public string? ConstraintsNote
        {
            get => _constraintsNote;
            set => SetProperty(ref _constraintsNote, value);
        }

        public DateTime ModifiedAt
        {
            get => _modifiedAt;
            set => SetProperty(ref _modifiedAt, value);
        }

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
