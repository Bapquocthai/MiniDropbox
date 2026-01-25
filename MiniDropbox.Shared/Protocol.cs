using System;

namespace MiniDropbox.Shared
{

    public enum CommandType
    {
        Handshake,     
        FileCreate,     // Tạo file
        FileUpdate,     // Cập nhật nội dung
        FileDelete,     // Xóa file
        FileRename,     // Đổi tên file
        RequestSync,    // Đồng bộ file
        FileContent,    
        Conflict       
    }

    public class Protocol
    {
        public const int BUFFER_SIZE = 1024 * 4;
        public const int PORT = 5000;
    }


}