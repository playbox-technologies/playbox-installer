#if UNITY_EDITOR
namespace PlayboxInstaller
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