using System;
using System.Text.Json.Serialization;

namespace MiniDropbox.Shared.Protocol
{

    // Massage types dinh nghia cac loai thong diep giua Client va Server
    public enum MessageType
    {
        // Client toi Server
        HELLO,
        SYNC_UPLOAD,  // Client gui file
        FILE_DELETE,  // Client xoa file
        SYNC_REQUEST, // Client yeu cau dong bo
        HEARTBEAT,    // Client gui tin hieu cho biet dang online

        // Server toi Client
        HELLO_ACK,    // Server phan hoi lai khi nhan duoc HELLO
        FILE_UPDATE,  // Server gui file
        FILE_DELETED, // Server thong bao file bi xoa
        SYNC_LIST,    // Server gui danh sach file de dong bo
        ERROR,        // Server thong bao loi
        ACK           // Server xac nhan da nhan tin nhan
    }
    public class Message
    {

        // Loai thong diep
        [JsonPropertyName("type")]
        public MessageType Type { get; set; }

        // ID cua client
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = "";

        // ID cua server
        [JsonPropertyName("server_id")]
        public string ServerId { get; set; } = "";

        // Duong dan file
        [JsonPropertyName("file_path")]
        public string FilePath { get; set; } = "";


        // Creat, update, delete
        [JsonPropertyName("file_action")]
        public string FileAction { get; set; } = "UPDATE";

        // Noi dung file (dung de upload)
        [JsonPropertyName("file_data")]
        public string FileData { get; set; } = "";


        // Hash cua file (dung de xu ly xung dot)
        [JsonPropertyName("file_hash")]
        public string FileHash { get; set; } = "";

        // Thoi gian chinh sua file
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";

        // Size cua file
        [JsonPropertyName("file_size")]
        public long FileSize { get; set; } = 0;

        // Thong bao tu message
        [JsonPropertyName("message")]
        public string MessageText { get; set; } = "";

        // Danh sach file
        [JsonPropertyName("file_list")]
        public FileInfo[] FileList { get; set; } = Array.Empty<FileInfo>();

        // Ten sync folder
        [JsonPropertyName("sync_folder_name")]
        public string SyncFolderName { get; set; } = "MiniDropboxSync";

        // Constructor mac dinh
        public Message() { }

        // Constructor voi type
        public Message(MessageType type, string client_id = "")
        {
            Type = type;
            client_id = client_id;
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        // Parse json string thanh Message object

        public static Message FromJson(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Message>(json) ?? new Message();

        }
    }
}
