using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

[ApiController]
[Route("[controller]")]
[EnableCors]
public class TagsController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public TagsController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] List<Tag> data)
    {
        if (data == null || data.Count == 0)
        {
            return BadRequest("Invalid request body - no tags specified.");
        }

        foreach (var tag in data)
        {
            if (string.IsNullOrEmpty(tag.name))
            {
                return BadRequest("Invalid request body - tag name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(tag.type))
            {
                return BadRequest("Invalid request body - tag type cannot be null or empty.");
            }
        }

        // Perform any actions with the tag data here
        var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

        var client = _clientFactory.CreateClient();
        var response = await client.PostAsync("https://script.google.com/macros/s/AKfycbykHVA8ukxxyImFhHXtJ6F-sOPxwfhrJiLqHcCv1fHmwotf2Iv-2vKnb-2-KzItZusl/exec", jsonContent);

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
    [HttpGet]
    public async Task<IActionResult> GetAllDataAsync()
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync("https://script.google.com/macros/s/AKfycbykHVA8ukxxyImFhHXtJ6F-sOPxwfhrJiLqHcCv1fHmwotf2Iv-2vKnb-2-KzItZusl/exec");

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            // Parse the JSON response into a list of objects
            // List<Tag> data = JsonSerializer.Deserialize<List<Tag>>(responseData);
            return Ok(responseData);
        }
        else
        {
            return BadRequest($"Request failed: {response.StatusCode}");
        }
    }


}

public class Tag
{
    public string name { get; set; }
    public string type { get; set; }
    public bool required { get; set; }
}
