namespace IoTMonitoring.Application.DTOs
{
    /// <summary>Credenciais para autenticação na API.</summary>
    public class LoginDto
    {
        /// <summary>Nome de usuário.</summary>
        /// <example>admin</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>Senha do usuário.</summary>
        /// <example>admin123</example>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>Resposta retornada após autenticação bem-sucedida.</summary>
    public class TokenResponseDto
    {
        /// <summary>Token JWT para ser enviado no header Authorization: Bearer {token}.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Tipo do token.</summary>
        /// <example>Bearer</example>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>Data e hora de expiração do token (UTC).</summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>Nome de usuário autenticado.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Role do usuário autenticado.</summary>
        public string Role { get; set; } = string.Empty;
    }
}
