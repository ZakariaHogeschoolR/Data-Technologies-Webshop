public interface IPasswordResetService
{
    Task SendResetEmail(string email);
   

    Task<bool> ResetPassword(string token, string newPassword);

}