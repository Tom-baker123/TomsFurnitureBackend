using OA.Domain.Common.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class UnitService : IUnitService
    {
        private readonly TomfurnitureContext _context;
        public UnitService(TomfurnitureContext context) { 
            _context = context;
        }

        public async Task<ResponseResult> Create(UnitCreateVModel vModel)
        {
            var response = new ResponseResult();
            try
            {
                var unit = UnitMapping.VModelToModel(vModel);
                _context.Units.Add(unit);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(unit,"Thêm đơn vị thành công");
                return response;
            }
            catch (Exception ex) { 
                return new ErrorResponseResult(ex.Message);
            }
        }
    }
}
