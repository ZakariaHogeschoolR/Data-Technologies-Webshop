namespace DataTransferObject;

public record UserDto(
    int? Id,
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password,
    string Address,
    string PostCode);
