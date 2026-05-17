using Service;

namespace Service;

public class PasswordResetService
{
    private readonly PasswordResetRepository _passwordResetRepository;
    private readonly UserRepository _userRepository;
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;

    public PasswordResetService(
        PasswordResetRepository passwordResetRepository,
        UserRepository userRepository,
        EmailService emailService,
        IConfiguration configuration)
    {
        _passwordResetRepository = passwordResetRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task SendResetEmail(string email)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null) return;

        var token = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddHours(1);

        await _passwordResetRepository.SaveToken(user.Id, token, expiresAt);

        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        var resetLink = $"{frontendUrl}/#/reset-password?token={token}";

        await _emailService.SendPasswordResetEmail(user.Email, resetLink);
    }

    public async Task<bool> ResetPassword(string token, string newPassword)
    {
        var tokenData = await _passwordResetRepository.GetToken(token);
        if (tokenData == null) return false;

        var (userId, used, expiresAt) = tokenData.Value;

        if (used || expiresAt < DateTime.UtcNow) return false;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _userRepository.UpdatePassword(userId, hashedPassword);
        await _passwordResetRepository.MarkTokenAsUsed(token);

        return true;
    }
}