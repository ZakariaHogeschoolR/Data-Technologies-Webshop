namespace DataTransferObject;

public record BulkBoughtDto(int UserId, List<int> ProductIds);
