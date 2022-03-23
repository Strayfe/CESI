using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CESI.Configuration;
using CESI.StaticData.DbContexts;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SharpCompress.Readers;
using SharpCompress.Readers.Tar;
using CompressionMode = System.IO.Compression.CompressionMode;

namespace CESI.StaticData.Handlers
{
    public class MySqlStaticDataImportHandler : IStaticDataImportHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly StaticDataContext _staticDataContext;
        private readonly CesiConfiguration _cesiConfiguration;

        public MySqlStaticDataImportHandler(IHttpClientFactory httpClientFactory, StaticDataContext staticDataContext, 
            IOptions<CesiConfiguration> cesiConfiguration)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _staticDataContext = staticDataContext ?? throw new ArgumentNullException(nameof(staticDataContext));
            _cesiConfiguration = cesiConfiguration.Value ?? throw new ArgumentNullException(nameof(cesiConfiguration));
        }

        public async Task<bool> ArchiveStaticData(DirectoryInfo downloadDirectory = null,
            DirectoryInfo workingDirectory = null,
            DirectoryInfo archiveDirectory = null)
        {
            downloadDirectory ??= StaticDataConstants.DownloadDirectory;
            archiveDirectory ??= StaticDataConstants.ArchiveDirectory;

            if (!Directory.Exists(downloadDirectory.FullName))
            {
                return await Task.FromResult(true);
            }

            if (!Directory.Exists(archiveDirectory.FullName))
            {
                Directory.CreateDirectory(archiveDirectory.FullName);
            }

            // archive any downloads
            var downloads = Directory.EnumerateFiles(downloadDirectory.FullName);
            foreach (var download in downloads)
            {
                var timeStamped = string.Concat(Path.GetFileNameWithoutExtension(download), "-",
                    DateTime.Today.ToFileTimeUtc(), Path.GetExtension(download));
                File.Move(download, Path.Combine(archiveDirectory.FullName, timeStamped), true);
            }

            // maintain the archive folder so it doesn't grow too large
            var archivedFiles = Directory.GetFiles(archiveDirectory.FullName);
            foreach (var file in archivedFiles)
            {
                var lastWriteTimeUtc = File.GetLastWriteTimeUtc(file);

                if (lastWriteTimeUtc < DateTime.Today.Subtract(TimeSpan.FromDays(7)))
                {
                    File.Delete(file);
                }
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> DownloadStaticData(Uri downloadUri = null, DirectoryInfo downloadDirectory = null)
        {
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage
            {
                RequestUri = StaticDataConstants.FuzzworkDownloadUri,
                Method = HttpMethod.Get,
                Headers =
                {
                    {"User-Agent", ".NET Core Web Application"},
                    {"Accept", "*/*"},
                    {"Accept-Language", "en-US, en;q=0.5"},
                    {"Accept-Encoding", "gzip, deflate, br"},
                    {"Connection", "keep-alive"},
                    {"Upgrade-Insecure-Requests", "1"}
                }
            };

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return await Task.FromResult(false);
            }

            if (!Directory.Exists(StaticDataConstants.DownloadDirectory.FullName))
            {
                Directory.CreateDirectory(StaticDataConstants.DownloadDirectory.FullName);
            }

            var downloadData = await response.Content.ReadAsByteArrayAsync();

            await using FileStream fileStream =
                File.OpenWrite(Path.Combine(StaticDataConstants.DownloadDirectory.FullName, "mysql-latest.tar.bz2"));
            await fileStream.WriteAsync(downloadData);
            await fileStream.DisposeAsync();

            return await Task.FromResult(File.Exists(Path.Combine(StaticDataConstants.DownloadDirectory.FullName,
                "mysql-latest.tar.bz2")));
        }

        public async Task<bool> ExtractStaticData(FileInfo fileToDecompress = null,
            DirectoryInfo workingDirectory = null)
        {
            fileToDecompress ??=
                new FileInfo(Path.Combine(StaticDataConstants.DownloadDirectory.FullName, "mysql-latest.tar.bz2"));
            workingDirectory ??= StaticDataConstants.WorkingDirectory;

            if (!File.Exists(fileToDecompress.FullName))
            {
                return await Task.FromResult(false);
            }

            if (Directory.Exists(workingDirectory.FullName))
            {
                Directory.Delete(workingDirectory.FullName, true);
            }
            
            Directory.CreateDirectory(workingDirectory.FullName);

            if (fileToDecompress.Extension != ".bz2" && fileToDecompress.Extension != ".gzip")
            {
                return await Task.FromResult(false);
            }
            
            var tarArchive = new FileInfo(Path.Combine(workingDirectory.FullName,
                fileToDecompress.Name.Remove(fileToDecompress.Name.Length - fileToDecompress.Extension.Length)));

            await using (var originalFileStream = new FileInfo(fileToDecompress.FullName).OpenRead())
            {
                await using (var decompressedFileStream = File.Create(tarArchive.FullName))
                {
                    await using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        await decompressionStream.CopyToAsync(decompressedFileStream);
                        Console.WriteLine($"Decompressed: {fileToDecompress.Name}");
                    }
                }
            }

            if (tarArchive.Extension != ".tar")
            {
                return await Task.FromResult(false);
            }
            
            await using (var originalFileStream = tarArchive.OpenRead())
            {
                TarReader.Open(originalFileStream).WriteAllToDirectory(workingDirectory.FullName);
                Console.WriteLine($"Decompressed: {tarArchive.Name}");
            }

            if (File.Exists(tarArchive.FullName))
            {
                File.Delete(tarArchive.FullName);
            }
            
            return await Task.FromResult(Directory.EnumerateFiles(workingDirectory.FullName, "*.sql").Any());
        }

        public async Task<bool> ImportStaticData(FileInfo importFile = null)
        {
            importFile ??= StaticDataConstants.WorkingDirectory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            _staticDataContext.Database.EnsureCreated();
            
            await using var mySqlConnection = new MySqlConnection(_cesiConfiguration?.DatabaseOptions?.ConnectionString);
            await using var mySqlCommand = new MySqlCommand();
            using var mySqlBackup = new MySqlBackup(mySqlCommand);
            mySqlCommand.Connection = mySqlConnection;
            await mySqlConnection.OpenAsync();
            mySqlBackup.ImportFromFile(importFile.FullName);
            await mySqlConnection.CloseAsync();
            
            return true;
        }
    }
}