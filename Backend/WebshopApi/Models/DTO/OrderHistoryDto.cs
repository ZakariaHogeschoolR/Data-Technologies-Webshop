namespace DataTransferObject;

public record OrderHistoryDto(int OrderId, DateTime OrderDate, List<OrderItemDto> Items);

public record OrderItemDto(int ProductId, int Quantity);
