using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Common.Models;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using TomsFurnitureBackend.Mappings;
using static TomsFurnitureBackend.VModels.ProductVModel;
using TomsFurnitureBackend.Helpers;

namespace TomsFurnitureBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly TomfurnitureContext _context;

        public ProductService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Hàm validation chung cho sản phẩm và biến thể
        private async Task<ResponseResult?> ValidateProductAndVariantsAsync(object model, bool isUpdate = false, int? excludeId = null)
        {
            if (model == null)
            {
                return new ErrorResponseResult("Dữ liệu sản phẩm là bắt buộc.");
            }

            ProductCreateVModel? createModel = null;
            ProductUpdateVModel? updateModel = null;
            ProductVariantUpdateVModel? variantModel = null;

            if (model is ProductCreateVModel cModel)
            {
                createModel = cModel;
            }
            else if (model is ProductUpdateVModel uModel)
            {
                updateModel = uModel;
                createModel = uModel; // UpdateModel kế thừa CreateModel
            }
            else if (model is ProductVariantUpdateVModel vModel)
            {
                variantModel = vModel;
            }

            // Validation cho sản phẩm
            if (createModel != null)
            {
                // Kiểm tra tên sản phẩm
                if (string.IsNullOrWhiteSpace(createModel.ProductName))
                {
                    return new ErrorResponseResult("Tên sản phẩm là bắt buộc.");
                }

                // Kiểm tra tên sản phẩm trùng lặp, loại trừ ID hiện tại trong trường hợp cập nhật
                int excludeIdValidation = isUpdate ? excludeId ?? 0 : 0;
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == createModel.ProductName && (excludeIdValidation == 0 || p.Id != excludeIdValidation));
                if (existingProduct != null)
                {
                    return new ErrorResponseResult("Tên sản phẩm đã tồn tại.");
                }

                // Kiểm tra khóa ngoại với kiểm tra null rõ ràng
                if (createModel.BrandId.HasValue && !await _context.Brands.AnyAsync(b => b.Id == createModel.BrandId.Value))
                {
                    return new ErrorResponseResult($"Thương hiệu với ID {createModel.BrandId} không tồn tại.");
                }
                if (createModel.CategoryId.HasValue && !await _context.Categories.AnyAsync(c => c.Id == createModel.CategoryId.Value))
                {
                    return new ErrorResponseResult($"Danh mục với ID {createModel.CategoryId} không tồn tại.");
                }
                if (createModel.CountriesId.HasValue && !await _context.Countries.AnyAsync(c => c.Id == createModel.CountriesId.Value))
                {
                    return new ErrorResponseResult($"Quốc gia với ID {createModel.CountriesId} không tồn tại.");
                }
                if (createModel.SupplierId.HasValue && !await _context.Suppliers.AnyAsync(s => s.Id == createModel.SupplierId.Value))
                {
                    return new ErrorResponseResult($"Nhà cung cấp với ID {createModel.SupplierId} không tồn tại.");
                }

                // Validation cho biến thể trong ProductCreateVModel
                if (createModel is ProductCreateVModel create)
                {
                    foreach (var variant in create.ProductVariants)
                    {
                        if (variant.ColorId > 0 && !await _context.Colors.AnyAsync(c => c.Id == variant.ColorId))
                        {
                            return new ErrorResponseResult($"Màu sắc với ID {variant.ColorId} không tồn tại.");
                        }
                        if (variant.SizeId > 0 && !await _context.Sizes.AnyAsync(s => s.Id == variant.SizeId))
                        {
                            return new ErrorResponseResult($"Kích thước với ID {variant.SizeId} không tồn tại.");
                        }
                        if (variant.MaterialId > 0 && !await _context.Materials.AnyAsync(m => m.Id == variant.MaterialId))
                        {
                            return new ErrorResponseResult($"Vật liệu với ID {variant.MaterialId} không tồn tại.");
                        }
                        if (variant.UnitId > 0 && !await _context.Units.AnyAsync(u => u.Id == variant.UnitId))
                        {
                            return new ErrorResponseResult($"Đơn vị với ID {variant.UnitId} không tồn tại.");
                        }
                        if (variant.OriginalPrice < 0)
                        {
                            return new ErrorResponseResult("Giá gốc không thể âm.");
                        }
                        if (variant.DiscountedPrice.HasValue && variant.DiscountedPrice.Value < 0)
                        {
                            return new ErrorResponseResult("Giá khuyến mãi không thể âm.");
                        }
                        if (variant.StockQty < 0)
                        {
                            return new ErrorResponseResult("Số lượng tồn kho không thể âm.");
                        }
                    }
                }

                // Validation cho biến thể trong ProductUpdateVModel
                if (createModel is ProductUpdateVModel update)
                {
                    foreach (var variant in update.ProductVariants)
                    {
                        // Kiểm tra biến thể tồn tại nếu là cập nhật
                        if (variant.Id > 0)
                        {
                            var existingVariant = await _context.ProductVariants.FirstOrDefaultAsync(pv => pv.Id == variant.Id);
                            if (existingVariant == null)
                            {
                                return new ErrorResponseResult($"Biến thể sản phẩm với ID {variant.Id} không tìm thấy.");
                            }
                            if (existingVariant.ProductId != update.Id)
                            {
                                return new ErrorResponseResult($"Biến thể sản phẩm với ID {variant.Id} không thuộc về sản phẩm với ID {update.Id}.");
                            }
                        }
                        // Kiểm tra khóa ngoại
                        if (variant.ColorId > 0 && !await _context.Colors.AnyAsync(c => c.Id == variant.ColorId))
                        {
                            return new ErrorResponseResult($"Màu sắc với ID {variant.ColorId} không tồn tại.");
                        }
                        if (variant.SizeId > 0 && !await _context.Sizes.AnyAsync(s => s.Id == variant.SizeId))
                        {
                            return new ErrorResponseResult($"Kích thước với ID {variant.SizeId} không tồn tại.");
                        }
                        if (variant.MaterialId > 0 && !await _context.Materials.AnyAsync(m => m.Id == variant.MaterialId))
                        {
                            return new ErrorResponseResult($"Vật liệu với ID {variant.MaterialId} không tồn tại.");
                        }
                        if (variant.UnitId > 0 && !await _context.Units.AnyAsync(u => u.Id == variant.UnitId))
                        {
                            return new ErrorResponseResult($"Đơn vị với ID {variant.UnitId} không tồn tại.");
                        }
                        if (variant.OriginalPrice < 0)
                        {
                            return new ErrorResponseResult("Giá gốc không thể âm.");
                        }
                        if (variant.DiscountedPrice.HasValue && variant.DiscountedPrice.Value < 0)
                        {
                            return new ErrorResponseResult("Giá khuyến mãi không thể âm.");
                        }
                        if (variant.StockQty < 0)
                        {
                            return new ErrorResponseResult("Số lượng tồn kho không thể âm.");
                        }
                    }
                }
            }

            // Validation cho cập nhật sản phẩm
            if (updateModel != null && isUpdate)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == updateModel.Id);
                if (product == null)
                {
                    return new ErrorResponseResult($"Sản phẩm với ID {updateModel.Id} không tìm thấy.");
                }
            }

            // Validation cho biến thể đơn lẻ
            if (variantModel != null)
            {
                var variant = await _context.ProductVariants.FirstOrDefaultAsync(pv => pv.Id == variantModel.Id);
                if (variant == null)
                {
                    return new ErrorResponseResult($"Biến thể sản phẩm với ID {variantModel.Id} không tìm thấy.");
                }
                if (!await _context.Colors.AnyAsync(c => c.Id == variantModel.ColorId))
                {
                    return new ErrorResponseResult($"Màu sắc với ID {variantModel.ColorId} không tồn tại.");
                }
                if (!await _context.Sizes.AnyAsync(s => s.Id == variantModel.SizeId))
                {
                    return new ErrorResponseResult($"Kích thước với ID {variantModel.SizeId} không tồn tại.");
                }
                if (!await _context.Materials.AnyAsync(m => m.Id == variantModel.MaterialId))
                {
                    return new ErrorResponseResult($"Vật liệu với ID {variantModel.MaterialId} không tồn tại.");
                }
                if (!await _context.Units.AnyAsync(u => u.Id == variantModel.UnitId))
                {
                    return new ErrorResponseResult($"Đơn vị với ID {variantModel.UnitId} không tồn tại.");
                }
                if (variantModel.OriginalPrice < 0)
                {
                    return new ErrorResponseResult("Giá gốc không thể âm.");
                }
                if (variantModel.DiscountedPrice.HasValue && variantModel.DiscountedPrice.Value < 0)
                {
                    return new ErrorResponseResult("Giá khuyến mãi không thể âm.");
                }
                if (variantModel.StockQty < 0)
                {
                    return new ErrorResponseResult("Số lượng tồn kho không thể âm.");
                }
            }

            return null; // Không có lỗi
        }

        // Tạo sản phẩm mới
        public async Task<ResponseResult> CreateAsync(ProductCreateVModel model)
        {
            try
            {
                // Kiểm tra validation
                var validationResult = await ValidateProductAndVariantsAsync(model);
                if (validationResult != null)
                {
                    return validationResult;
                }

                // Sinh slug duy nhất cho sản phẩm
                var slug = await SlugHelper.GenerateUniqueSlugAsync(
                    model.ProductName,
                    async (s) => await _context.Products.AnyAsync(p => p.Slug == s)
                );

                // Chuyển đổi từ ViewModel sang Entity
                var product = model.ToEntity();
                product.Slug = slug;

                // Thêm Product vào DbContext
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Tạo response với thông tin tóm tắt bao gồm danh sách ID biến thể
                var createResponse = new ProductVModel.ProductCreateResponseVModel
                {
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    Slug = product.Slug,
                    VariantIds = product.ProductVariants.Select(pv => pv.Id).ToList(),
                    TotalVariants = product.ProductVariants.Count,
                    CreatedDate = product.CreatedDate ?? DateTime.UtcNow
                };

                return new SuccessResponseResult(createResponse, "Sản phẩm và các biến thể đã được tạo thành công.");
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
                // Tìm sản phẩm theo ID, bao gồm các ProductVariants, ProductReviews và Sliders
                var product = await _context.Products
                    .Include(p => p.ProductVariants)
                    .Include(p => p.ProductReviews)
                    .Include(p => p.Sliders)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return new ErrorResponseResult($"Sản phẩm với ID {id} không tìm thấy.");
                }

                // Kiểm tra ràng buộc trong OrderDetails
                var variantIds = product.ProductVariants.Select(pv => pv.Id).ToList();
                var hasOrderDetails = await _context.OrderDetails
                    .AnyAsync(od => od.ProVarId.HasValue && variantIds.Contains(od.ProVarId.Value));

                if (hasOrderDetails)
                {
                    return new ErrorResponseResult("Không thể xóa sản phẩm vì nó đã được sử dụng trong đơn hàng.");
                }

                // Kiểm tra ràng buộc trong Cart thông qua ProductVariants
                var hasCarts = await _context.Carts
                    .AnyAsync(c => variantIds.Contains(c.ProVarId));

                if (hasCarts)
                {
                    return new ErrorResponseResult("Không thể xóa sản phẩm vì nó đang được tham chiếu trong giỏ hàng.");
                }

                // Kiểm tra ràng buộc trong ProductReviews
                if (product.ProductReviews.Any())
                {
                    return new ErrorResponseResult("Không thể xóa sản phẩm vì nó đang được tham chiếu trong đánh giá sản phẩm.");
                }

                // Xóa các ProductVariantImage liên quan đến các ProductVariant của sản phẩm
                if (variantIds.Any())
                {
                    var variantImages = await _context.ProductVariantImages
                        .Where(img => img.ProVarId.HasValue && variantIds.Contains(img.ProVarId.Value))
                        .ToListAsync();
                    if (variantImages.Any())
                    {
                        _context.ProductVariantImages.RemoveRange(variantImages);
                    }
                }

                // Xóa các Slider liên quan
                if (product.Sliders.Any())
                {
                    _context.Sliders.RemoveRange(product.Sliders);
                }

                // Xóa các ProductVariant
                if (product.ProductVariants.Any())
                {
                    _context.ProductVariants.RemoveRange(product.ProductVariants);
                }

                // Xóa sản phẩm
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return new SuccessResponseResult(null, "Sản phẩm đã được xóa thành công.");
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
        public async Task<PaginationModel<ProductGetVModel>> GetAllAsync(ProductFilterParams param)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Countries)
                    .Include(p => p.Supplier)
                    .Include(p => p.ProductVariants).ThenInclude(pv => pv.Color)
                    .Include(p => p.ProductVariants).ThenInclude(pv => pv.Size)
                    .Include(p => p.ProductVariants).ThenInclude(pv => pv.Material)
                    .Include(p => p.ProductVariants).ThenInclude(pv => pv.Unit)
                    .Include(p => p.ProductVariants).ThenInclude(pv => pv.ProductVariantImages)
                    .Include(p => p.Sliders.Where(s => s.IsActive == true))
                    .AsQueryable();

                // Tìm kiếm theo tên sản phẩm
                if (!string.IsNullOrWhiteSpace(param.ProductName))
                {
                    query = query.Where(p => p.ProductName.ToLower().Contains(param.ProductName.ToLower()));
                }

                // Tìm kiếm chung (giữ nguyên logic cũ)
                if (!string.IsNullOrWhiteSpace(param.Search))
                {
                    query = query.Where(p => p.ProductName.ToLower().Contains(param.Search.ToLower()) ||
                                             p.Category.CategoryName.ToLower().Contains(param.Search.ToLower()) ||
                                             p.ProductVariants.Any(pv => pv.Color.ColorName.ToLower().Contains(param.Search.ToLower())));
                }

                // Lọc theo danh mục (dựa trên tên danh mục)
                if (param.CategoryNames != null && param.CategoryNames.Any())
                {
                    var categoryIds = await _context.Categories
                        .Where(c => param.CategoryNames.Contains(c.CategoryName))
                        .Select(c => c.Id)
                        .ToListAsync();
                    if (categoryIds.Any())
                    {
                        query = query.Where(p => categoryIds.Contains(p.CategoryId.Value));
                    }
                    else
                    {
                        // Nếu không tìm thấy danh mục nào khớp, trả về rỗng
                        query = query.Where(p => false);
                    }
                }

                // Lọc theo thương hiệu (dựa trên tên thương hiệu)
                if (param.BrandNames != null && param.BrandNames.Any())
                {
                    var brandIds = await _context.Brands
                        .Where(b => param.BrandNames.Contains(b.BrandName))
                        .Select(b => b.Id)
                        .ToListAsync();
                    if (brandIds.Any())
                    {
                        query = query.Where(p => brandIds.Contains(p.BrandId.Value));
                    }
                    else
                    {
                        query = query.Where(p => false);
                    }
                }

                // Lọc theo xuất xứ (dựa trên tên quốc gia)
                if (param.CountryNames != null && param.CountryNames.Any())
                {
                    var countryIds = await _context.Countries
                        .Where(c => param.CountryNames.Contains(c.CountryName))
                        .Select(c => c.Id)
                        .ToListAsync();
                    if (countryIds.Any())
                    {
                        query = query.Where(p => countryIds.Contains(p.CountriesId.Value));
                    }
                    else
                    {
                        query = query.Where(p => false);
                    }
                }

                // Lọc theo màu sắc (dựa trên tên màu)
                if (param.ColorNames != null && param.ColorNames.Any())
                {
                    var colorIds = await _context.Colors
                        .Where(c => param.ColorNames.Contains(c.ColorName))
                        .Select(c => c.Id)
                        .ToListAsync();
                    if (colorIds.Any())
                    {
                        query = query.Where(p => p.ProductVariants.Any(pv => colorIds.Contains(pv.ColorId ?? 0)));
                    }
                    else
                    {
                        query = query.Where(p => false);
                    }
                }

                // Lọc theo vật liệu (dựa trên tên vật liệu)
                if (param.MaterialNames != null && param.MaterialNames.Any())
                {
                    var materialIds = await _context.Materials
                        .Where(m => param.MaterialNames.Contains(m.MaterialName))
                        .Select(m => m.Id)
                        .ToListAsync();
                    if (materialIds.Any())
                    {
                        query = query.Where(p => p.ProductVariants.Any(pv => materialIds.Contains(pv.MaterialId ?? 0)));
                    }
                    else
                    {
                        query = query.Where(p => false);
                    }
                }

                // Lọc theo kích thước (dựa trên tên kích thước)
                if (param.SizeNames != null && param.SizeNames.Any())
                {
                    var sizeIds = await _context.Sizes
                        .Where(s => param.SizeNames.Contains(s.SizeName))
                        .Select(s => s.Id)
                        .ToListAsync();
                    if (sizeIds.Any())
                    {
                        query = query.Where(p => p.ProductVariants.Any(pv => sizeIds.Contains(pv.SizeId ?? 0)));
                    }
                    else
                    {
                        query = query.Where(p => false);
                    }
                }

                // Lọc theo khoảng giá (MinPrice và MaxPrice)
                if (param.MinPrice.HasValue || param.MaxPrice.HasValue)
                {
                    // Log giá trị để debug
                    Console.WriteLine($"MinPrice: {param.MinPrice}, MaxPrice: {param.MaxPrice}");
                    query = query.Where(p => p.ProductVariants.Any(pv =>
                        (pv.DiscountedPrice ?? pv.OriginalPrice) >= (param.MinPrice ?? decimal.MinValue) &&
                        (pv.DiscountedPrice ?? pv.OriginalPrice) <= (param.MaxPrice ?? decimal.MaxValue)));
                }

                // Lọc theo trạng thái tồn kho
                // Nếu InStock được truyền (true/false), áp dụng điều điều kiện lọc tồn kho
                if (param.InStock.HasValue)
                {
                    if (param.InStock.Value)
                    {
                        // Lọc sản phẩm có ít nhất một biến thể còn hàng (StockQty > 0)
                        query = query.Where(p => p.ProductVariants.Any(pv => pv.StockQty > 0));
                    }
                    else
                    {
                        // Lọc sản phẩm có tất cả biến thể hết hàng (StockQty == 0)
                        query = query.Where(p => p.ProductVariants.All(pv => pv.StockQty == 0));
                    }
                }
                // Nếu InStock là null, không áp dụng bất kỳ điều kiện lọc tồn kho nào
                // Logic này cho phép trả về tất cả sản phẩm (còn hàng và hết hàng) để hỗ trợ hiển thị trên trang admin

                // Sắp xếp theo ID (giữ nguyên logic cũ)
                var sortBy = param.SortBy?.ToLower();
                var sortOrder = param.SortOrder?.ToLower();
                bool isDescending = sortOrder == "desc";

                switch (sortBy)
                {
                    case "productname":
                        query = isDescending
                            ? query.OrderByDescending(p => p.ProductName)
                            : query.OrderBy(p => p.ProductName);
                        break;
                    case "originalprice":
                        query = isDescending
                            ? query.OrderByDescending(p => p.ProductVariants.Any() ? p.ProductVariants.Min(pv => pv.OriginalPrice) : 0)
                            : query.OrderBy(p => p.ProductVariants.Any() ? p.ProductVariants.Min(pv => pv.OriginalPrice) : 0);
                        break;
                    case "stockqty":
                        query = isDescending
                            ? query.OrderByDescending(p => p.ProductVariants.Any() ? p.ProductVariants.Sum(pv => pv.StockQty) : 0)
                            : query.OrderBy(p => p.ProductVariants.Any() ? p.ProductVariants.Sum(pv => pv.StockQty) : 0);
                        break;
                    case "createddate":
                        query = isDescending
                            ? query.OrderByDescending(p => p.CreatedDate)
                            : query.OrderBy(p => p.CreatedDate);
                        break;
                    case "updateddate":
                        query = isDescending
                            ? query.OrderByDescending(p => p.UpdatedDate)
                            : query.OrderBy(p => p.UpdatedDate);
                        break;
                    default:
                        // Mặc định sắp xếp theo ID tăng dần nếu SortBy không hợp lệ
                        query = query.OrderBy(p => p.Id);
                        break;
                }

                // Đếm tổng số bản ghi
                var totalCount = await query.CountAsync();

                // Phân trang
                var pagedData = await query
                    .Skip((param.PageNumber - 1) * param.PageSize)
                    .Take(param.PageSize)
                    .ToListAsync();

                var result = new PaginationModel<ProductGetVModel>
                {
                    Items = pagedData.Select(ProductMapping.ToGetVModel).ToList(),
                    TotalCount = totalCount,
                    PageNumber = param.PageNumber,
                    PageSize = param.PageSize
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving products: {ex.Message}", ex);
            }
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
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.ProductVariantImages)
                .Include(p => p.Sliders.Where(s => s.IsActive == true))
                .FirstOrDefaultAsync(p => p.Id == id);

            return product?.ToGetVModel();
        }
        
        // Thêm phương thức GetVariantByIdAsync để lấy thông tin biến thể sản phẩm
        public async Task<ProductVariantGetVModel?> GetVariantByIdAsync(int variantId)
        {
            try
            {
                // Tìm biến thể sản phẩm theo ID, bao gồm các thông tin liên quan
                var variant = await _context.ProductVariants
                    .Include(pv => pv.Color)
                    .Include(pv => pv.Size)
                    .Include(pv => pv.Material)
                    .Include(pv => pv.Unit)
                    .FirstOrDefaultAsync(pv => pv.Id == variantId);

                if (variant == null)
                {
                    return null; // Không tìm thấy biến thể
                }

                // Chuyển đổi sang ProductVariantGetVModel
                return new ProductVModel.ProductVariantGetVModel
                {
                    Id = variant.Id,
                    OriginalPrice = variant.OriginalPrice,
                    DiscountedPrice = variant.DiscountedPrice,
                    StockQty = variant.StockQty,
                    ColorId = variant.ColorId ?? 0,
                    ColorName = variant.Color?.ColorName,
                    SizeId = variant.SizeId ?? 0,
                    SizeName = variant.Size?.SizeName,
                    MaterialId = variant.MaterialId ?? 0,
                    MaterialName = variant.Material?.MaterialName,
                    UnitId = variant.UnitId ?? 0,
                    UnitName = variant.Unit?.UnitName
                };
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                throw new Exception($"Error retrieving product variant with ID {variantId}: {ex.Message}", ex);
            }
        }

        // Lấy biến thể sản phẩm theo thuộc tính
        // Triển khai phương thức kiểm tra biến thể theo mã/tên sản phẩm và thuộc tính
        public async Task<ResponseResult> GetVariantIdByAttributesAsync(string productIdentifier, int colorId, int sizeId, int materialId)
        {
            try
            {
                // Bước 1: Kiểm tra tham số đầu vào
                if (string.IsNullOrWhiteSpace(productIdentifier))
                {
                    return new ErrorResponseResult("Mã định danh sản phẩm (ID hoặc tên) là bắt buộc.");
                }
                if (colorId <= 0)
                {
                    return new ErrorResponseResult("ID màu sắc hợp lệ là bắt buộc.");
                }
                if (sizeId <= 0)
                {
                    return new ErrorResponseResult("ID kích thước hợp lệ là bắt buộc.");
                }
                if (materialId <= 0)
                {
                    return new ErrorResponseResult("ID vật liệu hợp lệ là bắt buộc.");
                }

                // Bước 2: Kiểm tra sự tồn tại của các thuộc tính
                if (!await _context.Colors.AnyAsync(c => c.Id == colorId))
                {
                    return new ErrorResponseResult($"Màu sắc với ID {colorId} không tồn tại.");
                }
                if (!await _context.Sizes.AnyAsync(s => s.Id == sizeId))
                {
                    return new ErrorResponseResult($"Kích thước với ID {sizeId} không tồn tại.");
                }
                if (!await _context.Materials.AnyAsync(m => m.Id == materialId))
                {
                    return new ErrorResponseResult($"Vật liệu với ID {materialId} không tồn tại.");
                }

                // Bước 3: Tìm sản phẩm theo mã (ID) hoặc tên
                Product? product = null;
                if (int.TryParse(productIdentifier, out int productId))
                {
                    // Nếu productIdentifier là số, tìm theo ID
                    product = await _context.Products
                        .Include(p => p.ProductVariants)
                        .FirstOrDefaultAsync(p => p.Id == productId);
                }
                else
                {
                    // Nếu productIdentifier là chuỗi, tìm theo tên
                    product = await _context.Products
                        .Include(p => p.ProductVariants)
                        .FirstOrDefaultAsync(p => p.ProductName.ToLower() == productIdentifier.ToLower());
                }

                if (product == null)
                {
                    return new ErrorResponseResult($"Sản phẩm với mã định danh '{productIdentifier}' không tìm thấy.");
                }

                // Bước 4: Tìm biến thể phù hợp với các thuộc tính
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv =>
                        pv.ProductId == product.Id &&
                        pv.ColorId == colorId &&
                        pv.SizeId == sizeId &&
                        pv.MaterialId == materialId &&
                        pv.IsActive == true);

                if (variant == null)
                {
                    return new ErrorResponseResult("Không tìm thấy biến thể sản phẩm hoạt động nào với các thuộc tính đã chỉ định.");
                }

                // Bước 5: Trả về ID của biến thể
                return new SuccessResponseResult(variant.Id, "Tìm thấy biến thể sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                // Bước 6: Xử lý lỗi và trả về thông báo
                return new ErrorResponseResult($"Lỗi khi lấy biến thể sản phẩm: {ex.Message}");
            }
        }

        // Cập nhật sản phẩm và các biến thể
        public async Task<ResponseResult> UpdateAsync(ProductUpdateVModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra validation
                var validationResult = await ValidateProductAndVariantsAsync(model, true, model.Id);
                if (validationResult != null)
                {
                    return validationResult;
                }

                // Tìm sản phẩm với các biến thể hiện có
                var product = await _context.Products
                    .Include(p => p.ProductVariants)
                    .FirstOrDefaultAsync(p => p.Id == model.Id);
                if (product == null)
                {
                    return new ErrorResponseResult($"Sản phẩm với ID {model.Id} không tìm thấy.");
                }

                // Sinh slug duy nhất cho sản phẩm khi cập nhật
                var slug = await SlugHelper.GenerateUniqueSlugAsync(
                    model.ProductName,
                    async (s) => await _context.Products.AnyAsync(p => p.Slug == s && p.Id != model.Id)
                );

                // Cập nhật sản phẩm và biến thể
                product.Slug = slug;
                product.UpdateEntity(model, _context);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                // Lấy dữ liệu mới nhất để trả về
                var productVm = await GetByIdAsync(product.Id);
                return new SuccessResponseResult(productVm, "Sản phẩm và biến thể đã được cập nhật thành công.");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Lỗi khi cập nhật sản phẩm: {errorMessage}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ErrorResponseResult($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
            }
        }

        // Cập nhật biến thể sản phẩm
        public async Task<ResponseResult> UpdateVariantAsync(ProductVModel.ProductVariantUpdateVModel model)
        {
            try
            {
                // Kiểm tra validation
                var validationResult = await ValidateProductAndVariantsAsync(model);
                if (validationResult != null)
                {
                    return validationResult;
                }

                // Cập nhật biến thể
                var variant = await _context.ProductVariants
                    .Include(pv => pv.Color)
                    .Include(pv => pv.Size)
                    .Include(pv => pv.Material)
                    .Include(pv => pv.Unit)
                    .FirstOrDefaultAsync(pv => pv.Id == model.Id);

                if (variant == null)
                {
                    return new ErrorResponseResult($"Biến thể sản phẩm với ID {model.Id} không tìm thấy.");
                }

                variant.UpdateVariantEntity(model);
                await _context.SaveChangesAsync();

                // Lấy dữ liệu sản phẩm mới nhất để trả về
                var product = await _context.Products
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Color)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Size)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Material)
                    .Include(p => p.ProductVariants)
                        .ThenInclude(pv => pv.Unit)
                    .FirstOrDefaultAsync(p => p.Id == variant.ProductId);

                if (product == null)
                {
                    return new ErrorResponseResult("Sản phẩm chứa biến thể không tìm thấy.");
                }

                var productVm = product.ToGetVModel();
                return new SuccessResponseResult(productVm, "Biến thể sản phẩm đã được cập nhật thành công.");
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Lỗi khi cập nhật biến thể sản phẩm: {errorMessage}");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi cập nhật biến thể sản phẩm: {ex.Message}");
            }
        }
        // Thêm phương thức DeleteVariantAsync vào lớp ProductService
        public async Task<ResponseResult> DeleteVariantAsync(int variantId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Tìm biến thể sản phẩm theo ID
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.Id == variantId);
                if (variant == null)
                {
                    return new ErrorResponseResult($"Biến thể sản phẩm với ID {variantId} không tìm thấy.");
                }

                // Kiểm tra ràng buộc trong OrderDetails
                var hasOrderDetails = await _context.OrderDetails
                    .AnyAsync(od => od.ProVarId.HasValue && od.ProVarId.Value == variantId);
                if (hasOrderDetails)
                {
                    return new ErrorResponseResult("Không thể xóa biến thể sản phẩm vì nó đã được sử dụng trong đơn hàng.");
                }

                // Kiểm tra ràng buộc trong Cart
                var hasCarts = await _context.Carts
                    .AnyAsync(c => c.ProVarId == variantId);
                if (hasCarts)
                {
                    return new ErrorResponseResult("Không thể xóa biến thể sản phẩm vì nó đang được tham chiếu trong giỏ hàng.");
                }

                // Xóa biến thể
                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return new SuccessResponseResult(null, "Biến thể sản phẩm đã được xóa thành công.");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Lỗi khi xóa biến thể sản phẩm: {errorMessage}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ErrorResponseResult($"Lỗi khi xóa biến thể sản phẩm: {ex.Message}");
            }
        }
    }
}