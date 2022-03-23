using System;
using System.IO;

namespace CESI.Configuration
{
    public static class CesiConstants
    {
        public static readonly Uri SsoOrigin = new Uri("https://login.eveonline.com");
        
        public static readonly Uri EsiOrigin = new Uri("https://esi.evetech.net");
    }

    public static class StaticDataConstants
    {
        public static DirectoryInfo RootDirectory => 
            new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? "../", "static-data"));

        public static DirectoryInfo DownloadDirectory =>
            new DirectoryInfo(Path.Combine(RootDirectory.FullName, "downloads"));

        public static DirectoryInfo ArchiveDirectory =>
            new DirectoryInfo(Path.Combine(RootDirectory.FullName, "archive"));
        
        public static DirectoryInfo WorkingDirectory => 
            new DirectoryInfo(Path.Combine(RootDirectory.FullName, "current"));
        
        public static Uri FuzzworkDownloadUri =>
            new Uri("https://www.fuzzwork.co.uk/dump/mysql-latest.tar.bz2");
        
        public static Uri OfficialDownloadUri => 
            new Uri("https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip");
    }
}