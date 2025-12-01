#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

namespace PlayboxInstaller
{
    public static class FacebookArhivesData
    {
        private static Queue<string> packageQueue = new();
        private static bool _isImporting = false;

        public static bool isImporting
        {
            get => _isImporting;
            set => _isImporting = value;
        }
        
        public static List<UnityPackageData> UnpackArhives()
        {
            var path = Path.Combine(Application.dataPath, "../DownloadFiles/Facebook.zip");
            var extactFolder = Path.Combine(Application.dataPath, "../DownloadFiles/");
            List<UnityPackageData> packageEntries = new();
            
            if (!Directory.Exists(extactFolder))
                return packageEntries;

            if (!File.Exists(path))
            {
                return packageEntries;
            }

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                string packageEnds = ".unitypackage";
            
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(packageEnds))
                    {
                        packageEntries.Add(new UnityPackageData
                        {
                            packageName = Path.GetFileNameWithoutExtension(entry.FullName),
                            entryName = entry.FullName
                        });
                    }
                }
                
                return packageEntries;
            }
        }

        public static void InstallArhives(List<UnityPackageData> unityPackages)
        {
            var path = Path.Combine(Application.dataPath, "../DownloadFiles/Facebook.zip");
            var extactFolder = Path.Combine(Application.dataPath, "../DownloadFiles/");
            
            using (ZipArchive archive = new ZipArchive(File.OpenRead(path), ZipArchiveMode.Read))
            {
                List<string> extractingPackagesPaths = new();
                string packageEnds = ".unitypackage";

                foreach (var item in unityPackages)
                {
                    if (item.isImporting)
                    {
                        var extractingPackagePath = Path.Combine(extactFolder, item.packageName + ".unitypackage");
                        
                        archive.GetEntry(item.entryName)
                            .ExtractToFile(extractingPackagePath, true);
                        
                        extractingPackagesPaths.Add(extractingPackagePath);
                    }
                }

                foreach (var item in extractingPackagesPaths)
                {
                    packageQueue.Enqueue(item);
                }
                
                AssetDatabase.importPackageCompleted += OnPackageImportCompleted;
                AssetDatabase.importPackageCancelled += OnPackageImportCancelled;
                
                ImportNext();
            }
        }
        
        private static void OnPackageImportCompleted(string packagename)
        {
            Debug.Log($"✅ Imported: {packagename}");
            ImportNext();
        }

        private static void OnPackageImportCancelled(string packagename)
        {
            Debug.LogWarning($"❌ Cancelled: {packagename}");
            ImportNext();
        }
        
        private static void ImportNext()
        {
            if (packageQueue.Count == 0)
            {
                Debug.Log("✅ All packages imported.");
                isImporting = false;
                
                AssetDatabase.importPackageCompleted -= OnPackageImportCompleted;
                AssetDatabase.importPackageCancelled -= OnPackageImportCancelled;

                return;
            }

            isImporting = true;
            var path = packageQueue.Dequeue();
            Debug.Log($"📦 Importing: {path}");
            AssetDatabase.ImportPackage(path, false); 
        }
    }
}
#endif