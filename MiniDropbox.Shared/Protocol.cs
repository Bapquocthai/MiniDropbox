using System;

namespace MiniDropbox.Shared
{

    public enum CommandType
    {
        Handshake,      // Bắt tay
        FileCreate,     // Tạo mới
        FileUpdate,     // Cập nhật nội dung
        FileDelete,     // Xóa file
        FileRename,     // Đổi tên file
        RequestSync,    // Yêu cầu đồng bộ
        FileContent,    // (Dự phòng)
        Conflict        // (Dự phòng)
    }

    public class Protocol
    {
        public const int BUFFER_SIZE = 1024 * 4;
        public const int PORT = 5000;
    }


}