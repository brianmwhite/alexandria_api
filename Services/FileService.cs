namespace alexandria.api.Services
{
    public interface IFileService
    {
        void CopyFile(string sourcePath, string destinationPath);
        IEnumerable<DirectoryListing> GetSubDirectories(string path);
    }
    public class DirectoryListing
    {
        public required string FullPath { get; set; }
        public required string Name { get; set; }
    }
    public class FileService : IFileService
    {
        public void CopyFile(string sourcePath, string destinationPath)
        {
            if (Directory.Exists(destinationPath))
            {
                if (File.Exists(sourcePath))
                {
                    var filename = Path.GetFileName(sourcePath);
                    var destinationFilePath = Path.Combine(destinationPath, filename);
                    if (!File.Exists(destinationFilePath))
                    {
                        File.Copy(sourcePath, destinationFilePath);
                    }
                }
                else
                {
                    throw new FileNotFoundException($"Source file not found: {sourcePath}");
                }
            }
            else
            {
                throw new DirectoryNotFoundException($"Destination directory not found: {destinationPath}");
            }
        }


        public IEnumerable<DirectoryListing> GetSubDirectories(string path)
        {
            var directories = Directory.GetDirectories(path);
            var directoryList = new List<DirectoryListing>();
            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                directoryList.Add(
                    new DirectoryListing
                    {
                        FullPath = directory,
                        Name = directoryInfo.Name
                    });
            }
            return directoryList;
        }
    }
}