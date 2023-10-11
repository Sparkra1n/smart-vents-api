namespace smart_vents_api.Models;

public enum VentState
{
    open,
    close,
    stay
}

public class VentData
{
    public string Timestamp { get; set; }
    public double Temp { get; set; }
    public bool IsOccupied { get; set; }
}

public class VentSummary
{
    public string Id { get; set; }
    public double? TargetTemp { get; set; }
    public double? Temp { get; set; }
    public bool? IsOccupied { get; set; }
}

public class Vent
{
    public string? Id { get; set; }
    public double TargetTemp { get; set; }
    public List<VentData> History = new();
}
