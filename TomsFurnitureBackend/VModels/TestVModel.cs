namespace TomsFurnitureBackend.VModels
{
    public class TestCreateVModel
    {
        public string? Name { get; set; }
    }
    public class TestUpdateVModel
    {
        public string? Name { get; set; }
    }
    public class TestGetVModel : TestCreateVModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
