using PlanningsApp.UI.Models.PlanningModels;

namespace PlanningsApp.UI.Services.Interfaces;

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
