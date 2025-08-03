using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Helpers;

namespace TomsFurnitureBackend.Services
{
    public class UserGuestService : IUserGuestService
    {
        private readonly TomfurnitureContext _context;
        public UserGuestService(TomfurnitureContext context)
        {
            _context = context;
        }

        // Hàm validation chung cho UserGuest
        private static string ValidateUserGuest(string fullName, string phoneNumber, string detailAddress, out string normalizedPhone, string? email = null)
        {
            normalizedPhone = phoneNumber;
            
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(detailAddress))
                return "Full name, phone number, and address are required.";
            
            // Validate và chuẩn hóa số điện thoại bắt buộc
            var phoneValidation = PhoneNumberHelper.ValidateRequiredPhoneNumber(phoneNumber, out normalizedPhone);
            if (!string.IsNullOrEmpty(phoneValidation))
                return phoneValidation;

            // Kiểm tra định dạng email nếu có
            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(email.Trim(), emailRegex))
                    return "Invalid email format.";
            }
            return string.Empty;
        }

        public async Task<ResponseResult> CreateAsync(UserGuestCreateVModel model)
        {
            // Validation chung
            var validationResult = ValidateUserGuest(model.FullName, model.PhoneNumber, model.DetailAddress, out string normalizedPhone, model.Email);
            if (!string.IsNullOrEmpty(validationResult))
                return new ErrorResponseResult(validationResult);
            
            // Cập nhật số điện thoại đã chuẩn hóa
            model.PhoneNumber = normalizedPhone;
            
            // Validation: chỉ kiểm tra trùng email (nếu có)
            if (!string.IsNullOrEmpty(model.Email))
            {
                var existEmail = await _context.UserGuests.AnyAsync(x => x.Email == model.Email);
                if (existEmail)
                    return new ErrorResponseResult("Email already exists.");
            }
            var guest = model.ToEntity();
            _context.UserGuests.Add(guest);
            await _context.SaveChangesAsync();
            // Trả về root response
            var result = new SuccessResponseResult();
            result.Id = guest.Id;
            result.IsSuccess = true;
            result.Message = "Guest created successfully.";
            return result;
        }

        //public async Task<ResponseResult> UpdateAsync(UserGuestUpdateVModel model)
        //{
        //    // Validation chung
        //    var validationResult = ValidateUserGuest(model.FullName, model.PhoneNumber, model.DetailAddress, out string normalizedPhone, model.Email);
        //    if (!string.IsNullOrEmpty(validationResult))
        //        return new ErrorResponseResult(validationResult);
        //    
        //    // Cập nhật số điện thoại đã chuẩn hóa
        //    model.PhoneNumber = normalizedPhone;
        //    
        //    var guest = await _context.UserGuests.FirstOrDefaultAsync(x => x.Id == model.Id);
        //    if (guest == null) return new ErrorResponseResult("Guest not found.");
        //    // Validation: chỉ kiểm tra trùng email (nếu có) với khách khác
        //    if (!string.IsNullOrEmpty(model.Email))
        //    {
        //        var existEmail = await _context.UserGuests.AnyAsync(x => x.Id != model.Id && x.Email == model.Email);
        //        if (existEmail)
        //            return new ErrorResponseResult("Email already exists for another guest.");
        //    }
        //    model.UpdateEntity(guest);
        //    await _context.SaveChangesAsync();

        //    // Trả về root response
        //    var result = new SuccessResponseResult();
        //    result.Id = guest.Id;
        //    result.IsSuccess = true;
        //    result.Message = "Guest updated successfully.";
        //    result.Data = model;
        //    return result;
        //}

        public async Task<List<UserGuestGetVModel>> GetAllAsync()
        {
            var guests = await _context.UserGuests.Where(x => x.CreatedDate != null).ToListAsync();
            return guests.ToGetVModelList();
        }

        public async Task<UserGuestGetVModel?> GetByIdAsync(int id)
        {
            var x = await _context.UserGuests.FirstOrDefaultAsync(x => x.Id == id);
            return x?.ToGetVModel();
        }

        //public async Task<ResponseResult> DeleteAsync(int id)
        //{
        //    var guest = await _context.UserGuests.FirstOrDefaultAsync(x => x.Id == id);
        //    if (guest == null) return new ErrorResponseResult("Guest not found.");
        //    _context.UserGuests.Remove(guest);
        //    await _context.SaveChangesAsync();
        //    return new SuccessResponseResult(null, "Guest deleted successfully.");
        //}

        //// Tìm kiếm khách vãng lai theo số điện thoại hoặc email
        //public async Task<UserGuestGetVModel?> FindByPhoneOrEmailAsync(string? phone, string? email)
        //{
        //    var query = _context.UserGuests.AsQueryable();
        //    if (!string.IsNullOrWhiteSpace(phone))
        //    {
        //        // Chuẩn hóa số điện thoại trước khi tìm kiếm
        //        var normalizedPhone = PhoneNumberHelper.NormalizePhoneNumber(phone);
        //        query = query.Where(x => x.PhoneNumber == normalizedPhone);
        //    }
        //    if (!string.IsNullOrWhiteSpace(email))
        //        query = query.Where(x => x.Email == email);
        //    var guest = await query.FirstOrDefaultAsync();
        //    return guest?.ToGetVModel();
        //}
    }
}
