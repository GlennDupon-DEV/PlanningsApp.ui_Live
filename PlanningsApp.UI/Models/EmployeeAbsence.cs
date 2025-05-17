namespace PlanningsApp.UI.Models;

public class EmployeeAbsence
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int AbsenceId { get; set; }
    public required DateTime? StartDate { get; set; }
    public required DateTime? EndDate { get; set; }

    public string Description { get; set; } = "";
    public required Absence Absence { get; set; }
    public required Employee Employee { get; set; }
}
