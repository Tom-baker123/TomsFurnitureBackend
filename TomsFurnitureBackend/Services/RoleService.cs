using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services
{
    public class RoleService : IRoleService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext qua DI
        public RoleService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation cho phương thức tạo mới vai trò
        public static string ValidateCreate(RoleCreateVModel model)
        {
            // Kiểm tra RoleName không được để trống
            if (string.IsNullOrWhiteSpace(model.RoleName))
            {
                return "Role name is required.";
            }

            // Kiểm tra RoleName không quá 50 ký tự
            if (model.RoleName.Length > 50)
            {
                return "Role name must be less than 50 characters.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức cập nhật vai trò
        public static string ValidateUpdate(RoleUpdateVModel model)
        {
            // Kiểm tra Id hợp lệ
            if (model.Id <= 0)
            {
                return "Invalid role ID.";
            }

            // Áp dụng các validation của Create
            return ValidateCreate(model);
        }

        // [1.] Tạo mới vai trò
        public async Task<ResponseResult> CreateAsync(RoleCreateVModel model)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra RoleName đã tồn tại chưa
                var existingRole = await _context.Roles
                    .AnyAsync(r => r.RoleName.ToLower() == model.RoleName.ToLower());
                if (existingRole)
                {
                    return new ErrorResponseResult("Role name already exists.");
                }

                // B3: Chuyển ViewModel sang Entity
                var role = model.ToEntity();

                // B4: Thêm vai trò vào DbContext
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                // B5: Trả về kết quả thành công
                var roleVM = role.ToGetVModel();
                return new SuccessResponseResult(roleVM, "Role created successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while creating the role: {ex.Message}");
            }
        }

        // [2.] Xóa vai trò
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm vai trò theo ID
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (role == null)
                {
                    return new ErrorResponseResult($"Role not found with ID: {id}.");
                }

                // B2: Kiểm tra xem vai trò có đang được sử dụng trong Users không
                var isUsedInUsers = await _context.Users
                    .AnyAsync(u => u.RoleId == id);
                if (isUsedInUsers)
                {
                    return new ErrorResponseResult("Role cannot be deleted because it is associated with one or more users.");
                }

                // B3: Xóa vai trò
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Role deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while deleting the role: {ex.Message}");
            }
        }

        // [3.] Lấy tất cả vai trò
        public async Task<List<RoleGetVModel>> GetAllAsync()
        {
            // Lấy tất cả vai trò từ database và chuyển thành ViewModel
            var roles = await _context.Roles
                .OrderBy(r => r.RoleName) // Sắp xếp theo RoleName
                .ToListAsync();
            return roles.Select(r => r.ToGetVModel()).ToList();
        }

        // [4.] Lấy vai trò theo ID
        public async Task<RoleGetVModel?> GetByIdAsync(int id)
        {
            // Tìm vai trò theo ID
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id);
            return role?.ToGetVModel();
        }

        // [5.] Cập nhật vai trò
        public async Task<ResponseResult> UpdateAsync(RoleUpdateVModel model)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm vai trò theo ID
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == model.Id);
                if (role == null)
                {
                    return new ErrorResponseResult($"Role not found with ID: {model.Id}.");
                }

                // B3: Kiểm tra RoleName đã tồn tại chưa (ngoại trừ vai trò hiện tại)
                var existingRole = await _context.Roles
                    .AnyAsync(r => r.RoleName.ToLower() == model.RoleName.ToLower() && r.Id != model.Id);
                if (existingRole)
                {
                    return new ErrorResponseResult("Role name already exists.");
                }

                // B4: Cập nhật thông tin vai trò
                role.UpdateEntity(model);

                // B5: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B6: Chuyển Entity thành ViewModel để trả về
                var roleVM = role.ToGetVModel();
                return new SuccessResponseResult(roleVM, "Role updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the role: {ex.Message}");
            }
        }
    }
}