/**
 * TaskMaster - task.js
 * Xử lý các chức năng liên quan đến Task
 */

// Biến lưu trữ trạng thái
let selectedTags = [];
let taskData = {
    categories: [],
    tags: []
};

// Khởi tạo khi trang được tải
document.addEventListener('DOMContentLoaded', () => {
    // Thiết lập sự kiện cho nút tạo task
    const submitBtn = document.querySelector('.btn.btn-primary');
    if (submitBtn) {
        submitBtn.addEventListener('click', submitTask);
    }

    // Thiết lập sự kiện cho trường nhập tag
    const tagInput = document.getElementById('tags');
    if (tagInput) {
        tagInput.addEventListener('keydown', handleTagInput);
    }

    // Đóng modal khi click bên ngoài
    window.onclick = function (event) {
        const modalTask = document.getElementById("modalAddTask");
        if (event.target === modalTask) {
            closeModal();
        }
    };

    // Tải danh sách task khi trang được tải
    fetchAndDisplayTasks();

    // Thiết lập sự kiện tìm kiếm
    const searchInput = document.querySelector('.search-bar input');
    if (searchInput) {
        searchInput.addEventListener('input', debounce(searchTasks, 300));
    }
});

function handleTagInput(event) {
    const tagInput = document.getElementById('tags');
    if (event.key === 'Enter' && tagInput.value.trim() !== '') {
        const tag = tagInput.value.trim();
        // Xử lý thêm tag (hiển thị, gửi API, v.v.)
        console.log("Tag entered:", tag);
        addTagToSelected(tag);
        tagInput.value = ''; // reset input
        event.preventDefault(); // ngăn form submit
    }
}

// ==================== CÁC HÀM CHÍNH ====================

/**
 * Mở modal thêm task và tải dữ liệu cần thiết
 */
async function addNote() {
    try {
        // Reset form trước khi mở
        resetTaskForm();

        // Tải dữ liệu từ API
        const response = await fetch('/api/get-infor-task');
        if (!response.ok) throw new Error("Lỗi khi tải thông tin");

        const data = await response.json();
        taskData = data;

        // Tải danh mục
        loadCategories(data.categories);

        // Tải tags
        loadTagSuggestions(data.tags);

        // Thiết lập ngày mặc định là hôm nay
        const today = new Date().toISOString().split('T')[0];
        document.getElementById("dueDate").value = today;

        // Hiển thị modal
        document.getElementById("modalAddTask").style.display = "flex";
    } catch (error) {
        console.error('Lỗi khi tải dữ liệu task:', error);
        showToast("Không thể tải dữ liệu!", "error");
    }
}

/**
 * Gửi dữ liệu tạo Task
 */
async function submitTask() {
    try {
        // Kiểm tra các trường bắt buộc
        const title = document.getElementById('title').value.trim();
        const categoryValue = document.getElementById('category').value;

        if (!title) {
            showToast("Vui lòng nhập tiêu đề công việc!", "error");
            document.getElementById('title').focus();
            return;
        }

        if (!categoryValue) {
            showToast("Vui lòng chọn danh mục!", "error");
            document.getElementById('category').focus();
            return;
        }

        // Hiển thị loading state
        const submitBtn = document.querySelector('.btn.btn-primary');
        const originalText = submitBtn.textContent;
        submitBtn.textContent = 'Đang xử lý...';
        submitBtn.disabled = true;

        // Chuẩn bị dữ liệu
        const task = {
            userID: getUserId(),
            title: title,
            description: document.getElementById('description').value.trim(),
            categoryID: parseInt(categoryValue),
            priority: document.getElementById('priority').value,
            status: document.getElementById('status').value,
            dueDate: document.getElementById('dueDate').value || new Date().toISOString().split('T')[0],
            subtasks: [],
            tags: selectedTags
        };
        console.log('Selected Tags:', selectedTags); // In ra giá trị của selectedTags

        // Thu thập các subtask
        document.querySelectorAll('.subtasks-container input').forEach(input => {
            const value = input.value.trim();
            if (value) {
                task.subtasks.push({
                    title: value,
                    completed: false
                });
            }
        });

        // Gửi request tạo task
        const response = await fetch('/api/Create-task', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': getCSRFToken()
            },
            body: JSON.stringify(task)
        });

        // Khôi phục trạng thái nút
        submitBtn.textContent = originalText;
        submitBtn.disabled = false;

        if (response.ok) {
            const result = await response.json();
            showToast("Tạo công việc thành công!");
            closeModal();
            await refreshTaskList();
            await loadSidebarTags(); // Thêm dòng này để tải lại tags trong sidebar
        } else {
            const errorData = await response.json();
            showToast(errorData.message || "Có lỗi xảy ra khi tạo công việc!", "error");
        }
    } catch (error) {
        console.error('Lỗi khi tạo task:', error);
        showToast("Đã xảy ra lỗi, vui lòng thử lại sau!", "error");
        const submitBtn = document.querySelector('.btn.btn-primary');
        submitBtn.textContent = 'Tạo công việc';
        submitBtn.disabled = false;
    }
}

/**
 * Tải và hiển thị danh sách task
 */
async function fetchAndDisplayTasks() {
    try {
        // Fetch today's tasks
        const todayResponse = await fetch('/api/tasks/today');
        if (!todayResponse.ok) {
            throw new Error(`Error fetching today's tasks: ${todayResponse.statusText}`);
        }

        const todayData = await todayResponse.json();
        console.log("Raw Today Tasks Data:", todayData);

        // Extract tasks array (handle different API response structures)
        const todayTasks = todayData.data || todayData || [];

        console.log("Processed Today Tasks:", todayTasks);

        // Format tasks for display
        const formattedTodayTasks = todayTasks.map(task => ({
            taskId: task.id || task.taskId,
            title: task.title || "Không có tiêu đề",
            priority: task.priority || "medium",
            dueDate: task.dueDate || new Date().toISOString(),
            categoryName: task.categoryName || "Uncategorized",
            status: task.status || "pending"
        }));

        // Try to fetch upcoming tasks if API exists
        let upcomingTasks = [];
        try {
            const upcomingResponse = await fetch('/api/tasks/upcoming');
            if (upcomingResponse.ok) {
                const upcomingData = await upcomingResponse.json();
                upcomingTasks = (upcomingData.data || upcomingData || []).map(task => ({
                    taskId: task.id || task.taskId,
                    title: task.title || "Không có tiêu đề",
                    priority: task.priority || "medium",
                    dueDate: task.dueDate || new Date().toISOString(),
                    categoryName: task.categoryName || "Uncategorized",
                    status: task.status || "pending"
                }));
            }
        } catch (error) {
            console.log("No upcoming tasks API available, using empty array");
        }

        // Fetch completed tasks
        let completedTasks = [];
        try {
            const completedResponse = await fetch('/api/tasks/completed');
            if (completedResponse.ok) {
                const completedData = await completedResponse.json();
                completedTasks = (completedData.data || completedData || []).map(task => ({
                    taskId: task.id || task.taskId,
                    title: task.title || "Không có tiêu đề",
                    priority: task.priority || "medium",
                    dueDate: task.dueDate || new Date().toISOString(),
                    categoryName: task.categoryName || "Uncategorized",
                    status: task.status || "completed"
                }));
            }
        } catch (error) {
            console.log("Error fetching completed tasks:", error);
        }

        // Find and update the task sections
        const todayTasksSection = document.getElementById('today-tasks');
        const upcomingTasksSection = document.getElementById('upcoming-tasks');
        const completedTasksSection = document.getElementById('completed-tasks');

        if (todayTasksSection) {
            updateTaskSection('Công việc hôm nay', formattedTodayTasks, '#today-tasks');
        } else {
            console.error("Could not find today-tasks section");
        }

        if (upcomingTasksSection) {
            updateTaskSection('Sắp tới', upcomingTasks, '#upcoming-tasks');
        } else {
            console.error("Could not find upcoming-tasks section");
        }

        if (completedTasksSection) {
            updateTaskSection('Đã hoàn thành', completedTasks, '#completed-tasks');
        } else {
            console.error("Could not find completed-tasks section");
        }

        // Update dashboard statistics
        await updateDashboardStats();
    } catch (error) {
        console.error('Error fetching tasks:', error);
        showToast("Không thể tải danh sách công việc", "error");
    }
}

/**
 * Tìm kiếm công việc
 */
async function searchTasks(event) {
    const searchTerm = event.target.value.trim();
    if (searchTerm.length === 0) {
        fetchAndDisplayTasks();
        return;
    }

    try {
        const response = await fetch(`/api/tasks/search?q=${encodeURIComponent(searchTerm)}`);
        const results = await response.json();

        const todayResults = results.today || [];
        const upcomingResults = results.upcoming || [];
        const completedResults = results.completed || [];

        updateTaskSection('Kết quả tìm kiếm cho công việc hôm nay', todayResults, '#today-tasks');
        updateTaskSection('Kết quả tìm kiếm cho công việc sắp tới', upcomingResults, '#upcoming-tasks');
        updateTaskSection('Kết quả tìm kiếm cho công việc đã hoàn thành', completedResults, '#completed-tasks');
    } catch (error) {
        console.error('Error searching tasks:', error);
        showToast("Không thể tìm kiếm công việc", "error");
    }
}

// ==================== CÁC HÀM HỖ TRỢ ====================

/**
 * Tải danh sách danh mục vào dropdown
 */
function loadCategories(categories) {
    const categorySelect = document.getElementById("category");
    categorySelect.innerHTML = '<option value="" disabled selected>Chọn danh mục</option>';

    if (!categories || categories.length === 0) {
        // Add a default category if none exists
        const option = document.createElement("option");
        option.value = "default";
        option.textContent = "Mặc định";
        categorySelect.appendChild(option);
        return;
    }

    categories.forEach(cat => {
        const option = document.createElement("option");
        option.value = cat.categoryId;
        option.textContent = cat.name;
        categorySelect.appendChild(option);
    });
}

/**
 * Tải danh sách gợi ý tag
 */
function loadTagSuggestions(tags) {
    const tagInput = document.getElementById("tags");
    const oldSuggestions = document.querySelector(".tag-suggestions");
    if (oldSuggestions) oldSuggestions.remove();

    const tagSuggestions = document.createElement("div");
    tagSuggestions.classList.add("tag-suggestions");

    if (!tags || tags.length === 0) {
        const span = document.createElement("span");
        span.classList.add("tag-suggestion-item");
        span.textContent = "Không có thẻ nào";
        tagSuggestions.appendChild(span);
        tagInput.parentNode.appendChild(tagSuggestions);
        return;
    }

    tags.forEach(tag => {
        const span = document.createElement("span");
        span.classList.add("tag-suggestion-item");
        span.textContent = tag.name;
        span.dataset.tagId = tag.tagId;
        span.onclick = () => {
            addTagToSelected(tag.name, tag.tagId);
            tagInput.value = "";
            tagSuggestions.style.display = 'none';
        };
        tagSuggestions.appendChild(span);
    });

    tagInput.parentNode.appendChild(tagSuggestions);
    tagInput.addEventListener('focus', () => tagSuggestions.style.display = 'flex');

    document.addEventListener('click', (e) => {
        if (e.target !== tagInput && !tagSuggestions.contains(e.target)) {
            tagSuggestions.style.display = 'none';
        }
    });

    tagInput.addEventListener('input', () => {
        const value = tagInput.value.toLowerCase();
        tagSuggestions.style.display = 'flex';
        const items = tagSuggestions.querySelectorAll('.tag-suggestion-item');
        items.forEach(item => {
            item.style.display = item.textContent.toLowerCase().includes(value) ? 'block' : 'none';
        });
    });
}

/**
 * Thêm tag đã chọn vào danh sách
 */
function addTagToSelected(tagName, tagId) {
    const selectedTagsContainer = document.querySelector(".selected-tags");

    if ([...selectedTagsContainer.children].some(tag =>
        tag.textContent.trim().replace('×', '') === tagName
    )) return;

    const tagItem = document.createElement("div");
    tagItem.classList.add("tag-item");
    tagItem.textContent = tagName;
    if (tagId) tagItem.dataset.tagId = tagId;

    const removeBtn = document.createElement("span");
    removeBtn.textContent = "×";
    removeBtn.classList.add("tag-remove");
    removeBtn.onclick = () => {
        tagItem.remove();
        updateSelectedTags();
    };

    tagItem.appendChild(removeBtn);
    selectedTagsContainer.appendChild(tagItem);
    updateSelectedTags();
}

/**
 * Cập nhật mảng selectedTags từ các tag đã hiển thị
 */
function updateSelectedTags() {
    selectedTags = [];
    document.querySelectorAll('.selected-tags .tag-item').forEach(tag => {
        selectedTags.push({
            name: tag.textContent.trim().replace('×', ''),
            tagId: tag.dataset.tagId || null
        });
    });
}

/**
 * Cập nhật một section task trong UI
 */
function updateTaskSection(sectionTitle, tasks, sectionSelector) {
    const section = document.querySelector(sectionSelector);
    if (!section) {
        console.error(`Section not found: ${sectionSelector}`);
        return;
    }

    const header = section.querySelector('.section-header h2');
    if (header) header.textContent = sectionTitle;

    let taskList = section.querySelector('.task-list');
    if (!taskList) {
        taskList = document.createElement('ul');
        taskList.className = 'task-list';
        section.appendChild(taskList);
    } else {
        taskList.innerHTML = '';
    }

    if (!tasks || tasks.length === 0) {
        const emptyItem = document.createElement('li');
        emptyItem.textContent = 'Không có công việc nào';
        emptyItem.className = 'empty-message';
        taskList.appendChild(emptyItem);
    } else {
        tasks.forEach(task => {
            taskList.appendChild(createTaskElement(task));
        });
    }
}

/**
 * Tạo phần tử HTML cho một task
 */
function createTaskElement(task) {
    const li = document.createElement('li');
    li.className = 'task-item';
    li.dataset.taskId = task.taskId || task.id; // Xử lý cả 2 trường hợp

    const checkbox = document.createElement('input');
    checkbox.type = 'checkbox';
    checkbox.className = 'task-checkbox';
    checkbox.checked = task.status === 'completed';
    checkbox.addEventListener('change', () => updateTaskStatus(task.taskId || task.id, checkbox.checked));

    const content = document.createElement('div');
    content.className = 'task-content';

    const title = document.createElement('div');
    title.className = 'task-title';
    title.textContent = task.title || 'Không có tiêu đề';

    // Nếu task đã hoàn thành, thêm class để hiển thị gạch ngang
    if (task.status === 'completed') {
        title.classList.add('completed');
    }

    const meta = document.createElement('div');
    meta.className = 'task-meta';

    const due = document.createElement('div');
    due.className = 'task-due';
    due.textContent = formatDueDate(task.dueDate || new Date().toISOString());

    const priority = document.createElement('div');
    priority.className = `task-priority priority-${task.priority || 'medium'}`;
    priority.textContent = getPriorityText(task.priority || 'medium');

    const category = document.createElement('div');
    category.className = 'task-category';
    category.textContent = task.categoryName || 'Không có danh mục';

    meta.appendChild(due);
    meta.appendChild(priority);
    meta.appendChild(category);
    content.appendChild(title);
    content.appendChild(meta);
    li.appendChild(checkbox);
    li.appendChild(content);

    return li;
}

/**
 * Cập nhật trạng thái hoàn thành của task
 */
async function updateTaskStatus(taskId, isCompleted) {
    try {
        const newStatus = isCompleted ? 'completed' : 'in_progress';
        console.log("TaskID: ", taskId)
        const response = await fetch(`/api/tasks/${taskId}/status`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': getCSRFToken()
            },
            body: JSON.stringify({
                status: newStatus
            })
        });

        if (!response.ok) throw new Error('Không thể cập nhật trạng thái');

        const result = await response.json();
        if (!result.success) {
            throw new Error(result.message || 'Cập nhật trạng thái thất bại');
        }

        showToast(`Công việc đã ${isCompleted ? 'hoàn thành' : 'đang thực hiện'}`);

        // Cập nhật UI
        const taskElement = document.querySelector(`[data-task-id="${taskId}"]`);
        if (taskElement) {
            const titleElement = taskElement.querySelector('.task-title');
            if (titleElement) {
                if (isCompleted) {
                    titleElement.classList.add('completed');
                } else {
                    titleElement.classList.remove('completed');
                }
            }

            // Di chuyển task đến section phù hợp
            const taskListContainer = isCompleted
                ? document.querySelector('#completed-tasks .task-list')
                : document.querySelector('#today-tasks .task-list');

            if (taskListContainer) {
                // Xóa thông báo trống nếu có
                const emptyMessage = taskListContainer.querySelector('.empty-message');
                if (emptyMessage) emptyMessage.remove();

                // Di chuyển task
                taskElement.remove();
                taskListContainer.appendChild(taskElement);

                // Kiểm tra và thêm thông báo trống cho section cũ
                checkAndUpdateEmptySection(isCompleted ? '#today-tasks' : '#completed-tasks');
            }
        }

        await updateDashboardStats();
    } catch (error) {
        console.error('Lỗi khi cập nhật trạng thái task:', error);
        showToast(error.message || 'Không thể cập nhật trạng thái công việc', 'error');

        // Khôi phục trạng thái checkbox nếu có lỗi
        const checkbox = document.querySelector(`[data-task-id="${taskId}"] .task-checkbox`);
        if (checkbox) {
            checkbox.checked = !isCompleted;
        }
    }
}

/**
 * Kiểm tra và cập nhật thông báo khi section trống
 */
function checkAndUpdateEmptySection(sectionSelector) {
    const section = document.querySelector(sectionSelector);
    if (!section) return;

    const taskList = section.querySelector('.task-list');
    if (!taskList) return;

    // Kiểm tra nếu không còn task nào (ngoại trừ thông báo trống)
    const tasks = taskList.querySelectorAll('.task-item');
    if (tasks.length === 0) {
        // Kiểm tra nếu đã có thông báo trống rồi thì không thêm nữa
        const existingEmptyMessage = taskList.querySelector('.empty-message');
        if (!existingEmptyMessage) {
            const emptyItem = document.createElement('li');
            emptyItem.textContent = 'Không có công việc nào';
            emptyItem.className = 'empty-message';
            taskList.appendChild(emptyItem);
        }
    } else {
        // Xóa thông báo trống nếu có task
        const emptyMessage = taskList.querySelector('.empty-message');
        if (emptyMessage) {
            emptyMessage.remove();
        }
    }
}

/**
 * Cập nhật số liệu thống kê trên dashboard
 */
async function updateDashboardStats() {
    try {
        const response = await fetch('/api/tasks/stats');
        if (!response.ok) {
            console.log('Không thể tải số liệu thống kê, sử dụng dữ liệu mặc định');
            // Use default values from DOM if API fails
            return;
        }

        const stats = await response.json();

        // Update dashboard statistics
        document.querySelector('.stat-card:nth-child(1) .number').textContent = stats.total || 0;
        document.querySelector('.stat-card:nth-child(2) .number').textContent = stats.inProgress || 0;
        document.querySelector('.stat-card:nth-child(3) .number').textContent = stats.completed || 0;
        document.querySelector('.stat-card:nth-child(4) .number').textContent = stats.overdue || 0;
    } catch (error) {
        console.error('Lỗi khi cập nhật số liệu thống kê:', error);
    }
}

/**
 * Tải lại danh sách task sau khi thêm mới
 */
async function refreshTaskList() {
    try {
        await fetchAndDisplayTasks();
    } catch (error) {
        console.error('Lỗi khi làm mới danh sách task:', error);
        // Don't reload the page automatically, just show an error
        showToast('Không thể làm mới danh sách công việc', 'error');
    }
}

/**
 * Tải lại danh sách tag trong sidebar
 */
async function loadSidebarTags() {
    try {
        // Gọi API để lấy danh sách tag đã cập nhật
        const response = await fetch('/api/tags');
        if (!response.ok) throw new Error('Không thể tải danh sách thẻ');

        const tags = await response.json();

        // Cập nhật danh sách tag trong sidebar
        const tagList = document.querySelector('.tag-list');
        if (tagList) {
            tagList.innerHTML = ''; // Xóa danh sách hiện tại

            if (tags.length === 0) {
                tagList.innerHTML = '<li class="empty-item">Không có thẻ nào</li>';
            } else {
                tags.forEach(tag => {
                    const li = document.createElement('li');
                    li.innerHTML = `
                        <a href="#" data-tag-id="${tag.tagId}">
                            <span class="tag-color" style="background-color: ${tag.color || '#3498db'}"></span>
                            <span class="tag-name">${tag.name}</span>
                            <span class="tag-count">${tag.count || 0}</span>
                        </a>
                    `;
                    tagList.appendChild(li);
                });
            }
        }
    } catch (error) {
        console.error('Lỗi khi tải danh sách thẻ:', error);
    }
}

// ==================== CÁC HÀM TIỆN ÍCH ====================

/**
 * Đóng modal thêm task
 */
function closeModal() {
    document.getElementById("modalAddTask").style.display = "none";
    resetTaskForm();
}

/**
 * Reset form thêm task về trạng thái ban đầu
 */
function resetTaskForm() {
    document.getElementById('title').value = '';
    document.getElementById('description').value = '';
    document.getElementById('category').selectedIndex = 0;
    document.getElementById('priority').selectedIndex = 1;
    document.getElementById('status').selectedIndex = 0;
    document.querySelector('.selected-tags').innerHTML = '';
    selectedTags = [];
    document.querySelector('.subtasks-container').innerHTML = '';
}

/**
 * Hiển thị thông báo toast
 */
function showToast(message, type = 'success') {
    const toastContainer = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.textContent = message;
    toastContainer.appendChild(toast);

    setTimeout(() => toast.classList.add('show'), 10);
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

/**
 * Định dạng hiển thị ngày hạn
 */
function formatDueDate(dateString) {
    try {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) {
            return 'Không xác định';
        }

        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const tomorrow = new Date(today);
        tomorrow.setDate(tomorrow.getDate() + 1);

        if (date.toDateString() === today.toDateString()) {
            return `Hôm nay, ${date.getHours().toString().padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}`;
        } else if (date.toDateString() === tomorrow.toDateString()) {
            return `Ngày mai, ${date.getHours().toString().padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}`;
        } else {
            return `${date.getDate().toString().padStart(2, '0')}/${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getFullYear()}`;
        }
    } catch (error) {
        console.error('Error formatting date:', error);
        return 'Không xác định';
    }
}

/**
 * Chuyển đổi mã ưu tiên thành text hiển thị
 */
function getPriorityText(priority) {
    const priorities = {
        'low': 'Thấp',
        'medium': 'Trung bình',
        'high': 'Cao',
        'urgent': 'Khẩn cấp'
    };
    return priorities[priority] || 'Trung bình';
}

/**
 * Lấy ID người dùng hiện tại
 */
function getUserId() {
    // Trong thực tế, bạn sẽ lấy ID từ session/localStorage/cookies
    return 1; // Tạm thời hardcode
}

/**
 * Lấy CSRF token để bảo vệ form
 */
function getCSRFToken() {
    const tokenMeta = document.querySelector('meta[name="csrf-token"]');
    return tokenMeta ? tokenMeta.getAttribute('content') : '';
}

/**
 * Debounce function để giới hạn API calls khi tìm kiếm
 */
function debounce(func, timeout = 300) {
    let timer;
    return (...args) => {
        clearTimeout(timer);
        timer = setTimeout(() => { func.apply(this, args); }, timeout);
    };
}

/**
 * Thêm công việc nhỏ (subtask)
 */
function AddTask() {
    const container = document.querySelector(".subtasks-container");
    const div = document.createElement("div");
    div.classList.add("subtask-item");

    const input = document.createElement("input");
    input.type = "text";
    input.placeholder = "Nhập công việc nhỏ";

    const btn = document.createElement("button");
    btn.classList.add("subtask-remove");
    btn.textContent = "×";
    btn.onclick = (e) => {
        e.preventDefault();
        div.remove();
    };

    div.appendChild(input);
    div.appendChild(btn);
    container.appendChild(div);
    input.focus();
}  




