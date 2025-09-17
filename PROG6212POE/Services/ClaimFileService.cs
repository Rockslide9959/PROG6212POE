using Azure.Storage.Files.Shares;

namespace PROG6212POE.Services
{
    public class ClaimFileService
    {
        private readonly ShareClient _shareClient;

        public ClaimFileService(IConfiguration config)
        {
            string connectionString = config["AzureStorage:ConnectionString"];
            string shareName = "claimfiles";
            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return;

            var directory = _shareClient.GetRootDirectoryClient();
            var fileClient = directory.GetFileClient(file.FileName);

            using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(file.Length);
            await fileClient.UploadAsync(stream);
        }

        public async Task<List<string>> ListFilesAsync()
        {
            var directory = _shareClient.GetRootDirectoryClient();
            var files = new List<string>();

            await foreach (var item in directory.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                    files.Add(item.Name);
            }

            return files;
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var directory = _shareClient.GetRootDirectoryClient();
            var fileClient = directory.GetFileClient(fileName);
            var download = await fileClient.DownloadAsync();
            return download.Value.Content;
        }
    }
}
