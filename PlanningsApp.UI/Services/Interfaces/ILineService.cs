using PlanningsApp.Ui.Models;
using PlanningsApp.Ui.Models.PlanningModels;

namespace PlanningsApp.Ui.Services.Interfaces;

public interface ILineService
{
    Task<List<PlanningLine>> GetLinesByDepartmentNameAsync(string departmentName);
}
