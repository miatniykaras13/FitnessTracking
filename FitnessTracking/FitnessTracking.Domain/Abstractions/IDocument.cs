namespace FitnessTracking.Domain.Abstractions;

public interface IDocument
{
    string Id { get; set; }

    DateTime CreatedAt { get; set; }
}
