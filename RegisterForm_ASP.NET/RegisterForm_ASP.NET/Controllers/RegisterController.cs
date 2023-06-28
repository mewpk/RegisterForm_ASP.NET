using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

[ApiController]
[Route("[controller]")]
[EnableCors]
public class LineController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public LineController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] LineUserProfileData data)
    {
        if (data == null || string.IsNullOrEmpty(data.UserId))
        {
            return BadRequest("Invalid request body - user ID cannot be null or empty.");
        }

        // Perform any actions with user profile data here
        var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await _clientFactory.CreateClient().PostAsync("https://script.google.com/macros/s/AKfycbyk1weoJ2OEEBaYu1tUJZbm_9F9wghpf7gTGTVZBXofFNfldRqQJBAT-uUO2HnIF2ba3g/exec", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            return Ok(responseData);
        }
        else
        {
            return BadRequest($"Request failed: {response.StatusCode}");
        }
    }
}

public class LineUserProfileData
{
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public string PictureUrl { get; set; }
    public string StatusMessage { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
