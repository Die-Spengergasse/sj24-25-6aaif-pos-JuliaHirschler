namespace SPG_Fachtheorie.Aufgabe3.Dtos;

public record PaymentItemDto(
    string ArticleName,
    int Amount,
    decimal Price
    );