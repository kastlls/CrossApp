namespace Core.Dto;

// Простий формат даних для збереження примірника
public record BookCopyDto(string Id, string Isbn, bool IsIssued);
