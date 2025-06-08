using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly TomfurnitureContext _context;

        public ProductService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Kiểm tra validation cho Product
        private async Task<ResponseResult?> ValidateProductAsync(ProductCreateVModel model, int? excludeId = null)
        {
            // Kiểm tra tên sản phẩm đã tồn tại
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductName == model.ProductName && (excludeId == null || p.Id != excludeId));
            if (existingProduct != null)
            {
                return new ErrorResponseResult("Tên sản phẩm đã tồn tại.");
            }

            // Kiểm tra các khóa ngoại
            if (model.BrandId.HasValue && !await _context.Brands.AnyAsync(b => b.Id == model.BrandId))
            {
                return new ErrorResponseResult($"Thương hiệu với ID {model.BrandId} không tồn tại.");
            }
            if (model.CategoryId.HasValue && !await _context.Categories.AnyAsync(c => c.Id == model.CategoryId))
            {
                return new ErrorResponseResult($"Danh mục với ID {model.CategoryId} không tồn tại.");
            }
            if (model.CountriesId.HasValue && !await _context.Countries.AnyAsync(c => c.Id == model.CountriesId))
            {
                return new ErrorResponseResult($"xuất xứ với ID {model.CountriesId} không tồn tại.");
            }
            if (model.SupplierId.HasValue && !await _context.Suppliers.AnyAsync(s => s.Id == model.SupplierId))
            {
                return new ErrorResponseResult($"Nhà cung cấp với ID {model.SupplierId} không tồn tại.");
            }

            return null; // Không có lỗi
        }

        // Kiểm tra validation cho ProductVariants
        private async Task<ResponseResult?> ValidateProductVariantsAsync(List<ProductVariantCreateVModel> variants)
        {
            foreach (var variant in variants)
            {
                if (!await _context.Colors.AnyAsync(c => c.Id == variant.ColorId))
                {
                    return new ErrorResponseResult($"Màu với ID {variant.ColorId} không tồn tại.");
                }
                if (!await _context.Sizes.AnyAsync(s => s.Id == variant.SizeId))
                {
                    return new ErrorResponseResult($"Kích thước với ID {variant.SizeId} không tồn tại.");
                }
                if (!await _context.Materials.AnyAsync(m => m.Id == variant.MaterialId))
                {
                    return new ErrorResponseResult($"Chất liệu với ID {variant.MaterialId} không tồn tại.");
                }
                if (!await _context.Units.AnyAsync(u => u.Id == variant.UnitId))
                {
                    return new ErrorResponseResult($"Đơn vị với ID {variant.UnitId} không tồn tại.");
                }
            }
            return null; // Không có lỗi
        }

        // Kiểm tra validation cho Update
        private async Task<ResponseResult?> ValidateUpdateProductAsync(ProductUpdateVModel model)
        {
            if (model == null)
            {
                return new ErrorResponseResult("Dữ liệu sản phẩm không được cung cấp.");
            }

            if (string.IsNullOrWhiteSpace(model.ProductName))
            {
                return new ErrorResponseResult("Tên sản phẩm không được để trống.");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product == null)
            {
                return new ErrorResponseResult($"Không tìm thấy sản phẩm với ID: {model.Id}");
            }

            return null; // Không có lỗi
        }

        // Tạo sản phẩm mới
        public async Task<ResponseResult> CreateAsync(ProductCreateVModel model)
        {
            try
            {
                // Kiểm tra validation
                var productValidationResult = await ValidateProductAsync(model);
                if (productValidationResult != null)
                {
                    return productValidationResult;
                }

                var variantValidationResult = await ValidateProductVariantsAsync(model.ProductVariants);
                if (variantValidationResult != null)
                {
                    return variantValidationResult;
                }

                // Chuyển đổi từ ViewModel sang Entity
                var product = model.ToEntity();

                // Thêm Product vào DbContext
                _context.Products.Add(product);
                await _context.SaveChangesAsync(); // Lưu để sinh Product.Id

                // Gán ProductId và đảm bảo Id của ProductVariant không được gán
                foreach (var variant in product.ProductVariants)
                {
                    variant.ProductId = product.Id;
                    variant.Id = 0; // Đặt Id về 0 để EF bỏ qua và để SQL Server tự sinh
                }

                // Lưu các ProductVariant
                _context.ProductVariants.AddRange(product.ProductVariants);
                await _context.SaveChangesAsync();

                // Lấy dữ liệu mới nhất để trả về
                var productVm = await GetByIdAsync(product.Id);
                return new SuccessResponseResult(productVm, "Thêm sản phẩm và biến thể thành công");
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Lỗi khi tạo sản phẩm: {errorMessage}");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi tạo sản phẩm: {ex.Message}");
            }
        }

        // Xóa sản phẩm theo ID
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Tìm sản phẩm theo ID, bao gồm các ProductVariants
                var product = await _context.Products
                    .Include(p => p.ProductVariants)
                    .Include(p => p.Carts)
                    .Include(p => p.ProductReviews)
                    .Include(p => p.Sliders)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy sản phẩm với ID: {id}");
                }

                // Kiểm tra xem ProductVariants có được tham chiếu trong OrderDetails không
                var variantIds = product.ProductVariants.Select(pv => pv.Id).ToList();
                var hasOrderDetails = await _context.OrderDetails
                    .AnyAsync(od => variantIds.Contains((int)od.ProVarId));

                if (hasOrderDetails)
                {
                    return new ErrorResponseResult("Không thể xóa sản phẩm vì nó đã được sử dụng trong các đơn hàng.");
                }

                // Kiểm tra các ràng buộc
                if (product.Carts.Any())
                {
                    return new ErrorResponseResult($"Cannot delete the product because it is referenced in the cart table.");
                }

                if (product.ProductReviews.Any())
                {
                    return new ErrorResponseResult($"Cannot delete the product because it is referenced in the ProductReviews table.");
                }

                if (product.Sliders.Any())
                {
                    return new ErrorResponseResult($"Cannot delete the product because it is referenced in the Sliders table.");
                }

                // Xóa các ProductVariant thủ công
                if (product.ProductVariants.Any())
                {
                    _context.ProductVariants.RemoveRange(product.ProductVariants);
                }

                // Xóa sản phẩm
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return new SuccessResponseResult(null, "Xóa sản phẩm thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Lỗi khi xóa sản phẩm: {errorMessage}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ErrorResponseResult($"Lỗi khi xóa sản phẩm: {ex.Message}");
            }
        }

        // Lấy tất cả sản phẩm
        public async Task<List<ProductGetVModel>> GetAllAsync()
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Countries)
                .Include(p => p.Supplier)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Size)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Material)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Unit)
                .Include(p => p.Sliders.Where(s => s.IsActive == true))
                .OrderBy(p => p.Id)
                .ToListAsync();

            return products.Select(p => p.ToGetVModel()).ToList();
        }

        // Lấy sản phẩm theo ID
        public async Task<ProductGetVModel?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Countries)
                .Include(p => p.Supplier)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Size)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Material)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Unit)
                .Include(p => p.Sliders.Where(s => s.IsActive == true))
                .FirstOrDefaultAsync(p => p.Id == id);

            return product?.ToGetVModel();
        }

        // Cập nhật sản phẩm
        public async Task<ResponseResult> UpdateAsync(ProductUpdateVModel model)
        {
            try
            {
                // Kiểm tra validation
                var updateValidationResult = await ValidateUpdateProductAsync(model);
                if (updateValidationResult != null)
                {
                    return updateValidationResult;
                }

                var productValidationResult = await ValidateProductAsync(model, model.Id);
                if (productValidationResult != null)
                {
                    return productValidationResult;
                }

                // Cập nhật thông tin sản phẩm
                var product = await _context.Products.FirstAsync(p => p.Id == model.Id); // Đã kiểm tra tồn tại
                product.UpdateEntity(model);
                await _context.SaveChangesAsync();

                // Lấy dữ liệu mới nhất để trả về
                var productVm = await GetByIdAsync(product.Id);
                return new SuccessResponseResult(productVm, "Cập nhật sản phẩm thành công");
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Lỗi khi cập nhật sản phẩm: {errorMessage}");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
            }
        }
    }
}