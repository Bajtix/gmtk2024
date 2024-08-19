using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class Prop : MonoBehaviour {

    [ValidateInput(nameof(ValidateTags), "Not all tags include this object!")][EnableIf(nameof(CanEditTags))] public PropTag[] tags;
    [Required] public new Collider collider;
    [SerializeField] private List<PropGenerator> m_generators = new();
    private List<int> m_generatorGroupLocks = new();

    [Space(20)][SerializeField][ReadOnly] private List<Prop> m_children = new();
    [SerializeField][ReadOnly] private Prop m_parent = null;
    [ShowNativeProperty] public Prop Root => m_parent == null ? this : m_parent.Root;
    [ShowNativeProperty] public int DescendantCount => GetAllDescendants().Count();

    private void Awake() {
        if (transform.localScale != Vector3.one)
            Debug.LogWarning("Props with non-standard scaling will cause issues for collision detection");
    }

    public Prop Initialize(Prop daddy, bool vasectomised = false) {
        if (m_parent != null || m_children.Count > 0) throw new System.Exception("Cannot initialize twice!");
        m_parent = daddy;
        if (vasectomised) return this;
        var gens = m_generators.Shuffle();
        foreach (var w in gens) {
            var child = w.Generate();
            if (child == null) continue;
            m_children.Add(child);
            child.Initialize(this, GetDescendanceLevel() > 5);
        }
        return this;
    }

    public void DestroyAllChildren(bool immediate = false) {
        m_generatorGroupLocks.Clear();
        foreach (var w in m_children) {
            try {
                w.DestroyAllChildren(immediate);
                if (!immediate) Destroy(w.gameObject);
                else DestroyImmediate(w.gameObject);
            } catch {
                //ignore
            }

        }
        m_children.Clear();
    }

    public Prop[] GetAllDescendants() {
        try {
            return m_children.Union(m_children.SelectMany(a => a.GetAllDescendants())).ToArray();
        } catch {
            return m_children.Where(w => w != null).ToArray();
        }
    }

    public int GetDescendanceLevel(int n = 0) {
        if (m_parent != null) return m_parent.GetDescendanceLevel(n + 1);
        return n + 1;
    }

    public void LockGeneratorGroup(int id) {
        m_generatorGroupLocks.Add(id);
    }

    public bool IsGeneratorGroupLocked(int id) {
        return m_generatorGroupLocks.Contains(id);
    }


    #region Editor

    private bool ValidateTags() => tags == null ||
        tags.Where(
            tag => tag != null &&
            tag.objects.Where(o => o.prop == this).Count() == 0
        ).Count() <= 0 ||
        !CanEditTags;

    private bool CanEditTags => gameObject.IsPrefabDefinition();

    [Button]
    public void Generate() {
        Initialize(null);
    }

    [Button]
    public void RemoveGenerated() {
        DestroyAllChildren(true);
    }

    [Button]
    private bool AutoAssignCollider() {
        if (collider == null) collider = gameObject.GetComponent<Collider>();
        return collider != null;
    }

    [Button]
    private void AutoFindChildGenerators() {
        m_generators.Clear();
        m_generators.AddRange(transform.GetComponentsInChildren<PropGenerator>());
    }
    #endregion

}