namespace DataTransferObject;

public record ProfileDto(
    int Id,
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Address,
    string PostCode);

public record UpdateProfileDto(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Address,
    string PostCode);

public record ChangePasswordDto(string CurrentPassword, string NewPassword);
