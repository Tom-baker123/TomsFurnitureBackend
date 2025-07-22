using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class TestExtensions
    {
        // Chuyển từ TestCreateVModel sang Entity Test
        public static Test ToEntity(this TestCreateVModel model)
        {
            // Tạo mới Test entity với các giá trị từ ViewModel
            return new Test
            {
                Name = model.Name,
                CreatedDate = DateTime.Now,
            };
        }
        // Cập nhật thông tin Entity Test từ TestUpdateVModel
        public static void UpdateEntity(this Test entity, TestUpdateVModel model)
        {
            entity.Name = model.Name;
            entity.UpdatedDate = DateTime.Now;
        }
        // Chuyển từ Entity Test sang TestGetVModel
        public static TestGetVModel ToGetVModel(this Test entity)
        {
            {
                return new TestGetVModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    CreatedDate = entity.CreatedDate,
                    UpdatedDate = entity.UpdatedDate,
                };
            }
        }
    }
}
