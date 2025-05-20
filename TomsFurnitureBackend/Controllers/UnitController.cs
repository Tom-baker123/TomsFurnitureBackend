using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;
        public UnitController(IUnitService unitService) { 
            _unitService = unitService;
        }
        [HttpPost]
        public async Task<ResponseResult> Create(UnitCreateVModel vModel)
        {
            var result = await _unitService.Create(vModel);
            return result;
        }
    }
}
