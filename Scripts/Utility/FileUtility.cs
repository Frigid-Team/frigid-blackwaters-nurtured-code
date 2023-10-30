using UnityEngine;

namespace FrigidBlackwaters.Utility
{
	public class FileUtility
	{
#if UNITY_EDITOR
        public static string AssetsRelativePath(string absolutePath)
		{
			if (absolutePath.StartsWith(Application.dataPath))
			{
				return FrigidPaths.ProjectFolder.Assets + absolutePath.Substring(Application.dataPath.Length + 1);
			}
			else
			{
				throw new System.ArgumentException("Full path does not contain the current project's Assets folder.", absolutePath);
			}
		}
#endif
    }
}
