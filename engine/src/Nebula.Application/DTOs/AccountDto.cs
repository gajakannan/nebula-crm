namespace Nebula.Application.DTOs;

public record AccountDto(
    Guid Id,
    string Name,
    string Status,
    string? Industry);
