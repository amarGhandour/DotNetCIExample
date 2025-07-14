using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace VulnerableApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecurityTestController : ControllerBase
{
    private const string ApiKey = "5up3rS3cr3tK3y!";
    private static readonly List<User> _users = new()
    {
        new User(1, "admin", "P@ssw0rd123"),
        new User(2, "guest", "Guest1234")
    };

    // 1. SQL Injection (High Risk)
    [HttpGet("user-info")]
    public IActionResult GetUserInfo([FromQuery] string username)
    {
        using var connection = new SqlConnection("Server=db;Database=prod;User Id=sa;Password=Qwerty123;");
        var query = $"SELECT * FROM Users WHERE Username = '{username}'"; // UNSAFE
        var command = new SqlCommand(query, connection);

        connection.Open();
        var reader = command.ExecuteReader();
        return Ok(reader);
    }

    // 2. Hardcoded Secrets (Critical)
    [HttpGet("validate-key")]
    public IActionResult ValidateKey([FromHeader] string clientKey)
    {
        if (clientKey == ApiKey) // UNSAFE
        {
            return Ok(new { Message = "Access granted" });
        }
        return Unauthorized();
    }

    // 3. Weak Hashing (Medium Risk)
    [HttpPost("create-user")]
    public IActionResult CreateUser([FromBody] UserRequest request)
    {
        var hashedPassword = Convert.ToBase64String(
            MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(request.Password))); // UNSAFE

        _users.Add(new User(_users.Count + 1, request.Username, hashedPassword));
        return CreatedAtAction(nameof(GetUserInfo), new { request.Username });
    }

    // 4. Missing Rate Limiting (Low Risk)
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { Status = "API is vulnerable" });
    }

    // 5. Information Exposure (Medium)
    [HttpGet("user/{id}")]
    public IActionResult GetUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Ok(user); // UNSAFE (exposes password hash)
    }

    public record User(int Id, string Username, string Password);
    public record UserRequest(string Username, string Password);
}