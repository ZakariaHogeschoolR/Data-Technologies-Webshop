namespace DataTransferObject;

public record CheckoutRequestDto(string PaymentMethod);

public record CheckoutResultDto(int OrderId, decimal Total, DateTime CreatedAt, List<CheckoutItemDto> Items);

public record CheckoutItemDto(
    int ProductId,
    string Name,
    string ProductImage,
    decimal Price,
    int Quantity,
    decimal SubTotal);
