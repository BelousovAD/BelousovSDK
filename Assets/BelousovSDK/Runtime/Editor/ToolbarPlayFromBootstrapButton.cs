using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace BelousovSDK.Editor
{
    public class ToolbarPlayFromBootstrapButton
    {
        private const string IconName = "PlayButton";
        private const string Tooltip = "Play from scratch";
        private const string Path = "PlayFromScratchButton";
        
        [MainToolbarElement(Path, defaultDockPosition = MainToolbarDockPosition.Middle, ussName = "PlayMode")]
        private static MainToolbarElement GetPlayButton()
        {
            Texture2D icon = EditorGUIUtility.IconContent(IconName).image as Texture2D;
            
            return new MainToolbarButton(new MainToolbarContent(icon, Tooltip), EnterPlaymode);
        }

        private static void EnterPlaymode()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
            {
                return;
            }
                
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
            EditorApplication.EnterPlaymode();
        }
    }
}
