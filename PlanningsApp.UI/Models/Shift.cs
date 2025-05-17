namespace PlanningsApp.Ui.Models;

public class Shift
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required TimeOnly StartTime { get; set; }
    public required TimeOnly EndTime { get; set; }
}
