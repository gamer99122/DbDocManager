using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDocManager.Models
{
    public sealed class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public ExportOptions Export { get; set; } = new();
        public LoggingOptions Logging { get; set; } = new();
    }

    public sealed class ConnectionStrings
    {
        public string DbDoc { get; set; } = "";
    }

    public sealed class ExportOptions
    {
        public string OutputDir { get; set; } = "docs";
    }

    public sealed class LoggingOptions
    {
        public string Path { get; set; } = "logs";
    }
}
