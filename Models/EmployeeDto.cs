namespace ReminderTelegramAPI.Models
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string NoKtp { get; set; } = string.Empty;
        public string Dept { get; set; } = string.Empty;
        public bool SudahMCU { get; set; }
    }
}
