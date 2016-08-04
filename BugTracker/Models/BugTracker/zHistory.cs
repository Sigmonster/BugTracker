using System;

namespace BugTracker.Models.BugTracker
{
    public class zHistory
    {
        public int id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public string Assignee { get; set; }
        public string State { get; set; }

    }
}