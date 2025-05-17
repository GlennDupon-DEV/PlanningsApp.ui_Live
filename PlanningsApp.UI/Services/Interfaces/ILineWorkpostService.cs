using PlanningsApp.UI.Models.PlanningModels;

namespace PlanningsApp.UI.Services.Interfaces;

public interface ILineWorkpostService
{
    Task<List<PlanningWorkPost>> GetLineWorkpostsByDepartmentName(string departmentName);
    Task<List<PlanningWorkPost>> GetWorkpostsByLineId(int lineId);
}
