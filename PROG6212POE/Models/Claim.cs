using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class Claim
    {
        [Required]
        [Display(Name = "Claim ID")]
        public string ClaimId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Lecturer name is required")]
        [StringLength(50)]
        public string LecturerName { get; set; }

        [Required(ErrorMessage = "Please select a month")]
        [Display(Name = "Salary Month")]
        public string SalaryMonth { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(1, 300, ErrorMessage = "Hours worked must be between 1 and 300")]
        [Display(Name = "Hours Worked")]
        public int HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(50, 2000, ErrorMessage = "Hourly rate must be between 50 and 2000")]
        [Display(Name = "Hourly Rate (R)")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Additional Notes")]
        public string? AdditionalNotes { get; set; }

        [Display(Name = "Supporting Document")]
        public string? DocumentName { get; set; }

        public string Status { get; set; } = "Pending";

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; } = DateTime.Now;
    }
}
