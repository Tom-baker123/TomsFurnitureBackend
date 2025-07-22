using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class TestService : ITestService
    {
        // DB sử dụng chung
        private readonly TomfurnitureContext? _context;

        public TestService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation cho thêm 
        private string? ValidateTest(TestCreateVModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return "Tên test không được để trống.";
            if (model.Name.Length > 200)
                return "Tên test không được quá 200 ký tự.";
            return null;
        }
        // Validation cho sửa
        private string? ValidateTest(TestUpdateVModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                return "Tên test không được để trống.";
            if (model.Name.Length > 200)
                return "Tên test không được quá 200 ký tự.";
            return null;
        }

        // [1.] Tạo mới thương hiệu
        public async Task<ResponseResult> CreateTestAsync(TestCreateVModel model)
        {
            string validateResult = ValidateTest(model);
            if (validateResult != null)
                return new ErrorResponseResult(validateResult);

            try
            {
                var existingTest = await _context.Tests
                    .AnyAsync(b => b.Name.ToLower().Trim() == model.Name.ToLower());
                if (existingTest)
                {
                    return new ErrorResponseResult("Đã trùng tên");
                }

                var test = model.ToEntity();

                // B4: Thêm thương hiệu vào DbContext
                _context.Tests.Add(test);
                await _context.SaveChangesAsync();

                // B5: Trả về kết quả thành công
                var testVModel = test.ToGetVModel();
                return new SuccessResponseResult(testVModel, "Đã tạo test thành công!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("Có lỗi xảy ra với test: " + ex.Message);
            }
        }

        // [2.] Xóa Test
        public async Task<ResponseResult> DeleteTestAsync(int id)
        {
            try
            {
                var test = await _context.Tests
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (test == null)
                {
                    return new ErrorResponseResult("Không tìm thấy test.");
                }

                _context.Remove(test);
                await _context.SaveChangesAsync();

                return new SuccessResponseResult("Đã xóa test thành công!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("Có lỗi xảy ra khi xóa test: " + ex.Message);
            }
        }
        
        // [3.] Lấy tất cả danh sách test
        public async Task<List<TestGetVModel>> GetAllTestAsync()
        {
            var tests = await _context.Tests
                .OrderBy(t => t.Id)
                .ToListAsync();
            return tests.Select(b => b.ToGetVModel()).ToList();
        }

        // [4.] Lấy Test theo ID
        public async Task<TestGetVModel?> GetTestByIdAsync(int id)
        {
            var test = await _context.Tests
                .FirstOrDefaultAsync(t => t.Id == id);

            return test?.ToGetVModel();
        }
        // [5.] Cập nhật Test
        public async Task<ResponseResult> UpdateTestAsync(int id, TestUpdateVModel model)
        {
            string validateResult = ValidateTest(model);
            if (validateResult != null)
                return new ErrorResponseResult(validateResult);

            try
            {
                var test = await _context.Tests
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (test == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy test có id là: {id}.");
                }

                var existingTest = await _context.Tests
                    .AnyAsync(t => t.Name.ToLower().Trim() == model.Name.ToLower().Trim() 
                                && t.Id != id);
                if (existingTest)
                {
                    return new ErrorResponseResult("Tên Test đã trùng");
                }

                test.UpdateEntity(model);
                await _context.SaveChangesAsync();

                var testVM = test.ToGetVModel();
                return new SuccessResponseResult(testVM, "Đã cập nhật Test hoàn chỉnh");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Có lỗi xảy ra khi cập nhật tets. {ex.Message}");
            }
        }
    }
}
