#if UNITY_EDITOR
namespace Editor.PlayboxInstaller
{
    public class UnityPackageData
    {
        public bool isImporting = false;
        public string packageName;
        public string layoutName;
        public string entryName;
    }
}
#endif