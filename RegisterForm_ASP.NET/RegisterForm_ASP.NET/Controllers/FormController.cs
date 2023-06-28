using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class GoogleSheetController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public GoogleSheetController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] JsonElement jsonData)
    {
        if (jsonData.ValueKind != JsonValueKind.Array)
        {
            return BadRequest("Invalid request body - data must be an array.");
        }

        var data = JsonSerializer.Deserialize<TagData[]>(jsonData.GetRawText());

        if (data == null || data.Length == 0)
        {
            return BadRequest("Invalid request body - no tags specified.");
        }

        foreach (var tag in data)
        {
            if (string.IsNullOrEmpty(tag.name))
            {
                return BadRequest("Invalid request body - tag name cannot be null or empty.");
            }
        }

        // Perform any actions with the data here
        var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

        var client = _clientFactory.CreateClient();
        var response = await client.PostAsync("https://script.google.com/macros/s/AKfycbwOtujj1FB3yhRs7Zykhkwp_0sU6wUxsfxsSl-kWaWSekGlBCUb0csnyAx2-XhhejJI/exec", jsonContent);

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
public class TagData
{
    public string id { get; set; }
    public string name { get; set; }
    public string value { get; set; }
    
}
