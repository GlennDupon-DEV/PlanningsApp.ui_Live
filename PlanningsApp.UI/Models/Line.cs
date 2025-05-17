using System.Collections.Specialized;

namespace PlanningsApp.UI.Models;

public class Line
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int DepartmentId { get; set; }
    // public required List<LineWorkpost> LineWorkposts { get; set; }
}
