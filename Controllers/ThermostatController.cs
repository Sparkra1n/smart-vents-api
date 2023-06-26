using Microsoft.AspNetCore.Mvc;
using smart_vents_api.Models;

namespace smart_vents_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ThermostatController : ControllerBase
{
    private static DashboardData _dashboardData = new DashboardData();

    private readonly ILogger<ThermostatController> _logger;

    public ThermostatController(ILogger<ThermostatController> logger)
    {
        _logger = logger;
    }

    // Vent posts its info and is told whether it should be open or closed
    [HttpPost]
    [Route("VentStatus")]
    public bool SetVentStatus(VentData ventData)
    {
        // Leave everything open if Smart-Vents is disabled
        if (!_dashboardData.IsEnabled) return true;

        // Close if not occupied
        if (!ventData.IsOccupied) return false;

        // Close when the room reaches the target temp
        return ventData.Temp <= _dashboardData.TargetTemp ? true : false;
    }

    [HttpPost]
    [Route("Settings")]
    public void SetDashboardSettings(DashboardData dashboardData)
    {
        _dashboardData = dashboardData;
    }

    [HttpGet]
    [Route("VentData")]
    public List<VentData> GetVentData()
    {
        return new List<VentData> {
            new VentData {
                Id = 1,
                Temp = 25.1,
                IsOccupied = true
            },
            new VentData {
                Id = 2,
                Temp = 20.1,
                IsOccupied = false
            }
        };
    }
}
