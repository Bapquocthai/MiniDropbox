using System;

namespace MiniDropbox.Shared
{
    public class FileSyncEvent
    {
        public string? FileName { get; set; }
        public string? OldFileName { get; set; }

        public string? RelativePath { get; set; }
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public string? Checksum { get; set; }
        public int Version { get; set; }
    }
}