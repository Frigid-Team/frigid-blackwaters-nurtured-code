using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrigidBlackwaters.Core
{
    public class SaveFileSystem : FrigidMonoBehaviourWithUpdate
    {
        private static SaveFileSystem instance;

        [SerializeField]
        private string newSaveDirectoryName;
        private string selectedSaveDirectoryPath;

        private Queue<IEnumerator> readWriteTasks;

        public static void ReadSaveFile(string fileNameWithoutExtension, Action<SaveFileData> onComplete)
        {
            IEnumerator ReadTask()
            {
                string fileName = fileNameWithoutExtension + ".json";
                string savePath = Path.Combine(instance.selectedSaveDirectoryPath, fileName);
                if (File.Exists(savePath))
                {
                    using (FileStream stream = new FileStream(savePath, FileMode.Open))
                    {
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            Task<string> readTask = streamReader.ReadToEndAsync();
                            while (!readTask.IsCompleted)
                            {
                                yield return null;
                            }
                            SaveFileData saveFileData = JsonUtility.FromJson<SaveFileData>(readTask.Result);
                            if (saveFileData == null)
                            {
                                saveFileData = new SaveFileData();
                            }
                            onComplete?.Invoke(saveFileData);
                        }
                    }
                }
                else
                {
                    onComplete?.Invoke(new SaveFileData());
                }
            }
            instance.readWriteTasks.Enqueue(ReadTask());
        }

        public static void WriteSaveFile(string fileNameWithoutExtension, SaveFileData data, Action onComplete)
        {
            IEnumerator<FrigidCoroutine.Delay> WriteTask()
            {
                string fileName = fileNameWithoutExtension + ".json";
                string savePath = Path.Combine(instance.selectedSaveDirectoryPath, fileName);
                Directory.CreateDirectory(instance.selectedSaveDirectoryPath);
                string jsonString = JsonUtility.ToJson(data);
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        Task writeTask = streamWriter.WriteAsync(jsonString);
                        while (!writeTask.IsCompleted)
                        {
                            yield return null;
                        }
                        onComplete?.Invoke();
                    }
                }
            }
            instance.readWriteTasks.Enqueue(WriteTask());
        }

        public static void CreateSaveDirectory(string saveDirectoryName)
        {
            string saveDirectoryPath = Path.Combine(Application.persistentDataPath, saveDirectoryName);
            Directory.CreateDirectory(saveDirectoryPath);
        }

        public static void DeleteSaveDirectory(string saveDirectoryName)
        {
            string saveDirectoryPath = Path.Combine(Application.persistentDataPath, saveDirectoryName);
            Directory.Delete(saveDirectoryPath, true);
        }

        public static string[] GetSaveDirectoryNames()
        {
            string[] saveDirectoryPaths = Directory.GetDirectories(Application.persistentDataPath);
            string[] saveDirectoryNames = new string[saveDirectoryPaths.Length];
            for (int i = 0; i < saveDirectoryPaths.Length; i++)
            {
                saveDirectoryNames[i] = Path.GetFileName(saveDirectoryPaths[i]);
            }
            return saveDirectoryNames;
        }

        public static void SelectSaveDirectory(string saveDirectoryName)
        {
            instance.selectedSaveDirectoryPath = Path.Combine(Application.persistentDataPath, saveDirectoryName);
        }

        protected override void Awake()
        {
            base.Awake();

            DontDestroyInstanceOnLoad(this);
            instance = this;

            this.selectedSaveDirectoryPath = string.Empty;
            string[] saveDirectoryNames = GetSaveDirectoryNames();
            if (saveDirectoryNames.Length > 0)
            {
                SelectSaveDirectory(saveDirectoryNames[0]);
            }
            else
            {
                SelectSaveDirectory(this.newSaveDirectoryName);
            }

            this.readWriteTasks = new Queue<IEnumerator>();
        }

        protected override void Update()
        {
            base.Update();
            if (this.readWriteTasks.TryPeek(out IEnumerator readWriteTask))
            {
                if (!readWriteTask.MoveNext())
                {
                    this.readWriteTasks.Dequeue();
                }
            }
        }

#if UNITY_EDITOR
        [MenuItem(FrigidPaths.MenuItem.Jobs + "Save File System/Clear Local Saves")]
        private static void ClearLocalSaves()
        {
            foreach (string saveDirectoryName in GetSaveDirectoryNames())
            {
                DeleteSaveDirectory(saveDirectoryName);
            }
        }
#endif
    }
}
