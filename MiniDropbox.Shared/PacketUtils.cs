using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MiniDropbox.Shared
{
    public static class PacketUtils
    {
        // Hàm này dùng để đóng gói File thành mảng byte gửi đi
        // Cấu trúc gói tin: [4 byte độ dài Header] + [Header JSON] + [File Content]
        public static byte[] CreatePacket(CommandType cmd, FileSyncEvent fileInfo, byte[] fileData)
        {
            // 1. Tạo Header chứa thông tin metadata
            var header = new MessageHeader
            {
                Command = cmd,
                PayloadJson = JsonSerializer.Serialize(fileInfo)
            };

            string jsonHeader = JsonSerializer.Serialize(header);
            byte[] headerBytes = Encoding.UTF8.GetBytes(jsonHeader);
            byte[] lengthBytes = BitConverter.GetBytes(headerBytes.Length); // 4 byte độ dài

            // 2. Ghép tất cả lại: [Length] + [Header] + [Data]
            List<byte> packet = new List<byte>();

            packet.AddRange(lengthBytes);       // 4 byte đầu: Báo độ dài header
            packet.AddRange(headerBytes);       // Header JSON

            if (fileData != null)
            {
                packet.AddRange(fileData);      // Nội dung file (nếu có)
            }

            return packet.ToArray();
        }

        // Hàm hỗ trợ tạo MD5 Checksum (để kiểm tra file có thay đổi nội dung không)
        public static string GetMD5Checksum(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}