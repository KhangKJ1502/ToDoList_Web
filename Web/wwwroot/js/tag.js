async function loadSidebarTags() {
    try {
        const response = await fetch('/api/tags');
        if (!response.ok) throw new Error("Lỗi khi tải danh sách thẻ");

        const tags = await response.json();
        const sidebarTagsContainer = document.querySelector('.sidebar-tags .tag-list');

        if (!sidebarTagsContainer) return;

        // Xóa nội dung cũ
        sidebarTagsContainer.innerHTML = '';

        // Thêm các tag mới
        tags.forEach(tag => {
            const li = document.createElement('li');
            li.textContent = tag.name;
            li.dataset.tagId = tag.id;
            li.addEventListener('click', () => filterTasksByTag(tag.id));
            sidebarTagsContainer.appendChild(li);
        });
    } catch (error) {
        console.error('Lỗi khi tải danh sách thẻ:', error);
    }
} 

async function filterTasksByTag(tagId) {
    try {
        const response = await fetch(`/api/tasks?tagId=${tagId}`);
        const tasks = await response.json();

        // Hiển thị tasks đã lọc
        updateTaskSection('Tasks filtered by tag', tasks, '#today-tasks');
    } catch (error) {
        console.error('Lỗi khi lọc task theo tag:', error);
        showToast('Không thể lọc task theo tag', 'error');
    }
}