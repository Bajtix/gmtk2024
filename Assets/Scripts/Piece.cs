using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Piece : MonoBehaviour {
    [SerializeField][Required] private Rigidbody m_rigidbody;
    [SerializeField] private MeshRenderer m_renderer;
    [Required] public new Collider collider;
    private readonly int m_pieceLayer = 6, m_previewLayer = 7;
    public float scoreWeight = 1;

    [Space(30)][SerializeField][ReadOnly] private List<Piece> m_children = new();
    [SerializeField][ReadOnly] private Piece m_parent = null;
    [ShowNativeProperty] public Piece Root => m_parent == null ? this : m_parent.Root;
    [ShowNativeProperty] public int DescendantCount => GetAllDescendants().Count();

    public Piece[] GetAllDescendants() {
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

    public void SetDaddy(Piece daddy) {
        if (m_parent != null && daddy != null) throw new System.Exception("Cant have two daddies");
        m_parent = daddy;
    }

    public void Attach(Piece p) {
        p.SetDaddy(this);
        p.transform.SetParent(transform);
        m_children.Add(p);
    }

    public void Detach(Piece p) {
        p.SetDaddy(null);
        p.transform.SetParent(null);
        m_children.Remove(p);
    }

    public void DestroyAllChildren(bool immediate = false) {
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


    public virtual void BeginHighlight() {

    }

    public virtual void Highlight() {

    }

    public virtual void EndHighlight() {

    }

    public virtual void BeginPreview() {
        gameObject.layer = m_previewLayer;
    }

    public bool IsClearToBuild(Piece parent) {
        foreach (var p in Root.GetAllDescendants()) {
            if (Physics.ComputePenetration(collider, transform.position, transform.rotation, p.collider, p.transform.position, p.transform.rotation, out _, out float dst)) {
                if (dst > 0.001f) return false;
            }
        }

        return true;
    }

    public virtual void ComputePosition(Piece parent, Vector3 point, Vector3 up, float rotation) {
        var determinedRotation = Quaternion.FromToRotation(Vector3.up, up) * Quaternion.AngleAxis(rotation, Vector3.up);
        m_rigidbody.MoveRotation(determinedRotation);
        m_rigidbody.MovePosition(point);
    }

    public virtual void Preview(Piece parent, Vector3 point, Vector3 up, float rotation) {
        ComputePosition(parent, point, up, rotation);
    }

    public virtual void EndPreview() {
        gameObject.layer = m_pieceLayer;
    }


    public virtual bool Place(Piece parent, Vector3 point, Vector3 up, float rotation) {
        if (parent == null) return false;

        ComputePosition(parent, point, up, rotation);

        if (!IsClearToBuild(parent)) return false;

        m_rigidbody.isKinematic = true;
        parent.Attach(this);


        return true;
    }


    public virtual bool Pick() {
        DropAllChildren();
        m_rigidbody.isKinematic = true;
        if (m_parent != null) {
            m_parent.Detach(this);
        }

        return true;
    }

    protected void DropAllChildren() {
        for (int i = 0; i < m_children.Count; i++) {
            m_children[0].Drop();
        }
    }

    public virtual void Drop() {
        DropAllChildren();
        m_rigidbody.isKinematic = false;
        if (m_parent != null) {
            m_parent.Detach(this);
        }

    }

    #region Editor 

    [Button]
    private void AutoAssignComponents() {
        if (m_rigidbody == null) m_rigidbody = GetComponent<Rigidbody>();
        if (collider == null) collider = GetComponent<Collider>();
    }
    #endregion


}
