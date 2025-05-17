using System.ComponentModel.DataAnnotations;
using PlanningsApp.UI.Models;

namespace PlanningsApp.UI.Validations;

public class EmployeeAbsenceFormValidation
{
    [Required]
    public Employee Employee { get; set; }

    [Required]
    public Absence Absence { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }

    [StringLength(100, ErrorMessage = "Description must be shorter than 100 characters.")]
    public string Description { get; set; } = "";
}
