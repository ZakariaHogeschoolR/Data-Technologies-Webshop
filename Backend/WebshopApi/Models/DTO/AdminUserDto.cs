namespace DataTransferObject;

public record AdminUserDto(
    int Id,
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Address,
    string PostCode,
    string Role);
