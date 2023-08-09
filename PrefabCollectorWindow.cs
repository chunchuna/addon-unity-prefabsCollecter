using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AssetCollectorWindow : EditorWindow
{
    private static List<GameObject> collectedPrefabs = new List<GameObject>();
    private static List<Texture> collectedTextures = new List<Texture>();
    private static List<AudioClip> collectedAudioClips = new List<AudioClip>();
    private static List<Object> collectedAssets = new List<Object>();
    private static List<string> collectedFolders = new List<string>();
    private const string prefKey = "CollectedAssets";
    private const string locateInProjectPrefKey = "LocateInProject";
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "欢迎", "Prefabs", "Textures", "Audio Clips", "Assets", "Folders", "Settings" };
    private bool locateInProject = true;
    [MenuItem("Window/资源收藏器窗口")]
    public static void ShowWindow()
    {
        GetWindow<AssetCollectorWindow>("资源收藏器窗口");
    }

    private void OnEnable()
    {
        LoadAssets();
        locateInProject = EditorPrefs.GetBool(locateInProjectPrefKey, true);
    }

    private void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts);

        switch (toolbarOption)
        {
            case 0:
                DisplayWelcome();
                break;
            case 1:
                DisplayPrefabs(collectedPrefabs);
                break;
            case 2:
                DisplayAssets(collectedTextures);
                break;
            case 3:
                DisplayAssets(collectedAudioClips);
                break;
            case 4:
                DisplayAssets(collectedAssets);
                break;
            case 5:
                DisplayFolders(collectedFolders);
                break;
            case 6:
                DisplaySettings();
                break;
        }
    }

    private void DisplayWelcome()
    {
        EditorGUILayout.LabelField("欢迎使用资源收藏器窗口！", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("这个有啥用：", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("我们223 225 227 这类项目场景众多，里面的小物件数不胜数，为了不再Project窗口理翻箱倒柜；老是不能快速的找到想要的预制体或者其他资源。所以这个插件的诞生就是解放你的搜索框。你可以把常用的预制体放进来，可以快速定位或者生成到你正在编辑的场景里，可谓是227好帮手啊！", MessageType.Info);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("用法描述：", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("你可以直接在资源上右键选择 添加按钮 将资源和文件夹添加到收藏列表。在收藏列表中，你可以点击资源或文件夹来定位它们，也可以点击'删除'按钮来移除它们。", MessageType.Info);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("警告信息：", EditorStyles.boldLabel);
        EditorStyles.label.normal.textColor = Color.red;
        EditorGUILayout.HelpBox("请不要把插件上传到项目中，因为这是策划用GPT写的，可能会影响项目的代码，可能会有风险。所以自己在本地偷偷用就行", MessageType.Warning);
        EditorStyles.label.normal.textColor = Color.black;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("感谢列表：", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("特此感谢", MessageType.Info);
        EditorGUILayout.HelpBox("孙总没有你的需求就没有这插件的今天", MessageType.Info);
        EditorGUILayout.HelpBox("！", MessageType.Info);
        EditorGUILayout.HelpBox("！！", MessageType.Info);
        EditorGUILayout.HelpBox("有其他bug或者需求请告知我 反正我也不一定会改", MessageType.Info);
    }

    private void DisplayPrefabs(List<GameObject> prefabs)
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = locateInProject;
            GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(prefabs[i], typeof(GameObject), false);
            GUI.enabled = true;
            if (newPrefab != prefabs[i])
            {
                prefabs[i] = newPrefab;
                if (locateInProject)
                {
                    EditorGUIUtility.PingObject(newPrefab);
                }
            }
            if (GUILayout.Button("添加到场景"))
            {
                PrefabUtility.InstantiatePrefab(prefabs[i]);
            }
            if (GUILayout.Button("删除"))
            {
                prefabs.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        SaveAssets();
    }

    private void DisplayAssets<T>(List<T> assets) where T : Object
    {
        for (int i = 0; i < assets.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = locateInProject;
            T newAsset = (T)EditorGUILayout.ObjectField(assets[i], typeof(T), false);
            GUI.enabled = true;
            if (!EqualityComparer<T>.Default.Equals(newAsset, assets[i]))
            {
                assets[i] = newAsset;
                if (locateInProject)
                {
                    EditorGUIUtility.PingObject(newAsset);
                }
            }
            if (GUILayout.Button("删除"))
            {
                assets.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        SaveAssets();
    }

    private void DisplayFolders(List<string> folders)
    {
        for (int i = 0; i < folders.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(folders[i]);
            if (GUILayout.Button("定位"))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(folders[i]));
            }
            if (GUILayout.Button("删除"))
            {
                folders.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        SaveAssets();
    }

    private void DisplaySettings()
    {
        bool newLocateInProject = EditorGUILayout.Toggle("在Project窗口中定位资源", locateInProject);
        if (newLocateInProject != locateInProject)
        {
            locateInProject = newLocateInProject;
            EditorPrefs.SetBool(locateInProjectPrefKey, locateInProject);
        }
    }

    [MenuItem("Assets/添加到资源收藏器", false, 0)]
    private static void AddAssetToCollector()
    {
        foreach (var selectedObject in Selection.objects)
        {
            if (selectedObject is GameObject && !IsAssetInList(collectedPrefabs, selectedObject as GameObject))
            {
                collectedPrefabs.Add(selectedObject as GameObject);
            }
            else if (selectedObject is Texture && !IsAssetInList(collectedTextures, selectedObject as Texture))
            {
                collectedTextures.Add(selectedObject as Texture);
            }
            else if (selectedObject is AudioClip && !IsAssetInList(collectedAudioClips, selectedObject as AudioClip))
            {
                collectedAudioClips.Add(selectedObject as AudioClip);
            }
            else if (!IsAssetInList(collectedAssets, selectedObject))
            {
                collectedAssets.Add(selectedObject);
            }
        }
        SaveAssets();
    }

    [MenuItem("Assets/添加文件夹到资源收藏器", false, 0)]
    private static void AddFolderToCollector()
    {
        foreach (var selectedPath in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(selectedPath);
            if (AssetDatabase.IsValidFolder(path) && !collectedFolders.Contains(path))
            {
                collectedFolders.Add(path);
            }
        }
        SaveAssets();
    }

    private static bool IsAssetInList<T>(List<T> assets, T asset) where T : Object
    {
        foreach (var item in assets)
        {
            if (AssetDatabase.GetAssetPath(item) == AssetDatabase.GetAssetPath(asset))
            {
                return true;
            }
        }
        return false;
    }

    private static void SaveAssets()
    {
        SaveAssetList(collectedPrefabs, prefKey + "Prefabs");
        SaveAssetList(collectedTextures, prefKey + "Textures");
               SaveAssetList(collectedAudioClips, prefKey + "AudioClips");
        SaveAssetList(collectedAssets, prefKey + "Assets");
        SaveFolderList(collectedFolders, prefKey + "Folders");
    }

    private static void SaveAssetList<T>(List<T> assets, string key) where T : Object
    {
        string assetPaths = "";
        foreach (var asset in assets)
        {
            assetPaths += AssetDatabase.GetAssetPath(asset) + ";";
        }
        EditorPrefs.SetString(key, assetPaths);
    }

    private static void SaveFolderList(List<string> folders, string key)
    {
        string folderPaths = "";
        foreach (var folder in folders)
        {
            folderPaths += folder + ";";
        }
        EditorPrefs.SetString(key, folderPaths);
    }

    private void LoadAssets()
    {
        collectedPrefabs.Clear();
        collectedTextures.Clear();
        collectedAudioClips.Clear();
        collectedAssets.Clear();
        collectedFolders.Clear();

        LoadAssetList(collectedPrefabs, prefKey + "Prefabs");
        LoadAssetList(collectedTextures, prefKey + "Textures");
        LoadAssetList(collectedAudioClips, prefKey + "AudioClips");
        LoadAssetList(collectedAssets, prefKey + "Assets");
        LoadFolderList(collectedFolders, prefKey + "Folders");
    }

    private void LoadAssetList<T>(List<T> assets, string key) where T : Object
    {
        string assetPaths = EditorPrefs.GetString(key, "");
        string[] assetPathArray = assetPaths.Split(';');
        foreach (var path in assetPathArray)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
    }

    private void LoadFolderList(List<string> folders, string key)
    {
        string folderPaths = EditorPrefs.GetString(key, "");
        string[] folderPathArray = folderPaths.Split(';');
        foreach (var path in folderPathArray)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                folders.Add(path);
            }
        }
    }
}