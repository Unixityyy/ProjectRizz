using UnityEngine;
using UnityEditor;
using TMPro;

public class AutoApplyFont : EditorWindow
{
    private TMP_FontAsset customFont;

    [MenuItem("Tools/Auto Apply Custom Font")]
    public static void ShowWindow()
    {
        GetWindow<AutoApplyFont>("Auto Apply Custom Font");
    }

    void OnGUI()
    {
        GUILayout.Label("Select Custom Font", EditorStyles.boldLabel);
        customFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Font Asset", customFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Apply Font and Uppercase"))
        {
            ApplyFontAndUppercase();
        }
    }

    void ApplyFontAndUppercase()
    {
        TMP_Text[] allTextComponents = Resources.FindObjectsOfTypeAll<TMP_Text>(); // Includes disabled objects
        foreach (TMP_Text textComponent in allTextComponents)
        {
            if (textComponent.hideFlags == HideFlags.None && !EditorUtility.IsPersistent(textComponent))
            {
                textComponent.font = customFont;
                textComponent.text = textComponent.text.ToUpper();
                EditorUtility.SetDirty(textComponent); // Mark the object as dirty to save changes
            }
        }
        Debug.Log("Custom font applied and text converted to uppercase for all TMP text objects, including disabled ones.");
    }
}
