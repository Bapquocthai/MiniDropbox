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
        // Đóng gói file thành packet để gửi qua mạng
        public static byte[] CreatePacket(CommandType cmd, FileSyncEvent fileInfo, byte[] fileData)
        {
            var header = new MessageHeader
            {
                Command = cmd,
                PayloadJson = JsonSerializer.Serialize(fileInfo)
            };

            string jsonHeader = JsonSerializer.Serialize(header);
            byte[] headerBytes = Encoding.UTF8.GetBytes(jsonHeader);
            byte[] lengthBytes = BitConverter.GetBytes(headerBytes.Length);

            List<byte> packet = new List<byte>();
            packet.AddRange(lengthBytes);       
            packet.AddRange(headerBytes);     
            if (fileData != null)
            {
                packet.AddRange(fileData);
            }
            return packet.ToArray();
        }

        // MD5 checksum của file
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