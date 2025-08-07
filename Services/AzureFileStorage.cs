using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MyDotNet9Api.Services;

public class AzureFileStorage: IFileStorage
{
    private readonly string conectionString;

    public AzureFileStorage(IConfiguration configuration)
    {
        conectionString = configuration.GetConnectionString("AzureStorageConnection")!;
    }
    public async Task<string> Store(string container, IFormFile file)
    {
        var client = new BlobContainerClient(conectionString, container);
        await client.CreateIfNotExistsAsync();
        client.SetAccessPolicy(PublicAccessType.Blob);
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var blob = client.GetBlobClient(fileName);
        var blobHttpHeaders = new BlobHttpHeaders();
        blobHttpHeaders.ContentType = file.ContentType;
        await blob.UploadAsync(file.OpenReadStream(), blobHttpHeaders);
        return blob.Uri.ToString();
    }

    public async Task Delete(string? route, string container)
    {
        if (string.IsNullOrEmpty(route))
        {
            return;
        }
        var client = new BlobContainerClient(conectionString, container);
        await client.CreateIfNotExistsAsync();
        var fileName = Path.GetFileName(route);
        var blob = client.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();
    }
}