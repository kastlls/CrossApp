using System;

namespace Core.Dto;

// Простий формат даних для збереження видачі
public record LoanDto(string Id, string CopyId, string ReaderId, DateTime IssuedOn, DateTime? ReturnedOn);
