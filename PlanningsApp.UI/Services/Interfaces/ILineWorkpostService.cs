using PlanningsApp.Ui.Models.PlanningModels;

namespace PlanningsApp.Ui.Services.Interfaces;

public interface ILineWorkpostService
{
    Task<List<PlanningWorkPost>> GetLineWorkpostsByDepartmentName(string departmentName);
    Task<List<PlanningWorkPost>> GetWorkpostsByLineId(int lineId);
}
