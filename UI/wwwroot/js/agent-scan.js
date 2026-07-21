// Biến toàn cục nếu cần
let agentConnection = null;
let isScanning = false;
let scannedFileName = null;
let scannedFilePath = null;

function startAgentConnection() {

    console.log("[DEBUG] DOMContentLoaded fired. Starting SignalR HubConnection setup.");

    // begin :: SignalR for AgentHub - Khởi tạo bên trong DOMContentLoaded
    agentConnection = new signalR.HubConnectionBuilder()
        .withUrl("/agentHub", { // Đảm bảo URL này khớp với app.MapHub trong Program.cs của Server
            transport: signalR.HttpTransportType.WebSockets,
            withCredentials: true // Nếu server cần xác thực (ví dụ: cookie, token)
        })
        .configureLogging(signalR.LogLevel.Error) // Chỉ hiển thị lỗi cho kết nối này
        .build();

    console.log("[DEBUG] agentConnection object built:", agentConnection);
    console.log("[DEBUG] Initial agentConnection state:", agentConnection.state);

    // Xử lý các sự kiện từ AgentHub
    // Tên sự kiện phải khớp chính xác với tên phương thức InvokeAsync từ Agent C#
    agentConnection.on("UpdateAgentScanners", (agentId, scanners) => {
        console.log(`[AgentHub] Nhận được danh sách máy scan từ Agent ${agentId}:`, scanners);
        updateScannerList(scanners);
        displayMessage(`Đã cập nhật danh sách máy scan từ Agent ${agentId}.`);
    });

    agentConnection.on("SendStatusUpdate", (agentId, status, scanClientId) => {
        console.log(`[AgentHub] Trạng thái scan từ Agent ${agentId} (Client: ${scanClientId}):`, status);
        displayScanStatus(agentId, status, scanClientId);
    });

    agentConnection.on("SendScanResultToClient", (agentId, scanClientId, fileName) => {
        console.log(`[AgentHub] Nhận được file PDF từ Agent ${agentId}: ${fileName} cho client ${scanClientId}`);

        scannedFileName = fileName;
        document.getElementById('scanStatus').textContent = `Đã nhận file ${fileName} từ máy scan.`;
        displayMessage(`Đã nhận file ${fileName}. Bạn có thể nhấn 'Thêm mới' để lưu vào hệ thống.`);

        const filePath = `/FileUpload/Scan/${agentId}/${fileName}`;
        // 👇 Cập nhật src cho iframe xem trước (sau khi kiểm tra file có tồn tại)
        fetch(filePath, { method: 'HEAD' })
            .then(res => {
                if (res.ok) {
                    const previewLink = document.getElementById('pdfPreviewLink');
                    previewLink.href = filePath;
                    previewLink.style.display = 'inline-block';
                    previewLink.textContent = `Xem trước file ${fileName}`;
                    // Gán vào biến toàn cục
                    scannedFilePath = filePath;
                } else {
                    displayMessage("Không tìm thấy file được quét!", "warning");
                }
            })
            .catch(err => {
                console.error("Lỗi khi kiểm tra file PDF:", err);
                displayMessage("Đã xảy ra lỗi khi kiểm tra file PDF!", "danger");
            });

        isScanning = false;
    });

    // Khi Agent báo đã quét xong 1 trang
    agentConnection.on("OnScanPageCompleted", (agentId, clientId) => {
        currentAgentId = agentId;
        currentClientId = clientId;

        // Cập nhật nội dung modal và hiển thị
        $("#scanMessage").text("Trang đã quét xong. Bạn muốn quét tiếp hay hoàn tất?");
        $("#btnScanNext").show();
        $("#btnFinishScan").text("Hoàn tất");
        $("#scanModal").modal("show");
    });

    // Khi Agent báo đã ghép xong PDF
    agentConnection.on("OnScanFinished", (agentId, clientId, pdfFileName) => {
        scannedFileName = pdfFileName;
        document.getElementById('scanStatus').textContent = `Đã nhận file ${pdfFileName} từ máy scan.`;
        displayMessage(`Đã nhận file ${pdfFileName}. Bạn có thể nhấn 'Thêm mới' để lưu vào hệ thống.`);

        const filePath = `/FileUpload/Scan/${agentId}/${pdfFileName}`;
        // 👇 Cập nhật src cho iframe xem trước (sau khi kiểm tra file có tồn tại)
        fetch(filePath, { method: 'HEAD' })
            .then(res => {
                if (res.ok) {
                    const previewLink = document.getElementById('pdfPreviewLink');
                    previewLink.href = filePath;
                    previewLink.style.display = 'inline-block';
                    previewLink.textContent = `Xem trước file ${pdfFileName}`;
                    // Gán vào biến toàn cục
                    scannedFilePath = filePath;
                } else {
                    displayMessage("Không tìm thấy file được quét!", "warning");
                }
            })
            .catch(err => {
                console.error("Lỗi khi kiểm tra file PDF:", err);
                displayMessage("Đã xảy ra lỗi khi kiểm tra file PDF!", "danger");
            });

        isScanning = false;
        $("#scanMessage").text(`Đã quét xong tất cả các trang!\nFile: ${pdfFileName}`);
        $("#btnScanNext").hide();
        $("#btnFinishScan").text("Đóng");
        $("#scanModal").modal("show");
    });

    // Khi có lỗi
    agentConnection.on("OnScanError", (agentId, clientId, error) => {
        $("#scanMessage").text("Có lỗi xảy ra: " + error);
        $("#btnScanNext").hide();
        $("#btnFinishScan").text("Đóng");
        $("#scanModal").modal("show");
    });

    // Bắt đầu kết nối đến AgentHub
    console.log("[DEBUG] Attempting to start agentConnection...");
    agentConnection.start().then(() => {
        console.log("Connected to AgentHub!");
        console.log("[DEBUG] agentConnection state after successful connection:", agentConnection.state);
        console.log("[DEBUG] My SignalR Connection ID (ClientId):", agentConnection.connectionId);
        displayMessage("Đã kết nối đến Agent Hub.");

    }).catch(err => {
        console.error("Lỗi kết nối đến AgentHub:", err);
        console.error("[DEBUG] agentConnection state after failed connection:", agentConnection.state);
        displayMessage("Lỗi kết nối đến Agent Hub. Vui lòng kiểm tra console.", 'error');
    });
    // end :: SignalR for AgentHub      
}

// Hàm này sẽ được gọi khi người dùng nhấn nút "Làm mới danh sách máy scan"
async function refreshScannerBtn() {
    console.log("[DEBUG] refreshScannerBtn called.");
    const agentId = $('#agentId').val();

    if (!agentId || agentId === '[THAY_THE_BANG_AGENT_ID_THUC_TE]') {
        displayMessage("Vui lòng nhập hoặc thiết lập Agent ID trước.", 'warning');
        return;
    }

    // Kiểm tra trạng thái kết nối trước khi gọi invoke
    if (agentConnection.state !== signalR.HubConnectionState.Connected) {
        displayMessage("Kết nối đến Agent Hub chưa sẵn sàng. Vui lòng đợi hoặc làm mới trang.", 'error');
        console.error("[DEBUG] Cannot invoke RequestScannersForAgent: agentConnection is not connected. Current state:", agentConnection.state);
        return;
    }

    console.log(`[DEBUG] Đang gửi yêu cầu lấy danh sách máy scan từ Agent ID: ${agentId}...`);
    try {
        await agentConnection.invoke("RequestScannersForAgent", agentId);
        displayMessage(`Đã gửi yêu cầu lấy danh sách máy scan đến Agent ${agentId}.`);
        console.log("[DEBUG] RequestScannersForAgent invoked successfully.");
    } catch (err) {
        console.error("Lỗi khi gửi yêu cầu lấy danh sách máy scan:", err);
        displayMessage("Không thể gửi yêu cầu lấy danh sách máy scan. Vui lòng kiểm tra console.", 'error');
    }
}

// Hàm này được gọi khi người dùng nhấn nút "Bắt đầu Scan"
async function sendScanCommandToAgent() {
    isScanning = true; // Khi bắt đầu gửi lệnh scan
    console.log("[DEBUG] sendScanCommandToAgent called.");
    const scannerSelect = document.getElementById('scannerSelect');
    const selectedDeviceId = scannerSelect.value;

    const agentId = $('#agentId').val();

    if (!agentId || agentId === '[THAY_THE_BANG_AGENT_ID_THUC_TE]') {
        displayMessage("Vui lòng nhập hoặc thiết lập Agent ID trước.", 'warning');
        return;
    }

    if (!selectedDeviceId) {
        displayMessage("Vui lòng chọn một máy scan.", 'warning');
        return;
    }

    const pageCount = parseInt(document.getElementById('pageCountInput').value || '1');
    const manualMode = document.getElementById("scannerMode").value === "true";

    if (isNaN(pageCount) || pageCount <= 0) {
        displayMessage("Số trang không hợp lệ. Vui lòng nhập số nguyên dương.", 'warning');
        return;
    }

    // Kiểm tra trạng thái kết nối trước khi gọi invoke
    if (agentConnection.state !== signalR.HubConnectionState.Connected) {
        displayMessage("Kết nối đến Agent Hub chưa sẵn sàng. Vui lòng đợi hoặc làm mới trang.", 'error');
        console.error("[DEBUG] Cannot invoke ReceiveCommand: agentConnection is not connected. Current state:", agentConnection.state);
        return;
    }
    const token = await getTokenForAgent(agentId);
    if (!token) return;

    try {
        console.log(`[DEBUG] Đang gửi lệnh scan đến Agent ${agentId}, Device ${selectedDeviceId} (${pageCount} trang).`);
        await agentConnection.invoke("ReceiveCommand", {
            Command: "Scan",
            AgentId: agentId,
            ClientId: agentConnection.connectionId,
            DeviceId: selectedDeviceId,
            ManualMode: manualMode,
            PageCount: manualMode ? 0 : pageCount,
            Token: token
        });
        console.log("[DEBUG] ReceiveCommand invoked successfully.");
        displayScanStatus(agentId, "Đang gửi lệnh...", agentConnection.connectionId);
        displayMessage(`Đang gửi lệnh scan đến Agent ${agentId}...`);
    } catch (err) {
        console.error("Lỗi khi gửi lệnh scan:", err);
        displayMessage("Có lỗi xảy ra khi gửi lệnh scan: " + err.message, 'error');
    }
}

async function FinishScanBtn() {
    console.log("[DEBUG] FinishScanBtn called.");
    $("#scanModal").modal("hide");
    const token = await getTokenForAgent(currentAgentId);
    await agentConnection.invoke("ReceiveCommand", {
        AgentId: currentAgentId,
        ClientId: currentClientId,
        Command: "FinishScan",
        Token: token
    });
}

async function ScanNextBtn() {
    console.log("[DEBUG] ScanNextBtn called.");
    $("#scanModal").modal("hide");
    await agentConnection.invoke("ReceiveCommand", {
        AgentId: currentAgentId,
        ClientId: currentClientId,
        DeviceId: $("#scannerSelect").val(),
        ManualMode: true,
        Command: "ScanNext"
    });
}

// Cập nhật danh sách máy scan vào dropdown
function updateScannerList(scanners) {
    const scannerSelect = document.getElementById('scannerSelect');
    if (!scannerSelect) {
        console.error("Element #scannerSelect not found.");
        return;
    }
    scannerSelect.innerHTML = '<option value="">-- Chọn máy scan --</option>'; // Xóa các tùy chọn cũ
    if (scanners && scanners.length > 0) {
        scanners.forEach(scanner => {
            const option = document.createElement('option');
            option.value = scanner.deviceId;
            option.textContent = `${scanner.name} (Máy: ${scanner.computerName}, User: ${scanner.userName})`;
            scannerSelect.appendChild(option);
        });
    } else {
        displayMessage("Không tìm thấy máy scan nào từ agent này.", 'info');
    }
}

// Hiển thị trạng thái scan trên UI
function displayScanStatus(agentId, status, scanClientId) {
    const statusDiv = document.getElementById('scanStatus');
    if (statusDiv) {
        statusDiv.textContent = `Trạng thái: ${status}`;
    }
}

// Hàm hiển thị thông báo tùy chỉnh (thay thế alert)
function displayMessage(message, type = 'info') {
    const messageDiv = document.getElementById('messageDisplay');
    if (messageDiv) {
        messageDiv.textContent = message;
        messageDiv.classList.remove('hidden'); // Hiển thị div
        // Đặt lại các lớp màu sắc
        messageDiv.classList.remove('bg-red-500', 'bg-blue-500', 'bg-yellow-500');
        if (type === 'error') {
            messageDiv.classList.add('bg-red-500');
        } else if (type === 'warning') {
            messageDiv.classList.add('bg-yellow-500');
        } else {
            messageDiv.classList.add('bg-blue-500');
        }
        // Tự động ẩn sau vài giây
        setTimeout(() => {
            messageDiv.classList.add('hidden'); // Ẩn div
            messageDiv.textContent = '';
        }, 5000);
    } else {
        console.warn("Element #messageDisplay not found. Falling back to console.log.");
        console.log(`MESSAGE (${type}): ${message}`);
    }
}

async function getTokenForAgent(agentId) {
    try {
        const formData = new URLSearchParams();
        formData.append("agentId", agentId);

        const response = await fetch(`/api/GenerateToken/GetTokenForAgent`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: formData.toString()
        });

        if (!response.ok) throw new Error("Không lấy được token.");
        const data = await response.json();
        return data.token || data.Token; // tuỳ backend trả về chữ thường hay hoa
    } catch (error) {
        console.error("Lỗi lấy token:", error);
        displayMessage("Không thể lấy token cho Agent.", 'error');
        return null;
    }
}
