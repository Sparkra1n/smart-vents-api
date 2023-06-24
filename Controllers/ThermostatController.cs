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

    [HttpPost]
    [Route("VentStatus")]
    public bool UpdateVentStatus(VentData ventData)
    {
        // Leave everything open if Smart-Vents is disabled
        if (_dashboardData.IsEnabled) return true;
    
        // Close if there's no motion
        if (!ventData.HasMotion) return false;
        
        // Close when the room reaches the target temp
        return ventData.Temp <= _dashboardData.TargetTemp ? true : false;
    }

    [HttpPost]
    [Route("Temp")]
    public void SetInfo(DashboardData dashboardData)
    {
        _dashboardData = dashboardData;
    }
}
