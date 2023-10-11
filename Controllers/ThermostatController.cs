using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using smart_vents_api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace smart_vents_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ThermostatController : ControllerBase
{
    private static DashboardSettings Settings = new();
    private static readonly List<Vent> Vents = new();
    private readonly ILogger<ThermostatController> Logger;
    public ThermostatController(ILogger<ThermostatController> logger)
    {
        Logger = logger;
        Settings = new DashboardSettings()
        {
            IsEnabled = false,
            MasterTargetTemp = 25
        };
    }

    [HttpPost]
    [Route("setVentTargetTemp")]
    public ActionResult SetVentTargetTemp(string id, double targetTemp)
    {
        foreach (var vent in Vents)
        {
            if (vent.Id == id)
            {
                vent.TargetTemp = targetTemp;
                return Ok();
            }
        }
        return NotFound();
    }

    [HttpPost]
    [Route("addVent")]
    public ActionResult AddVent(string id)
    {
        if (Vents.Any(vent => vent.Id == id))
            return BadRequest();

        Vents.Add(new Vent()
        {
            Id = id,
            TargetTemp = 25,
            History = new List<VentData>()
        });

        return Ok();
    }

    [HttpPost]
    [Route("settings")]
    public void SetDashboardSettings(DashboardSettings dashboardData)
    {
        Settings = dashboardData;
    }

    [HttpGet]
    [Route("ventHistory")]
    public ActionResult<List<VentData>> GetVentHistory(string id)
    {
        foreach (var vent in Vents)
        {
            if (vent.Id == id)
                return vent.History;
        }
        
        return NotFound();
    }

    [HttpGet]
    [Route("ventHistories")]
    public string GetVentHistories()
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        return JsonConvert.SerializeObject(Vents, settings);
    }

    [HttpGet]
    [Route("ventSummaries")]
    public List<VentSummary> GetVentSummaries()
    {
        List<VentSummary> ventSummaries = new();

        foreach (var vent in Vents)
        {
            var lastHistory = vent.History.LastOrDefault();

            ventSummaries.Add(new VentSummary()
            {
                Id = vent.Id,
                TargetTemp = vent.TargetTemp,
                IsOccupied = lastHistory?.IsOccupied,
                Temp = lastHistory?.Temp
            });
        }

        return ventSummaries;
    }

        // Vent posts its info and is told whether it should be open or closed
    [HttpPost]
    [Route("updateVent")]
    public ActionResult<VentState> UpdateVent(VentSummary payload)
    {
        var ventToUpdate = Vents.FirstOrDefault(vent => vent.Id == payload.Id);
        if (ventToUpdate == null)
            return NotFound();

        var timestampRounded = DateTime.UtcNow.AddTicks(-(DateTime.UtcNow.Ticks % TimeSpan.TicksPerMinute)).ToString("u");

        if (ventToUpdate.History.Any(item => timestampRounded == item.Timestamp))
            return VentState.stay;

        var newVentData = new VentData
        {
            Timestamp = timestampRounded,
            Temp = payload.Temp ?? 0,
            IsOccupied = payload.IsOccupied ?? false
        };

        ventToUpdate.History.Add(newVentData);

        if (!Settings.IsEnabled)
            return VentState.open;

        if (!newVentData.IsOccupied)
            return VentState.close;

        return payload.Temp <= ventToUpdate.TargetTemp ? VentState.open : VentState.close;
    }
}
