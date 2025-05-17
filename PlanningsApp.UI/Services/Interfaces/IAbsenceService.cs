using PlanningsApp.Ui.Models;

namespace PlanningsApp.Ui.Services.Interfaces;

public interface IAbsenceService
{
    Task<List<EmployeeAbsence>> GetEmployeeAbsences();
    Task<EmployeeAbsence> PatchEmployeeAbsence(EmployeeAbsence updatedEmployeeAbsence);
    Task<EmployeeAbsence> PostEmployeeAbsence(EmployeeAbsence newEmployeeAbsence);
    Task<List<Absence>> GetAbsencesAsync();
}
