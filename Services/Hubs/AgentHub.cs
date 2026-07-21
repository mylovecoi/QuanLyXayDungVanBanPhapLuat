// File: AgentHub.cs

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.Hubs;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

// DTO để nhận thông tin từ Agent
public class ScannerInfo
{
    public string? AgentId { get; set; }
    public string? DeviceId { get; set; }
    public string? Name { get; set; }
    public string? ComputerName { get; set; }
    public string? UserName { get; set; }
}
public class BaseMessage // Cần có BaseMessage để deserialization JsonElement
{
    public string? Command { get; set; }
    public string? AgentId { get; set; }
    public string? ClientId { get; set; } // Thêm ClientId để gửi lại trạng thái cho đúng Web App
}

// DTO để gửi lệnh từ Web App đến Agent
public class ScanCommand
{
    public string? ClientId { get; set; }
    public string? DeviceId { get; set; }
    public int PageCount { get; set; } = 1;
}

public class AgentHub : Hub
{
    private readonly ILogger<AgentHub> _logger;
    private readonly AgentConnectionManager _agentConnectionManager;

    // Constructor để inject Logger và AgentConnectionManager
    public AgentHub(ILogger<AgentHub> logger, AgentConnectionManager agentConnectionManager)
    {
        _logger = logger;
        _agentConnectionManager = agentConnectionManager;
    }

    // Phương thức này được Agent C# gọi ngay sau khi kết nối thành công
    // để đăng ký AgentId của nó với Hub.
    // Đây là phương thức bị thiếu gây ra lỗi "Method does not exist"
    public void RegisterAgentId(string agentId)
    {
        string connectionId = Context.ConnectionId;
        _agentConnectionManager.AddAgent(agentId, connectionId);
        _logger.LogInformation($"Agent {agentId} registered with ConnectionId: {connectionId}");

        // Tùy chọn: Thông báo cho tất cả các Web App client rằng có một Agent mới online
        // await Clients.All.SendAsync("AgentOnline", agentId); 
    }

    // Phương thức này được gọi khi bất kỳ client nào (Agent hoặc Web App) ngắt kết nối.
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? agentId = _agentConnectionManager.RemoveAgentByConnectionId(Context.ConnectionId);
        if (agentId != null)
        {
            _logger.LogInformation($"Agent {agentId} (ConnectionId: {Context.ConnectionId}) disconnected.");
            // Tùy chọn: Thông báo cho tất cả các Web App client rằng Agent này đã offline
            await Clients.All.SendAsync("AgentOffline", agentId);
        }
        else
        {
            _logger.LogInformation($"Client {Context.ConnectionId} disconnected.");
        }
        await base.OnDisconnectedAsync(exception);
    }

    // --- Các phương thức mà Web App Client gọi lên Hub ---

    // Phương thức này được Web App gọi để yêu cầu danh sách máy scan từ một Agent cụ thể.
    // Web App gọi: await agentConnection.invoke("RequestScannersForAgent", agentId);
    public async Task RequestScannersForAgent(string agentId)
    {
        _logger.LogInformation($"Web App (Client: {Context.ConnectionId}) requested scanner list from Agent ID: {agentId}");
        string? agentConnectionId = _agentConnectionManager.GetConnectionId(agentId);

        if (agentConnectionId != null)
        {
            // Gửi lệnh "GetScanners" tới Agent C# tương ứng
            // Agent C# sẽ nhận lệnh này qua phương thức ReceiveCommand của nó
            await Clients.Client(agentConnectionId).SendAsync("ReceiveCommand",
                JsonSerializer.SerializeToElement(new BaseMessage { Command = "GetScanners", AgentId = agentId }));
            _logger.LogInformation($"Command 'GetScanners' sent to Agent {agentId} ({agentConnectionId}).");
        }
        else
        {
            _logger.LogWarning($"Agent {agentId} not found or not connected to forward 'GetScanners' command.");
            // Có thể gửi thông báo lỗi lại cho client Web App đã yêu cầu
            await Clients.Caller.SendAsync("SendStatusUpdate", "SERVER", $"ERROR:Agent {agentId} is offline or not registered.", Context.ConnectionId);
        }
    }

    // Phương thức này được Web App gọi để gửi một lệnh bất kỳ tới một Agent cụ thể.
    // Web App gọi: await agentConnection.invoke("ReceiveCommand", { Command: "Scan", AgentId: "...", ... });
    // Đây là phương thức mà Agent C# của bạn mong đợi để nhận các lệnh như "Scan"
    public async Task ReceiveCommand(JsonElement commandMessage) // Nhận JsonElement để giữ nguyên cấu trúc JSON
    {
        var baseMessage = JsonSerializer.Deserialize<BaseMessage>(commandMessage.GetRawText());
        if (baseMessage == null || string.IsNullOrEmpty(baseMessage.AgentId) || string.IsNullOrEmpty(baseMessage.Command))
        {
            _logger.LogWarning($"Invalid command received from Web App (Client: {Context.ConnectionId}). Message: {commandMessage.GetRawText()}");
            return;
        }

        string? agentConnectionId = _agentConnectionManager.GetConnectionId(baseMessage.AgentId);

        if (agentConnectionId != null)
        {
            // Chuyển tiếp toàn bộ JsonElement tới Agent
            await Clients.Client(agentConnectionId).SendAsync("ReceiveCommand", commandMessage);
            _logger.LogInformation($"Command '{baseMessage.Command}' forwarded from Web App (Client: {Context.ConnectionId}) to Agent {baseMessage.AgentId} ({agentConnectionId}).");
        }
        else
        {
            _logger.LogWarning($"Agent {baseMessage.AgentId} not found or not connected to forward command '{baseMessage.Command}' from Web App (Client: {Context.ConnectionId}).");
            // Gửi thông báo lỗi lại cho client Web App
            string? clientWebAppId = baseMessage.ClientId ?? Context.ConnectionId; // Dùng ClientId nếu có, không thì dùng ConnectionId của Web App gọi
            await Clients.Client(clientWebAppId).SendAsync("SendStatusUpdate", "SERVER", $"ERROR:Agent {baseMessage.AgentId} is offline or not registered.", clientWebAppId);
        }
    }

    // --- Các phương thức mà Agent C# gọi lên Hub (để Hub chuyển tiếp tới Web App) ---

    // Agent gọi: await _hubConnection.InvokeAsync("UpdateAgentScanners", _clientId, scannersInfo);
    public async Task UpdateAgentScanners(string agentId, List<ScannerInfo> scanners)
    {
        _logger.LogInformation($"Agent {agentId} sent scanner update ({scanners.Count} devices).");
        // Gửi danh sách máy scan tới tất cả các Web App client, hoặc chỉ client đã yêu cầu
        // Để đơn giản, gửi tới tất cả các client đã kết nối
        await Clients.All.SendAsync("UpdateAgentScanners", agentId, scanners);
    }

    // Agent gọi: await _hubConnection.InvokeAsync("SendStatusUpdate", _clientId, "STATUS", command.ClientId);
    public async Task SendStatusUpdate(string agentId, string status, string scanClientId)
    {
        _logger.LogInformation($"Agent {agentId} sent status update: {status} for client {scanClientId}.");
        // Gửi cập nhật trạng thái tới client Web App cụ thể đã gửi lệnh scan
        await Clients.Client(scanClientId).SendAsync("SendStatusUpdate", agentId, status, scanClientId);
    }

    // Agent gọi: await _hubConnection.InvokeAsync("SendScanResultToClient", _clientId, command.ClientId, fileName);
    public async Task SendScanResultToClient(string agentId, string scanClientId, string fileName)
    {
        _logger.LogInformation($"Agent {agentId} sent scan result: {fileName} for client {scanClientId}.");
        // Gửi kết quả scan tới client Web App cụ thể đã gửi lệnh scan
        await Clients.Client(scanClientId).SendAsync("SendScanResultToClient", agentId, scanClientId, fileName);
    }

    // Agent gọi về server
    public async Task OnScanPageCompleted(string agentId, string clientId)
    {
        // Gửi về client web
        await Clients.Client(clientId).SendAsync("OnScanPageCompleted", agentId, clientId);
    }

    public async Task OnScanError(string agentId, string clientId, string error)
    {
        await Clients.Client(clientId).SendAsync("OnScanError", agentId, clientId, error);
    }

    public async Task OnScanFinished(string agentId, string clientId, string pdfFileName)
    {
        await Clients.Client(clientId).SendAsync("OnScanFinished", agentId, clientId, pdfFileName);
    }
}