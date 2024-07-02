namespace FC.Codeflix.Catalog.Application.Interfaces;
public interface IStorageService
{
    Task<string> UploadAsync(string fileName, Stream fileStream, CancellationToken cancellationToken);
}
