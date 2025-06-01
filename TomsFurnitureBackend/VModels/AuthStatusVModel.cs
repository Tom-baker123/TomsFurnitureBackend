namespace TomsFurnitureBackend.VModels
{
    public class AuthStatusVModel
    {
        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
    }
}