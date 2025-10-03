namespace UmaEventReaderV2.Abstractions;

public interface IRepositoryInitializer
{
    public Task InitializeAsync(CancellationToken cancellationToken = default);
}