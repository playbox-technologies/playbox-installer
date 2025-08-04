#if UNITY_EDITOR

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor.PlayboxInstaller
{
    public class UnityPackageData
    {
        public bool isImporting = false;
        public string packageName;
        public string layoutName;
        public string entryName;
    }

    public enum FirebasePackage
    {
        [Description("💥")]
        Crashlytics,
        [Description("🗄️")]
        Database,
        [Description("⚙️")]
        RemoteConfig,
        [Description("📲")]
        Installations,
        [Description("🔧")]
        Functions,
        [Description("📄")]
        Firestore,
        [Description("🧠")]
        AI,
        [Description("🗂️")]
        Storage,
        [Description("🔐")]
        Auth,
        [Description("📩")]
        Messaging,
        [Description("🛡️")]
        AppCheck,
        [Description("📊")]
        Analytics
    }

public class FirebaseArhivesData
    {
        private static Queue<string> packageQueue = new();
        private static bool isImporting = false;
        
        public static string GetEnumEmoji<T>(T value)
        {
            var field = typeof(T).GetField(value.ToString());
            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? "";
        }
        
        public static List<UnityPackageData> UnpackArhives()
        {
            var path = Path.Combine(Application.dataPath, "../DownloadFiles/Firebase.zip");
            var extactFolder = Path.Combine(Application.dataPath, "../DownloadFiles/");
            
            using (ZipArchive archive = new ZipArchive(File.OpenRead(path), ZipArchiveMode.Read))
            {
                List<UnityPackageData> packageEntries = new();
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
        
        public static readonly Dictionary<FirebasePackage, string> EmojiMap = new()
        {
            { FirebasePackage.Crashlytics, "💥" },
            { FirebasePackage.Database, "🗄️" },
            { FirebasePackage.RemoteConfig, "⚙️" },
            { FirebasePackage.Installations, "📲" },
            { FirebasePackage.Functions, "🔧" },
            { FirebasePackage.Firestore, "📄" },
            { FirebasePackage.AI, "🧠" },
            { FirebasePackage.Storage, "🗂️" },
            { FirebasePackage.Auth, "🔐" },
            { FirebasePackage.Messaging, "📩" },
            { FirebasePackage.AppCheck, "🛡️" },
            { FirebasePackage.Analytics, "📊" },
        };

        public static void InstallArhives(List<UnityPackageData> unityPackages)
        {
            var path = Path.Combine(Application.dataPath, "../DownloadFiles/Firebase.zip");
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

                // Отписка
                AssetDatabase.importPackageCompleted -= OnPackageImportCompleted;
                AssetDatabase.importPackageCancelled -= OnPackageImportCancelled;

                return;
            }

            isImporting = true;
            var path = packageQueue.Dequeue();
            Debug.Log($"📦 Importing: {path}");
            AssetDatabase.ImportPackage(path, true); // показываем окно импорта
        }
    }

    public class PlayboxInstallerWindow : EditorWindow
    {
        private List<UnityPackageData> unityPackages = new();
        

        [MenuItem("PlayboxInstaller/Open Installer Window")]
        private static void UnpackArhivesMenu()
        {
            GetWindow<PlayboxInstallerWindow>("Arhives Window");
        }

        private void CreateGUI()
        {
            unityPackages = FirebaseArhivesData.UnpackArhives();
        }

        private void OnGUI()
        {
            GUILayout.Space(20);
            
            GUILayout.BeginVertical();
            
            GUILayout.Label("Install Firebase packages");
            
            GUILayout.Space(10);
            
            foreach (var item in unityPackages)
            {
                GUILayout.Space(5);
                
                GUILayout.BeginHorizontal();
                
                GUILayout.Space(10);
                
                GUILayout.Label(item.packageName);
                
                item.isImporting = GUILayout.Toggle(item.isImporting, "Importing",GUILayout.ExpandWidth(false));
                
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Install Arhives")) 
            {
                FirebaseArhivesData.InstallArhives(unityPackages);
            }

            GUILayout.EndVertical();
        }
    }
}

#endif
