using System.ComponentModel.DataAnnotations;
using PlanningsApp.Ui.Models;

namespace PlanningsApp.Ui.Validations;

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
