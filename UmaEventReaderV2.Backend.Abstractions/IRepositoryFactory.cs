namespace UmaEventReaderV2.Abstractions;

public interface IRepositoryFactory<TRepository>
{
    Task<TRepository> CreateAsync();
}