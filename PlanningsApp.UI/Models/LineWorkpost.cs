namespace PlanningsApp.UI.Models;

public class LineWorkpost
{
    public int Id { get; set; }

    public int LineId { get; set; }

    public int WorkpostId { get; set; }
    public Workpost WorkPost { get; set; }
}
