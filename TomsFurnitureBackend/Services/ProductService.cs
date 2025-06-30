using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Common.Models;
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

        // Hàm validation chung cho sản phẩm và biến thể
        private async Task<ResponseResult?> ValidateProductAndVariantsAsync(object model, bool isUpdate = false, int? excludeId = null)
        {
            if (model == null)
            {
                return new ErrorResponseResult("Product data is required.");
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
                    return new ErrorResponseResult("Product name is required.");
                }

                // Kiểm tra tên sản phẩm trùng lặp, loại trừ ID hiện tại trong trường hợp cập nhật
                int excludeIdValidation = isUpdate ? excludeId ?? 0 : 0;
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == createModel.ProductName && (excludeIdValidation == 0 || p.Id != excludeIdValidation));
                if (existingProduct != null)
                {
                    return new ErrorResponseResult("Product name already exists.");
                }

                // Kiểm tra khóa ngoại với kiểm tra null rõ ràng
                if (createModel.BrandId.HasValue && !await _context.Brands.AnyAsync(b => b.Id == createModel.BrandId.Value))
                {
                    return new ErrorResponseResult($"Brand with ID {createModel.BrandId} does not exist.");
                }
                if (createModel.CategoryId.HasValue && !await _context.Categories.AnyAsync(c => c.Id == createModel.CategoryId.Value))
                {
                    return new ErrorResponseResult($"Category with ID {createModel.CategoryId} does not exist.");
                }
                if (createModel.CountriesId.HasValue && !await _context.Countries.AnyAsync(c => c.Id == createModel.CountriesId.Value))
                {
                    return new ErrorResponseResult($"Country with ID {createModel.CountriesId} does not exist.");
                }
                if (createModel.SupplierId.HasValue && !await _context.Suppliers.AnyAsync(s => s.Id == createModel.SupplierId.Value))
                {
                    return new ErrorResponseResult($"Supplier with ID {createModel.SupplierId} does not exist.");
                }

                // Validation cho biến thể trong ProductCreateVModel
                if (createModel is ProductCreateVModel create)
                {
                    foreach (var variant in create.ProductVariants)
                    {
                        if (!await _context.Colors.AnyAsync(c => c.Id == variant.ColorId))
                        {
                            return new ErrorResponseResult($"Color with ID {variant.ColorId} does not exist.");
                        }
                        if (!await _context.Sizes.AnyAsync(s => s.Id == variant.SizeId))
                        {
                            return new ErrorResponseResult($"Size with ID {variant.SizeId} does not exist.");
                        }
                        if (!await _context.Materials.AnyAsync(m => m.Id == variant.MaterialId))
                        {
                            return new ErrorResponseResult($"Material with ID {variant.MaterialId} does not exist.");
                        }
                        if (!await _context.Units.AnyAsync(u => u.Id == variant.UnitId))
                        {
                            return new ErrorResponseResult($"Unit with ID {variant.UnitId} does not exist.");
                        }
                        if (variant.OriginalPrice < 0)
                        {
                            return new ErrorResponseResult("Original price cannot be negative.");
                        }
                        if (variant.DiscountedPrice.HasValue && variant.DiscountedPrice.Value < 0)
                        {
                            return new ErrorResponseResult("Discounted price cannot be negative.");
                        }
                        if (variant.StockQty < 0)
                        {
                            return new ErrorResponseResult("Stock quantity cannot be negative.");
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
                                return new ErrorResponseResult($"Product variant with ID {variant.Id} not found.");
                            }
                            if (existingVariant.ProductId != update.Id)
                            {
                                return new ErrorResponseResult($"Product variant with ID {variant.Id} does not belong to product with ID {update.Id}.");
                            }
                        }
                        // Kiểm tra khóa ngoại
                        if (!await _context.Colors.AnyAsync(c => c.Id == variant.ColorId))
                        {
                            return new ErrorResponseResult($"Color with ID {variant.ColorId} does not exist.");
                        }
                        if (!await _context.Sizes.AnyAsync(s => s.Id == variant.SizeId))
                        {
                            return new ErrorResponseResult($"Size with ID {variant.SizeId} does not exist.");
                        }
                        if (!await _context.Materials.AnyAsync(m => m.Id == variant.MaterialId))
                        {
                            return new ErrorResponseResult($"Material with ID {variant.MaterialId} does not exist.");
                        }
                        if (!await _context.Units.AnyAsync(u => u.Id == variant.UnitId))
                        {
                            return new ErrorResponseResult($"Unit with ID {variant.UnitId} does not exist.");
                        }
                        if (variant.OriginalPrice < 0)
                        {
                            return new ErrorResponseResult("Original price cannot be negative.");
                        }
                        if (variant.DiscountedPrice.HasValue && variant.DiscountedPrice.Value < 0)
                        {
                            return new ErrorResponseResult("Discounted price cannot be negative.");
                        }
                        if (variant.StockQty < 0)
                        {
                            return new ErrorResponseResult("Stock quantity cannot be negative.");
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
                    return new ErrorResponseResult($"Product with ID {updateModel.Id} not found.");
                }
            }

            // Validation cho biến thể đơn lẻ
            if (variantModel != null)
            {
                var variant = await _context.ProductVariants.FirstOrDefaultAsync(pv => pv.Id == variantModel.Id);
                if (variant == null)
                {
                    return new ErrorResponseResult($"Product variant with ID {variantModel.Id} not found.");
                }
                if (!await _context.Colors.AnyAsync(c => c.Id == variantModel.ColorId))
                {
                    return new ErrorResponseResult($"Color with ID {variantModel.ColorId} does not exist.");
                }
                if (!await _context.Sizes.AnyAsync(s => s.Id == variantModel.SizeId))
                {
                    return new ErrorResponseResult($"Size with ID {variantModel.SizeId} does not exist.");
                }
                if (!await _context.Materials.AnyAsync(m => m.Id == variantModel.MaterialId))
                {
                    return new ErrorResponseResult($"Material with ID {variantModel.MaterialId} does not exist.");
                }
                if (!await _context.Units.AnyAsync(u => u.Id == variantModel.UnitId))
                {
                    return new ErrorResponseResult($"Unit with ID {variantModel.UnitId} does not exist.");
                }
                if (variantModel.OriginalPrice < 0)
                {
                    return new ErrorResponseResult("Original price cannot be negative.");
                }
                if (variantModel.DiscountedPrice.HasValue && variantModel.DiscountedPrice.Value < 0)
                {
                    return new ErrorResponseResult("Discounted price cannot be negative.");
                }
                if (variantModel.StockQty < 0)
                {
                    return new ErrorResponseResult("Stock quantity cannot be negative.");
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

                // Chuyển đổi từ ViewModel sang Entity
                var product = model.ToEntity();

                // Thêm Product vào DbContext
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Lấy dữ liệu mới nhất để trả về
                var productVm = await GetByIdAsync(product.Id);
                return new SuccessResponseResult(productVm, "Product and its variants created successfully.");
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Error creating product: {errorMessage}");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Error creating product: {ex.Message}");
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
                    return new ErrorResponseResult($"Product with ID {id} not found.");
                }

                // Kiểm tra ràng buộc trong OrderDetails
                var variantIds = product.ProductVariants.Select(pv => pv.Id).ToList();
                var hasOrderDetails = await _context.OrderDetails
                    .AnyAsync(od => od.ProVarId.HasValue && variantIds.Contains(od.ProVarId.Value));

                if (hasOrderDetails)
                {
                    return new ErrorResponseResult("Cannot delete product because it is used in orders.");
                }

                // Kiểm tra ràng buộc trong Cart thông qua ProductVariants
                var hasCarts = await _context.Carts
                    .AnyAsync(c => variantIds.Contains(c.ProVarId));

                if (hasCarts)
                {
                    return new ErrorResponseResult("Cannot delete product because it is referenced in the cart table.");
                }

                // Kiểm tra ràng buộc trong ProductReviews
                if (product.ProductReviews.Any())
                {
                    return new ErrorResponseResult("Cannot delete product because it is referenced in the ProductReviews table.");
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

                return new SuccessResponseResult(null, "Product deleted successfully.");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Error deleting product: {errorMessage}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ErrorResponseResult($"Error deleting product: {ex.Message}");
            }
        }

        // Lấy tất cả sản phẩm
        public async Task<PaginationModel<ProductGetVModel>> GetAllAsync(ProductFilterParams param)
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
                .Include(p => p.Sliders.Where(s => s.IsActive == true))
                .OrderBy(p => p.Id)
                .AsQueryable();

            var totalCount = await query.CountAsync();

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
                    return new ErrorResponseResult($"Product with ID {model.Id} not found.");
                }

                // Kiểm tra ràng buộc trước khi xóa hoặc cập nhật biến thể
                var variantIdsToRemove = product.ProductVariants
                    .Select(pv => pv.Id)
                    .Except(model.ProductVariants.Where(v => v.Id > 0).Select(v => v.Id))
                    .ToList();
                if (variantIdsToRemove.Any())
                {
                    var hasOrderDetails = await _context.OrderDetails
                        .AnyAsync(od => od.ProVarId.HasValue && variantIdsToRemove.Contains(od.ProVarId.Value));
                    if (hasOrderDetails)
                    {
                        return new ErrorResponseResult("Cannot delete product variants because they are used in orders.");
                    }

                    var hasCarts = await _context.Carts
                        .AnyAsync(c => variantIdsToRemove.Contains(c.ProVarId));
                    if (hasCarts)
                    {
                        return new ErrorResponseResult("Cannot delete product variants because they are referenced in the cart table.");
                    }
                }

                // Cập nhật sản phẩm và biến thể
                product.UpdateEntity(model, _context);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                // Lấy dữ liệu mới nhất để trả về
                var productVm = await GetByIdAsync(product.Id);
                return new SuccessResponseResult(productVm, "Product and variants updated successfully.");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Error updating product: {errorMessage}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ErrorResponseResult($"Error updating product: {ex.Message}");
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
                    return new ErrorResponseResult($"Product variant with ID {model.Id} not found.");
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
                    return new ErrorResponseResult("Product containing the variant not found.");
                }

                var productVm = product.ToGetVModel();
                return new SuccessResponseResult(productVm, "Product variant updated successfully.");
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return new ErrorResponseResult($"Error updating product variant: {errorMessage}");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Error updating product variant: {ex.Message}");
            }
        }
    }
}