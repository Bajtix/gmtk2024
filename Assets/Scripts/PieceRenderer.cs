using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PieceRenderer : MonoBehaviour {

    [SerializeField] private MeshRenderer[] m_renderers;
    private Shader m_shader;
    // private Material[][] m_originalMaterials;

    // private void Start() {
    //     m_originalMaterials = new Material[m_renderers.Length][];
    //     for (int i = 0; i < m_renderers.Length; i++) {
    //         m_originalMaterials[i] = new Material[m_renderers[i].sharedMaterials.Length];
    //         Array.Copy(m_renderers[i].sharedMaterials, m_originalMaterials[i], m_originalMaterials[i].Length);
    //     }
    // }

    private void Awake() {
        m_shader = Shader.Find("Shader Graphs/PieceShader");
    }

    public void SetHighlighted() {
        if (m_shader == null) return;
        foreach (var r in m_renderers) {
            foreach (var m in r.materials) {
                m.SetKeyword(new UnityEngine.Rendering.LocalKeyword(m_shader, "_HIGHLIGHTED"), true);
            }
        }
    }

    public void SetNormal() {
        if (m_shader == null) return;
        foreach (var r in m_renderers) {
            foreach (var m in r.materials) {
                m.SetKeyword(new UnityEngine.Rendering.LocalKeyword(m_shader, "_HIGHLIGHTED"), false);
                m.SetKeyword(new UnityEngine.Rendering.LocalKeyword(m_shader, "_INVALID"), false);
                m.SetKeyword(new UnityEngine.Rendering.LocalKeyword(m_shader, "_LOOSE"), false);
            }
        }
    }

    public void SetInvalid() {
        if (m_shader == null) return;
        foreach (var r in m_renderers) {
            foreach (var m in r.materials) {
                m.SetKeyword(new UnityEngine.Rendering.LocalKeyword(m_shader, "_INVALID"), true);
            }
        }
    }

    public void SetLoose() {
        if (m_shader == null) return;
        foreach (var r in m_renderers) {
            foreach (var m in r.materials) {
                m.SetKeyword(new UnityEngine.Rendering.LocalKeyword(m_shader, "_HIGHLIGHTED"), false);
                m.SetKeyword(new UnityEngine.Rendering.LocalKeyword(m_shader, "_INVALID"), false);
                m.SetKeyword(new UnityEngine.Rendering.LocalKeyword(m_shader, "_LOOSE"), true);
            }
        }
    }

    #region Editor

    [Button]
    private void AutoAddRenderers() {
        m_renderers = GetComponentsInChildren<MeshRenderer>();
    }
    #endregion
}