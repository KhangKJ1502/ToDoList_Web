// Thêm code này vào đầu file JavaScript để kiểm tra chi tiết hơn
document.addEventListener('DOMContentLoaded', () => {
    console.log("Trang đã tải xong, bắt đầu tải danh sách thẻ");
    loadSidebarTags();
});

async function loadSidebarTags() {
    console.log("Đang tải danh sách thẻ...");
    try {
        // Hiển thị thông báo đang tải
        const sidebarTagsContainer = document.querySelector('.sidebar-tags .tag-list');
        if (sidebarTagsContainer) {
            sidebarTagsContainer.innerHTML = '<li class="loading-tags">Đang tải...</li>';
        }

        const response = await fetch('/api/tags');
        console.log("Response status:", response.status);

        if (!response.ok) throw new Error(`Lỗi khi tải danh sách thẻ: ${response.status}`);

        const json = await response.json();
        console.log("Dữ liệu đầy đủ từ API:", JSON.stringify(json));

        // Kiểm tra cấu trúc phản hồi từ API
        if (!json.success) throw new Error(json.message || "Lỗi từ server");

        const tags = json.data;
        console.log("Danh sách thẻ:", tags);

        if (!sidebarTagsContainer) {
            console.error('Không tìm thấy container cho danh sách thẻ');
            return;
        }

        // Xóa nội dung cũ
        sidebarTagsContainer.innerHTML = '';

        // Kiểm tra và hiển thị danh sách thẻ
        if (Array.isArray(tags) && tags.length > 0) {
            console.log(`Tìm thấy ${tags.length} thẻ, bắt đầu render`);
            tags.forEach(tag => {
                console.log("Đang tạo phần tử cho thẻ:", tag);
                const li = document.createElement('li');
                li.textContent = tag.name;

                // Kiểm tra thuộc tính id của tag
                const tagId = tag.tagID || tag.id; // Sử dụng tagID nếu có, ngược lại sử dụng id
                console.log(`ID được sử dụng cho thẻ ${tag.name}:`, tagId);

                if (tagId) {
                    li.dataset.tagId = tagId;
                    li.addEventListener('click', () => {
                        console.log(`Thẻ "${tag.name}" được nhấp với ID: ${tagId}`);
                        filterTasksByTag(tagId, tag.name);
                    });
                } else {
                    console.warn(`Không tìm thấy ID cho thẻ: ${tag.name}`);
                }

                sidebarTagsContainer.appendChild(li);
                console.log(`Đã thêm thẻ "${tag.name}" vào sidebar`);
            });
        } else {
            console.log("Không có thẻ nào được tìm thấy");
            const emptyMessage = document.createElement('li');
            emptyMessage.textContent = 'Không có thẻ nào';
            emptyMessage.classList.add('empty-message');
            sidebarTagsContainer.appendChild(emptyMessage);
        }

        console.log("Hoàn tất tải danh sách thẻ");
    } catch (error) {
        console.error('Lỗi chi tiết khi tải danh sách thẻ:', error);

        const sidebarTagsContainer = document.querySelector('.sidebar-tags .tag-list');
        if (sidebarTagsContainer) {
            sidebarTagsContainer.innerHTML = '<li class="error-message">Lỗi khi tải danh sách thẻ</li>';
        }

        showToast('Không thể tải danh sách thẻ: ' + error.message, 'error');
    }
}

async function filterTasksByTag(tagId, tagName) {
    console.log(`Bắt đầu lọc công việc theo thẻ ID: ${tagId}, Tên: ${tagName}`);
    try {
        // Hiển thị trạng thái loading
        showLoading(true);

        const url = `/api/tasks?tagId=${tagId}`;
        console.log("Gọi API:", url);

        const response = await fetch(url);
        console.log("Response status:", response.status);

        if (!response.ok) throw new Error(`Lỗi khi tải danh sách công việc: ${response.status}`);

        const json = await response.json();
        console.log("Dữ liệu công việc từ API:", json);

        if (!json.success) throw new Error(json.message || "Lỗi từ server");

        const tasks = json.data;
        console.log(`Tìm thấy ${tasks ? tasks.length : 0} công việc`);

        // Hiển thị tasks đã lọc
        updateTaskSection(`Công việc theo thẻ: ${tagName}`, tasks);

        // Đánh dấu tag đang được chọn trong sidebar
        //123
        highlightSelectedTag(tagId);

        showLoading(false);
        console.log("Hoàn tất lọc công việc theo thẻ");
    } catch (error) {
        console.error('Lỗi chi tiết khi lọc task theo tag:', error);
        showToast('Không thể lọc task theo tag: ' + error.message, 'error');
        showLoading(false);
    }
}