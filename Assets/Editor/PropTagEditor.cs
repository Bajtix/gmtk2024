using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PropTag))]
public class PropTagEditor : Editor {

    private float m_animT = 0;
    private Texture2D m_randomTex;

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

        if (GUILayout.Button("Get a random one!")) {
            m_animT = 0;
            m_randomTex = AssetPreview.GetAssetPreview(edited.GetRandomProp().gameObject);
        }
        if (m_randomTex != null)
            GUI.DrawTexture(new Rect((EditorGUIUtility.currentViewWidth - 100) / 2, m_animT, 100, 100), m_randomTex);

        m_animT += 10;
    }

    public override bool RequiresConstantRepaint() {
        return m_animT < 1000;
    }
}
