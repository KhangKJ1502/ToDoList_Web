/**
 * category.js - JavaScript cho việc quản lý danh mục
 */

// Document ready event
document.addEventListener('DOMContentLoaded', () => {
    // Load categories khi trang được tải
    loadAndDisplayCategories();

    // Gắn các event cho nút và modal của Category
    setupCategoryEvents();
});

// Thiết lập các sự kiện cho Category
function setupCategoryEvents() {
    // Sự kiện mở modal thêm danh mục
    document.querySelector('#btnAddCategory').addEventListener('click', openModalAddCategory);

    // Sự kiện đóng các modal
    document.querySelector('#btnCancelAddCategory').addEventListener('click', () => closeModalCategory('modalAddCategory'));
    document.querySelector('#btnCancelUpdateCategory').addEventListener('click', () => closeModalCategory('modalUpdateCategory'));

    // Sự kiện submit form thêm danh mục
    document.querySelector('#btnSubmitAddCategory').addEventListener('click', addCategory);

    // Sự kiện submit form cập nhật danh mục
    document.querySelector('#btnSubmitUpdateCategory').addEventListener('click', submitCategory);

    // Sự kiện đóng modal khi click ngoài modal
    setupOutsideClickHandlers();
}

// Thiết lập sự kiện click bên ngoài modal để đóng modal
function setupOutsideClickHandlers() {
    window.addEventListener('click', function (event) {
        let modalAddCategory = document.getElementById("modalAddCategory");
        let modalUpdateCategory = document.getElementById("modalUpdateCategory");

        if (event.target === modalAddCategory) {
            modalAddCategory.style.display = "none";
        }

        if (event.target === modalUpdateCategory) {
            modalUpdateCategory.style.display = "none";
        }
    });
}

// Hiển thị modal thêm danh mục
function openModalAddCategory() {
    document.getElementById("modalAddCategory").style.display = "flex";
}

// Đóng modal theo ID
function closeModalCategory(modalId) {
    document.getElementById(modalId).style.display = "none";
}

// Thêm danh mục mới
async function addCategory() {
    const name = document.getElementById('addCategoryName').value.trim();
    const color = document.getElementById('addCategoryColor').value;

    if (name === '') {
        showToast('Vui lòng nhập tên danh mục.', 'error');
        return;
    }

    try {
        const response = await fetch('/api/add-Categories', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                Name: name,
                Color: color
            })
        });

        const result = await response.json();

        if (result.success) {
            showToast('Đã thêm danh mục thành công!', 'success');
            closeModalCategory('modalAddCategory');

            // Reset form to default
            document.getElementById('addCategoryName').value = '';
            document.getElementById('addCategoryColor').value = '#3498db';

            // Reload categories list
            await loadAndDisplayCategories();
        } else {
            showToast(result.message || 'Không thể thêm danh mục.', 'error');
        }
    } catch (error) {
        console.error('Error adding category:', error);
        showToast('Đã xảy ra lỗi khi thêm danh mục.', 'error');
    }
}

// Tải và hiển thị danh sách danh mục
async function loadAndDisplayCategories() {
    try {
        // Show loading indicator
        document.getElementById('categories-list').innerHTML = '<div class="loading">Đang tải danh mục...</div>';

        const response = await fetch('/api/list-Category', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const result = await response.json();

        if (result.success) {
            renderCategoriesAsList(result.data);
        } else {
            showToast(result.message || 'Không thể tải danh mục.', 'error');
            document.getElementById('categories-list').innerHTML =
                '<div class="error">Không thể tải danh mục.</div>';
        }
    } catch (error) {
        console.error('Error loading categories:', error);
        showToast('Đã xảy ra lỗi khi tải danh mục.', 'error');
        document.getElementById('categories-list').innerHTML =
            '<div class="error">Đã xảy ra lỗi khi tải danh mục.</div>';
    }
}

// Hiển thị danh sách danh mục
function renderCategoriesAsList(categories) {
    const container = document.getElementById('categories-list');

    if (!categories || categories.length === 0) {
        container.innerHTML = '<div class="no-data">Không có danh mục nào</div>';
        return;
    }

    let html = '';
    categories.forEach(category => {
        html += `
        <div class="category-card">
            <div class="category-name">
                <span class="category-color" style="background-color: ${category.color}"></span>
                ${category.name}
            </div>
            <div class="category-actions">
                <button class="edit-btn" onclick="editCategory(${category.categoryId})">Sửa</button>
                <button class="delete-btn" onclick="deleteCategory(${category.categoryId})">Xóa</button>
            </div>
        </div>
        `;
    });

    container.innerHTML = html;
}

// Hiển thị modal chỉnh sửa danh mục
async function editCategory(id) {
    try {
        const res = await fetch(`/api/get-category-by-id?categoryId=${id}`);
        const data = await res.json();

        if (data.success) {
            const cat = data.data;
            document.getElementById("updateCategoryId").value = cat.categoryId;
            document.getElementById("updateCategoryName").value = cat.name;
            document.getElementById("updateCategoryColor").value = cat.color;

            // Show modal
            document.getElementById("modalUpdateCategory").style.display = "flex";
        } else {
            showToast(`Không tìm thấy danh mục: ${id}`, 'error');
        }
    } catch (error) {
        console.error("Error fetching category:", error);
        showToast("Lỗi khi lấy thông tin danh mục.", 'error');
    }
}

// Cập nhật danh mục
async function submitCategory() {
    const id = document.getElementById('updateCategoryId').value;
    const name = document.getElementById('updateCategoryName').value.trim();
    const color = document.getElementById('updateCategoryColor').value;

    if (name === '') {
        showToast('Vui lòng nhập tên danh mục.', 'error');
        return;
    }

    try {
        const response = await fetch('/api/update-categories', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                CategoryId: parseInt(id),
                Name: name,
                Color: color
            })
        });

        const result = await response.json();

        if (result.success) {
            showToast('Đã cập nhật danh mục thành công!', 'success');
            closeModalCategory('modalUpdateCategory');
            await loadAndDisplayCategories();
        } else {
            showToast(result.message || 'Không thể cập nhật danh mục.', 'error');
        }
    } catch (error) {
        console.error('Error updating category:', error);
        showToast('Đã xảy ra lỗi khi cập nhật danh mục.', 'error');
    }
}

// Xóa danh mục
async function deleteCategory(categoryId) {
    // Using SweetAlert2 for confirmation dialog
    const result = await Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa danh mục này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy',
        reverseButtons: true
    });

    // If the user clicks "Xóa"
    if (result.isConfirmed) {
        try {
            // Send DELETE request to API
            const response = await fetch(`/api/delete-categories-by-id?categoryId=${categoryId}`, {
                method: 'DELETE'
            });

            const result = await response.json();

            // Show success or error toast based on response
            if (result.success) {
                showToast('Đã xóa danh mục thành công!', 'success');
                await loadAndDisplayCategories();
            } else {
                showToast(result.message || 'Không thể xóa danh mục.', 'error');
            }
        } catch (error) {
            console.error('Error deleting category:', error);
            showToast('Đã xảy ra lỗi khi xóa danh mục.', 'error');
        }
    }
}