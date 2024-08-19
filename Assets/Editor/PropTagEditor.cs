using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PropTag))]
public class PropTagEditor : Editor {
    public override void OnInspectorGUI() {
        float max = EditorGUIUtility.currentViewWidth;
        float currentX = 0;

        GUILayout.Space(200);
        if (target == null) return;
        var edited = (PropTag)target;
        float chanceSum = edited.CalculateFullWidth();
        foreach (var pc in edited.objects) {
            if (pc.prop != null) {
                var pr = AssetPreview.GetAssetPreview(pc.prop.gameObject);
                GUI.DrawTexture(new Rect(currentX, 0, max * pc.width / chanceSum, 200), pr);
                GUI.Label(new Rect(currentX, 0, max * pc.width / chanceSum, 200), $"{pc.prop.name}\n{pc.width * 100 / chanceSum:0.00}%", new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter });
            }
            currentX += max * pc.width / chanceSum;
        }

        base.OnInspectorGUI();

    }
}
