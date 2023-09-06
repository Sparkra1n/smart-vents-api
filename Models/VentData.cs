namespace smart_vents_api.Models;

// Basic vent data
public class VentData
{
    public DateTime TimeStamp { get; set; }
    public double Temp { get; set; }
    public bool IsOccupied { get; set; }
}

public class VentSummary
{
    public string Id { get; set; }
    public double Temp { get; set; }
    public bool IsOccupied { get; set; }
}

public class Vent
{
    public string? Id { get; set; }
    public List<VentData> History = new();
}
