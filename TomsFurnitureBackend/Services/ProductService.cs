using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;
using System.Linq;

namespace TomsFurnitureBackend.Services
{
    public class ProductService: IProductService
    {
        private readonly TomfurnitureContext _context;

        public ProductService(TomfurnitureContext context)
        {
            _context = context; // Inject DbContext
        }

        // Tạo sản phẩm mới
        public async Task<ResponseResult> CreateAsync(ProductCreateVModel model)
        {
            try
            {
                // Kiểm tra tên sản phẩm đã tồn tại
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == model.ProductName);
                if (existingProduct != null)
                {
                    return new ErrorResponseResult("Tên sản phẩm đã tồn tại.");
                }

                // Chuyển đổi từ ViewModel sang Entity
                var product = model.ToEntity();

                // Thêm vào DbContext
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Chuyển đổi sang ViewModel để trả về
                var productVm = product.ToGetVModel();
                return new SuccessResponseResult(productVm, "Thêm sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi tạo sản phẩm: {ex.Message}");
            }
        }

        // Xóa sản phẩm theo ID
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // Tìm sản phẩm theo ID, bao gồm các ProductVariants
                var product = await _context.Products
                    .Include(p => p.ProductVariants)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy sản phẩm với ID: {id}");
                }

                // Kiểm tra xem ProductVariants có được tham chiếu trong OrderDetails không
                var variantIds = product.ProductVariants.Select(pv => pv.Id).ToList(); // Chuyển thành List<int>
                var hasOrderDetails = await _context.OrderDetails
                    .AnyAsync(od => variantIds.Contains((int)od.ProVarId));

                if (hasOrderDetails)
                {
                    return new ErrorResponseResult("Không thể xóa sản phẩm vì nó đã được sử dụng trong các đơn hàng.");
                }

                // Xóa sản phẩm (ProductVariants, ProductReviews, Sliders sẽ tự động xóa do cascade)
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return new SuccessResponseResult(null, "Xóa sản phẩm thành công");
            }
            catch (DbUpdateException dbEx)
            {
                // Lấy thông tin chi tiết từ InnerException
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Lỗi khi xóa sản phẩm: {errorMessage}");
            }
            catch (Exception ex)
            {
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
                .OrderBy(p => p.ProductName) // Sắp xếp theo tên sản phẩm
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
                .FirstOrDefaultAsync(p => p.Id == id);

            return product?.ToGetVModel();
        }

        // Cập nhật sản phẩm
        public async Task<ResponseResult> UpdateAsync(ProductUpdateVModel model)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (model == null)
                {
                    return new ErrorResponseResult("Dữ liệu sản phẩm không được cung cấp.");
                }

                if (string.IsNullOrWhiteSpace(model.ProductName))
                {
                    return new ErrorResponseResult("Tên sản phẩm không được để trống.");
                }

                // Tìm sản phẩm theo ID
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == model.Id);
                if (product == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy sản phẩm với ID: {model.Id}");
                }

                // Kiểm tra tên sản phẩm đã tồn tại (trừ sản phẩm hiện tại)
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == model.ProductName && p.Id != model.Id);
                if (existingProduct != null)
                {
                    return new ErrorResponseResult("Tên sản phẩm đã tồn tại.");
                }

                // Kiểm tra các khóa ngoại (nếu có)
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
                    return new ErrorResponseResult($"Quốc gia với ID {model.CountriesId} không tồn tại.");
                }
                if (model.SupplierId.HasValue && !await _context.Suppliers.AnyAsync(s => s.Id == model.SupplierId))
                {
                    return new ErrorResponseResult($"Nhà cung cấp với ID {model.SupplierId} không tồn tại.");
                }

                // Cập nhật thông tin sản phẩm
                product.UpdateEntity(model);
                await _context.SaveChangesAsync();

                // Chuyển đổi sang ViewModel để trả về
                var productVm = await GetByIdAsync(product.Id); // Lấy dữ liệu mới nhất bao gồm các quan hệ
                return new SuccessResponseResult(productVm, "Cập nhật sản phẩm thành công");
            }
            catch (DbUpdateException dbEx)
            {
                // Lấy thông tin chi tiết từ InnerException
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
