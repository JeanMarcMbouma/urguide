namespace UrGuide.WebApp.Models
{
    /// <summary>
    /// Request model for admin login that accepts either email or username
    /// </summary>
    public class AdminLoginRequest
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool Persist { get; set; }
    }

    /// <summary>
    /// Response model from IdentityServer token endpoint
    /// </summary>
    public class IdentityServerTokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }

    /// <summary>
    /// Request model for token refresh
    /// </summary>
    public class RefreshTokenRequest
    {
        public string? RefreshToken { get; set; }
    }
}
