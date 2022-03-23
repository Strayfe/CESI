using System;
using System.IO;
using System.Threading.Tasks;

namespace CESI.StaticData.Handlers
{
    public interface IStaticDataImportHandler
    {
        Task<bool> ArchiveStaticData(DirectoryInfo downloadDirectory = null,
            DirectoryInfo workingDirectory = null,
            DirectoryInfo archiveDirectory = null);

        Task<bool> DownloadStaticData(Uri downloadUri = null, DirectoryInfo downloadDirectory = null);

        Task<bool> ExtractStaticData(FileInfo fileToDecompress = null, DirectoryInfo workingDirectory = null);

        Task<bool> ImportStaticData(FileInfo importFile = null);
    }
}