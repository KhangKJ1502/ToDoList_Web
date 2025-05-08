/**
 * dashboard.js - JavaScript dành riêng cho chức năng dashboard
 */

// Document ready event
document.addEventListener('DOMContentLoaded', () => {
    // Khởi tạo các chức năng dashboard
    setupDashboard();
});

// Thiết lập các chức năng cho dashboard
function setupDashboard() {
    // Có thể thêm các hàm khởi tạo cho Dashboard ở đây
    console.log('Dashboard ready!');

    // Ví dụ: Thống kê về số lượng công việc (có thể fetch từ API)
    loadTaskStatistics(); 
}

// Hàm lấy thống kê về công việc
async function loadTaskStatistics() {
    // Trong tương lai, bạn có thể gọi API để lấy dữ liệu thực tế
    // Ví dụ:
    /*
    try {
        const response = await fetch('/api/task-statistics');
        const data = await response.json();
        
        if (data.success) {
            updateStatistics(data.statistics);
        }
    } catch (error) {
        console.error('Error loading task statistics', error);
    }
    */

    // Hiện tại chỉ sử dụng dữ liệu mẫu
    console.log('Task statistics loaded (demo data)');
}

// Cập nhật thông tin thống kê trên dashboard
function updateStatistics(statistics) {
    // Cập nhật số liệu vào các stat card
    document.querySelector('.stat-card:nth-child(1) .number').textContent = statistics.total;
    document.querySelector('.stat-card:nth-child(2) .number').textContent = statistics.inProgress;
    document.querySelector('.stat-card:nth-child(3) .number').textContent = statistics.completed;
    document.querySelector('.stat-card:nth-child(4) .number').textContent = statistics.overdue;
}