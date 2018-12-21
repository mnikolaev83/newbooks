using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookDataProvider.Entities
{
    public class JobLog
    {
        public int Id { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public int BooksFetched { get; set; } = 0;
        public DateTime? FailedAt { get; set; }
        public string ExceptonMessage { get; set; } = String.Empty;
        public string ExceptonStacktrace { get; set; } = String.Empty;

    }
}