namespace PlanningsApp.UI.Models;

public class Employee
{
    public required int Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int DepartmentId { get; set; }
    public required int ShiftId { get; set; }
    public string FullName => $"{FirstName} {LastName?[0]}.";
    List<EmployeeAbsence> Absences { get; set; }
}
