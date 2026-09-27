using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BelousovSDK.Editor
{
    public class ToolbarSceneDropdown
    {
        private const string AssetsFolder = "Assets";
        private const string IconName = "SceneAsset Icon";
        private const string SceneFilter = "t:Scene";
        private const string Tooltip = "Select active scene";
        private const string Path = "Scene Selector";

        private static GUID[] s_sceneGuids;

        static ToolbarSceneDropdown()
        {
            RefreshSceneGuids();
            EditorApplication.projectChanged += RefreshSceneGuids;
            EditorSceneManager.activeSceneChangedInEditMode += SceneSwitched;
            SceneManager.activeSceneChanged += SceneSwitched;
        }

        private static void SceneSwitched(Scene oldScene, Scene newScene) =>
            MainToolbar.Refresh(Path);

        private static void RefreshSceneGuids() =>
            s_sceneGuids = AssetDatabase.FindAssetGUIDs(SceneFilter, new[] { AssetsFolder });

        [MainToolbarElement(Path, defaultDockPosition = MainToolbarDockPosition.Right)]
        private static MainToolbarElement CreateSceneSelectorDropdown()
        {
            string activeSceneName = Application.isPlaying
                ? SceneManager.GetActiveScene().name
                : EditorSceneManager.GetActiveScene().name;

            if (string.IsNullOrWhiteSpace(activeSceneName))
            {
                activeSceneName = "Untitled";
            }
            
            Texture2D icon = EditorGUIUtility.IconContent(IconName).image as Texture2D;
            MainToolbarContent content = new (activeSceneName, icon, Tooltip);
            
            return new MainToolbarDropdown(content, ShowDropdownMenu);
        }

        private static void ShowDropdownMenu(Rect rect)
        {
            GenericMenu menu = new ();

            if (s_sceneGuids.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Scenes in Project"));
            }
            else
            {
                foreach (GUID guid in s_sceneGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                    menu.AddItem(new GUIContent(scene.name), false, () => SwitchScene(path));
                }
            }

            menu.DropDown(rect);
        }

        private static void SwitchScene(string path)
        {
            if (Application.isPlaying)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                
                if (Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    Debug.Log($"Switching to scene: {path}");
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    Debug.LogError($"Scene '{path}' is not in the Build Settings.");
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        Debug.Log($"Switching to scene: {path}");
                        EditorSceneManager.OpenScene(path);
                    }
                }
                else
                {
                    Debug.LogError($"Scene at path '{path}' does not exist.");
                }
            }
        }
    }
}
