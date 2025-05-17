using PlanningsApp.Ui.Models.PlanningModels;

namespace PlanningsApp.Ui.Services.Interfaces;

public interface IPlanningService
{
    Task<DepartmentPlanning> GetPlanningAsync(
        string departmentName,
        int selectedYear,
        int selectedWeek
    );
    Task<DepartmentPlanning> CreatePlanningAsync(DepartmentPlanning planning);
    Task UpdatePlanningAsync(DepartmentPlanning planning);
}
