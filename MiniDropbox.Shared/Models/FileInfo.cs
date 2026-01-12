using System;
using System.Text.Json.Serialization;

namespace MiniDropbox.Shared.Models
{

    // Luu thong tin ve file
    // Dung de theo doi file, phat hien xung dot.... 
    public class FileInfo
    {
        // Duong dan file
        [JsonPropertyName("file_path")]
        public string FilePath { get; set; } = "";

        // Hash cua file
        [JsonPropertyName("file_hash")]
        public string FileHash { get; set; } = "";


        // Size cua file
        [JsonPropertyName("file_size")]
        public long FileSize { get; set; } = 0;

        // Thoi gian chinh sua file lan cuoi cung
        [JsonPropertyName("lastModified")]
        public string LastModified { get; set; }


        // Thoi gian khi file duoc upload len server
        [JsonPropertyName("uploadedAt")]
        public string UploadedAt { get; set; }

        // ID cua client da upload file lan cuoi
        [JsonPropertyName("uploadedByClientId")]
        public string UploadedByClientId { get; set; } = "";

        // Trang thai file SYNCED, SYNCING, CONFLICT, ERROR(trong server)
        [JsonPropertyName("status")]
        public string Status { get; set; } = "SYNCED";

        //Version cua file (sau moi lan update se tang len)
        [JsonPropertyName("version")]
        public int Version { get; set; } = 1;


        // File hay folder
        [JsonPropertyName("isDirectory")]
        public bool IsDirectory { get; set; } = false;


        // Constructor mac dinh
        public FileInfo() { }

        // Constructor voi tham so
        public FileInfo(string filePath, string fileHash, long fileSize)
        {
            FilePath = filePath;
            FileHash = fileHash;
            FileSize = fileSize;
            LastModified = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            UploadedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            Status = "SYNCED";
            Version = 1;
        }

        // SHA256 hash byte array
        public static string CalulateHash(byte[] data)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                return Convert.ToHexString(hash);
            }
        }

        // Sha256 hash file
        public static string CalulateHash(string filePath)
        {
            try
            {
                byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                return CalulateHash(fileData);
            }
            catch
            {
                return "";
            }   

        }

        // Convert FileInfo sang json string
        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize<FileInfo>(this);
        }

        // Parse json string thanh FileInfo object
        public static FileInfo FromJson(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<FileInfo>(json) ?? new FileInfo();
        }

        // Override ToString() de hien thi thong tin file
        public override string ToString()
        {
            return $"[{Status}] {FilePath} (v{Version}, {FileSize} bytes, {LastModified})";
        }

    }
}
