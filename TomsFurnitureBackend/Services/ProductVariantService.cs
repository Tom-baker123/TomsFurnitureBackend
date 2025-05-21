using System;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly TomfurnitureContext _context;

        public ProductVariantService(TomfurnitureContext context)
        {
            _context = context;
        }
       public async Task<ResponseResult> Create(ProductVariantCreateVModel vModel)
        {
            //var entity = model.ToEntity();
            //_context.ProductVariants.Add(entity);
            //await _context.SaveChangesAsync();
            //return entity.Id;
            return null;
        }

    }
}
