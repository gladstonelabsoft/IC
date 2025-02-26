
/// <summary>
/// Examples demonstrating password reset endpoint usage
/// </summary>
public class PasswordResetExamples
{
    /// <summary>
    /// Example of initiating password reset
    /// </summary>
    public static async Task InitiatePasswordResetExample(HttpClient client)
    {
        var request = new UserResetTokenByEmailApiDto 
        { 
            Email = "user@example.com" 
        };

        var response = await client.PutAsync(
            "api/users/ResetTokenByEmail",
            new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            )
        );

        if (response.IsSuccessStatusCode)
        {
            // Success - email sent if user exists
            Console.WriteLine("Password reset initiated successfully");
        }
        else if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            // Handle validation errors
            var error = await response.Content.ReadFromJsonAsync<ErrorDto>();
            Console.WriteLine($"Validation error: {error.Error}");
        }
    }
}
