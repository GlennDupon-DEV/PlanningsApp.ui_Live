using PlanningsApp.UI.Models;
using PlanningsApp.UI.Models.PlanningModels;

namespace PlanningsApp.UI.Services.Interfaces;

public interface ILineService
{
    Task<List<PlanningLine>> GetLinesByDepartmentNameAsync(string departmentName);
}
