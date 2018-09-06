using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LZWPlib.Core))]
public class CoreEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Prepare project for CAVE"))
        {
            UnityEditorInternal.VR.VREditor.SetVREnabledOnTargetGroup(UnityEditor.BuildTargetGroup.Standalone, true);
            UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(UnityEditor.BuildTargetGroup.Standalone, new string[] { "split", "stereo", "None" });

            Application.runInBackground = true;
            PlayerSettings.visibleInBackground = true;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;

            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows, false);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);

            UnityEngine.Rendering.GraphicsDeviceType[] devices = new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.Direct3D12, UnityEngine.Rendering.GraphicsDeviceType.Direct3D11, UnityEngine.Rendering.GraphicsDeviceType.Vulkan, UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore };

            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows, devices);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, devices);

            if (Application.HasProLicense())
                PlayerSettings.SplashScreen.show = false;




            Debug.Log("Settings have been adjusted!");
        }
    }
}