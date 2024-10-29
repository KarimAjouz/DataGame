#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.DreamOS
{
    public class NDToolsMenu : Editor
    {
        static string objectPath;

        #region Methods & Helpers
        static void GetObjectPath()
        {
            objectPath = AssetDatabase.GetAssetPath(Resources.Load("UI Manager/DreamOS UI Manager (90s)"));
            objectPath = objectPath.Replace("Resources/UI Manager/DreamOS UI Manager (90s).asset", "").Trim();
            objectPath = objectPath + "Prefabs/";
        }

        static void MakeSceneDirty(GameObject source, string sourceName)
        {
            if (Application.isPlaying == false)
            {
                Undo.RegisterCreatedObjectUndo(source, sourceName);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        static void SelectErrorDialog()
        {
            EditorUtility.DisplayDialog("DreamOS", "Cannot create the object due to missing manager file. " +
                    "Make sure you have a valid 'DreamOS UI Manager' file in DreamOS > Resources > UI Manager folder.", "Okay");
        }
        #endregion

        #region Tools Menu
        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Create Overlay Resources", false, 12)]
        static void CreateOverlayResources()
        {
            try
            {
                GetObjectPath();
                GameObject clone = AssetDatabase.LoadAssetAtPath(objectPath + "Main Sources/DreamOS Canvas (90s).prefab", typeof(GameObject)) as GameObject;
                PrefabUtility.InstantiatePrefab(clone);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                MakeSceneDirty(clone, clone.name);
            }

            catch { SelectErrorDialog(); }
        }

        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Create World Space Resources", false, 12)]
        static void CreateWorldSpaceResources()
        {
            try
            {
                GetObjectPath();
                GameObject clone = AssetDatabase.LoadAssetAtPath(objectPath + "Main Sources/World Space Resources (90s).prefab", typeof(GameObject)) as GameObject;
                PrefabUtility.InstantiatePrefab(clone);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                MakeSceneDirty(clone, clone.name);
            }

            catch { SelectErrorDialog(); }
        }

        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Create Multi Instance Manager", false, 12)]
        static void CreateMultiInstanceManager()
        {
            GameObject tempObj = new GameObject("Multi Instance Manager");
            tempObj.AddComponent<MultiInstanceManager>();
            Selection.activeObject = tempObj;
            MakeSceneDirty(tempObj, tempObj.name);
        }

        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Open UI Manager", false, 24)]
        static void OpenUIManager()
        {
            Selection.activeObject = Resources.Load("UI Manager/DreamOS UI Manager (90s)");

            if (Selection.activeObject == null)
                Debug.Log("Can't find an asset named 'DreamOS UI Manager (90s)'. Make sure you have a valid 'UI Manager' asset in Resources/UI Manager folder. " +
                    "You can create a new UI Manager asset or re-import the pack if you can't see the file.");
        }

        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Select App Library")]
        static void SelectAppLibrary()
        {
            Selection.activeObject = Resources.Load("Apps/App Library (90s)");

            if (Selection.activeObject == null)
                Debug.Log("Can't find an asset named 'App Library (90s)'. Make sure you have 'App Library (90s)' asset in Resources/Apps folder.");
        }

        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Select Music Library")]
        static void SelectMusicLibrary()
        {
            Selection.activeObject = Resources.Load("Music Player/Music Library (90s)");

            if (Selection.activeObject == null)
                Debug.Log("Can't find an asset named 'Music Library (90s)'. Make sure you have 'Music Library (90s)' asset in Resources/Music Player folder.");
        }

        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Select Profile Picture Library")]
        static void SelectPPLibrary()
        {
            Selection.activeObject = Resources.Load("Profile Pictures/Profile Picture Library (90s)");

            if (Selection.activeObject == null)
                Debug.Log("Can't find an asset named 'Profile Picture Library (90s)'. Make sure you have 'Profile Picture Library (90s)' asset in Resources/Profile Pictures folder.");
        }

        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Select Wallpaper Library")]
        static void SelectWPLibrary()
        {
            Selection.activeObject = Resources.Load("Wallpapers/Wallpaper Library (90s)");

            if (Selection.activeObject == null)
                Debug.Log("Can't find an asset named 'Wallpaper Library (90s)'. Make sure you have 'Wallpaper Library (90s)' asset in Resources/Wallpapers folder.");
        }

        [MenuItem("Tools/DreamOS/Add-ons/90s Desktop/Select Web Library")]
        static void SelectWebLibrary()
        {
            Selection.activeObject = Resources.Load("Web Browser/Web Library (90s)");

            if (Selection.activeObject == null)
                Debug.Log("Can't find an asset named 'Web Library (90s)'. Make sure you have 'Web Library (90s)' asset in Resources/Web Browser folder.");
        }
        #endregion
    }
}
#endif