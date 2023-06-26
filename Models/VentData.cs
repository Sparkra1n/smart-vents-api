namespace smart_vents_api.Models;

public class VentData
{
    public int Id { get; set; }

    public double Temp { get; set; }

    public bool IsOccupied { get; set; }

    // public VentData(int id, double temp, bool isOccupied) 
    // {
    //     this.Id = id;
    //     this.Temp = temp;
    //     this.IsOccupied = isOccupied;
    // }
}