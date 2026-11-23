namespace Rentals.API.Models;

public sealed class UserWriteRequest
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}
