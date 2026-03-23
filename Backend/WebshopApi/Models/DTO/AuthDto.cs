namespace DataTransferObject;

public record Register(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password,
    string Address,
    string PostCode);

public record Login(string Email, string Password);

public record ResetPassword(string Token, string Password);