using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using smart_vents_api.Models;
using Newtonsoft.Json;

namespace smart_vents_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ThermostatController : ControllerBase {
    private static DashboardSettings Settings = new();
    private static readonly List<Vent> Vents = new();
    private readonly ILogger<ThermostatController> Logger;
    public ThermostatController(ILogger<ThermostatController> logger) {
        Logger = logger;
        Settings = new DashboardSettings() {
            IsEnabled = false,
            TargetTemp = 25
        };
    }

    // Vent posts its info and is told whether it should be open or closed
    [HttpPost]
    [Route("UpdateVent")]
    public ActionResult<bool> UpdateVent(VentSummary payload) {
        foreach (var vent in Vents) {
            if (vent.Id == payload.Id) {
                vent.History.Add(new VentData() {
                    TimeStamp = DateTime.UtcNow,
                    Temp = payload.Temp,
                    IsOccupied = payload.IsOccupied
                });

                // Leave everything open if Smart-Vents is disabled
                if (!Settings.IsEnabled) 
                    return true;

                // Close if not occupied
                if (!payload.IsOccupied) 
                    return false;

                // Close when the room reaches the target temp
                return payload.Temp <= Settings.TargetTemp;
            }
        }
        return NotFound();
    }

    [HttpPost]
    [Route("AddVent")]
    public void AddVent(string id) {
        Vents.Add(new Vent() {
            Id = id,
            History = new List<VentData>()   
        });  
    }

    [HttpPost]
    [Route("Settings")]
    public void SetDashboardSettings(DashboardSettings dashboardData) {
        Settings = dashboardData;
    }

    [HttpGet]
    [Route("VentHistory")]
    public ActionResult<List<VentData>> GetVentHistory(string id) {
        foreach (var vent in Vents) {
            if (vent.Id == id)
                return vent.History;
        }
        return NotFound();
    }

    [HttpGet]
    [Route("VentSummaries")]
    public string GetVentSummaries() {
        List<VentSummary> ventSummaries = new();
        foreach (var vent in Vents) {
            ventSummaries.Add(new VentSummary() {
                Id = vent.Id,
                IsOccupied = vent.History.Last().IsOccupied,
                Temp = vent.History.Last().Temp
            });
        }

        return JsonConvert.SerializeObject(ventSummaries, Formatting.Indented);
    }
}
