namespace PROG6212POE.Models
{
    public static class UserStore
    {
        // Username → (Password, Role)
        public static Dictionary<string, (string Password, string Role)> Users =
            new Dictionary<string, (string Password, string Role)>
            {
                { "lecturer1", ("password123", "Lecturer") },
                { "coordinator1", ("coord123", "Coordinator") },
                { "manager1", ("manager123", "Manager") },
                { "admin", ("admin123", "HR") }
            };
    }
}
