using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Hubs
{
    // Lớp này quản lý ánh xạ giữa AgentId và ConnectionId của SignalR
    public class AgentConnectionManager
    {
        // Key: AgentId (string), Value: ConnectionId (string)
        public ConcurrentDictionary<string, string> AgentConnections { get; } = new ConcurrentDictionary<string, string>();

        // Key: ConnectionId (string), Value: AgentId (string) - để tìm AgentId khi ConnectionId bị ngắt kết nối
        public ConcurrentDictionary<string, string> ConnectionIdToAgentId { get; } = new ConcurrentDictionary<string, string>();

        // Thêm một Agent vào danh sách quản lý
        public void AddAgent(string agentId, string connectionId)
        {
            // AddOrUpdate sẽ thêm mới nếu chưa tồn tại, hoặc cập nhật ConnectionId nếu AgentId đã có
            AgentConnections.AddOrUpdate(agentId, connectionId, (key, oldValue) => connectionId);
            ConnectionIdToAgentId.AddOrUpdate(connectionId, agentId, (key, oldValue) => agentId);
        }

        // Lấy ConnectionId của một Agent dựa trên AgentId
        public string? GetConnectionId(string agentId)
        {
            AgentConnections.TryGetValue(agentId, out string? connectionId);
            return connectionId;
        }

        // Xóa một Agent khỏi danh sách quản lý khi nó ngắt kết nối
        public string? RemoveAgentByConnectionId(string connectionId)
        {
            if (ConnectionIdToAgentId.TryRemove(connectionId, out string? agentId))
            {
                AgentConnections.TryRemove(agentId, out _); // Loại bỏ cả từ dictionary kia
                return agentId;
            }
            return null;
        }
    }
}