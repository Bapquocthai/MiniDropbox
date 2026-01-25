# MiniDropbox

## Giới thiệu

MiniDropbox là ứng dụng đồng bộ hóa file tự động giữa client và server, lấy cảm hứng từ Dropbox. Dự án được xây dựng bằng C# với .NET 10.0 và Windows Forms, cho phép người dùng đồng bộ file theo thời gian thực qua mạng TCP/IP

## Tính năng chính

- Đồng bộ tự động: Tự động phát hiện và đồng bộ các thay đổi file
- Theo dõi thay đổi: Giám sát các thao tác file (tạo, sửa, xóa, đổi tên)
- Giao tiếp Client-Server: Kết nối TCP/IP ổn định giữa nhiều client và server
- Xử lý xung đột: Phát hiện và xử lý xung đột khi có nhiều thay đổi đồng thời
- Kiểm tra tính toàn vẹn**: Sử dụng checksum để đảm bảo dữ liệu chính xác
- Quản lý phiên bản: Theo dõi version của file để đồng bộ chính xác

## Cấu trúc dự án

Dự án được chia thành 3 module chính:

### 1. MiniDropbox.Client
- Ứng dụng client với giao diện Windows Forms
- Kết nối đến server để đồng bộ file
- Theo dõi thư mục local và gửi thay đổi đến server

### 2. MiniDropbox.Server
- Ứng dụng server với giao diện Windows Forms
- Quản lý kết nối từ nhiều client
- Điều phối đồng bộ file giữa các client
- Lưu trữ dữ liệu file tập trung

### 3. MiniDropbox.Shared
- Thư viện chứa các class và protocol dùng chung
- Protocol.cs: Định nghĩa các CommandType (Handshake, FileCreate, FileUpdate, FileDelete, FileRename và các hằng số (BUFFER_SIZE, PORT)
- FileSyncEvent.cs: Model chứa thông tin file (tên, đường dẫn, kích thước, thời gian sửa đổi, checksum, version)
- MessageHeader.cs: Cấu trúc header cho message truyền tải
- PacketUtils.cs: Các tiện ích xử lý packet

## Công nghệ sử dụng

- Framework: .NET 10.0
- UI: Windows Forms
- Giao thức mạng: TCP/IP
- Ngôn ngữ: C#

## Cài đặt

### Yêu cầu hệ thống
- Windows OS
- .NET 10.0 Runtime
- Visual Studio 2022 hoặc cao hơn (để build từ source)

### Hướng dẫn chạy

1. Build solution:
   ```bash
   dotnet build MiniDropbox.slnx
   ```

2. Chạy Server:
   ```bash
   cd MiniDropbox.Server/bin/Debug/net10.0-windows
   dotnet MiniDropbox.Server.dll
   ```

3. Chạy Client:
   ```bash
   cd MiniDropbox.Client/bin/Debug/net10.0-windows
   dotnet MiniDropbox.Client.dll
   ```

## Cấu hình

- Port mặc định: 5000
- Buffer size: 4KB (4096 bytes)
- Có thể thay đổi trong file `Protocol.cs`

## Kiến trúc hệ thống

```
┌─────────────┐         ┌─────────────┐
│   Client 1  │◄───────►│             │
└─────────────┘         │             │
                        │   Server    │◄───► File Storage
┌─────────────┐         │             │
│   Client 2  │◄───────►│             │
└─────────────┘         └─────────────┘
      TCP/IP
```

## Quy trình đồng bộ

1. Handshake: Client thiết lập kết nối với server
2. FileWatcher: Client theo dõi thay đổi trong thư mục đồng bộ
3. Event Detection: Phát hiện các thao tác (Create/Update/Delete/Rename)
4. Transmission: Gửi thông tin và nội dung file qua TCP
5. Sync: Server phân phối thay đổi đến các client khác

## Các CommandType và mục đích sử dụng

### CommandType đã triển khai:
- Handshake: Thiết lập kết nối ban đầu giữa client và server
- FileCreate: Tạo file mới và gửi nội dung lên server
- FileUpdate: Cập nhật nội dung file đã có
- FileDelete: Xóa file khỏi hệ thống đồng bộ
- FileRename: Đổi tên file


## Tính năng mở rộng trong tương lai

- [ ] Hỗ trợ mã hóa dữ liệu truyền tải
- [ ] Authentication và authorization
- [ ] Giao diện web-based
- [ ] Đồng bộ thư mục phân cấp
- [ ] History và khôi phục phiên bản cũ
- [ ] Nén dữ liệu để tối ưu băng thông

## Tác giả
Ngô Quốc Thái - Nguyễn Đức Thành Long
Dự án được phát triển như một đề tài học tập về lập trình mạng và đồng bộ file.
Lưu ý: Đây là phiên bản học tập và demo. Không khuyến khích sử dụng cho production mà chưa có các biện pháp bảo mật đầy đủ.
Github: https://github.com/Bapquocthai/MiniDropbox.git
