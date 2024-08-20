using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PieceRenderer))]
public class Piece : MonoBehaviour {
    [ValidateInput(nameof(ValidateId), "Object id does not match the registry")][EnableIf(nameof(IsPrefab))] public string objectId;
    [Required] public new Rigidbody rigidbody;
    [Required] public new Collider collider;
    [Required] public new PieceRenderer renderer;
    private readonly int m_pieceLayer = 6, m_previewLayer = 7;
    public float scoreWeight = 1;
    public int maxDescendance = 4;
    private float m_rememberedRotation;

    [Space(30)][SerializeField][ReadOnly] private List<Piece> m_children = new();
    [SerializeField][ReadOnly] private Piece m_parent = null;
    [ShowNativeProperty] public Piece Root => m_parent == null ? this : m_parent.Root;
    [ShowNativeProperty] public int DescendantCount => GetAllDescendants().Count();

    public Piece Original => ObjectRegistry.GetPiece(objectId);

    private void Awake() {
        gameObject.layer = m_pieceLayer;

    }

    private void Start() {
        if (rigidbody.isKinematic)
            renderer.SetNormal();
        else
            renderer.SetLoose();
    }

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

    public bool IsClearToBuild(Piece parent) {
        if (parent == null) return false;
        if (parent.Root is not BuildPlate) return false;
        if (parent.GetDescendanceLevel() > maxDescendance) return false;

        foreach (var p in parent.Root.GetAllDescendants().Append(parent.Root)) {
            if (Physics.ComputePenetration(collider, transform.position, transform.rotation, p.collider, p.transform.position, p.transform.rotation, out _, out float dst)) {
                if (dst > 0.001f) {
                    Debug.Log(p.gameObject.name + "is preventing placement");
                    return false;
                }
            }
        }

        return true;
    }

    public virtual void ComputePosition(Piece parent, Vector3 point, Vector3 up, float rotation) {
        var determinedRotation = Quaternion.FromToRotation(Vector3.up, up) * Quaternion.AngleAxis(rotation, Vector3.up);
        rigidbody.MoveRotation(determinedRotation);
        rigidbody.MovePosition(point);
    }


    public virtual void BeginHighlight() {
        renderer.SetHighlighted();
    }

    public virtual void Highlight() {

    }

    public virtual void EndHighlight() {
        if (rigidbody.isKinematic)
            renderer.SetNormal();
        else
            renderer.SetLoose();
    }

    public virtual void BeginPreview() {
        gameObject.layer = m_previewLayer;
    }

    public virtual void Preview(Piece parent, Vector3 point, Vector3 up, float rotation) {
        ComputePosition(parent, point, up, rotation);

        if (!IsClearToBuild(parent)) // potentially expensive!
            renderer.SetInvalid();
        else
            renderer.SetNormal();

    }

    public virtual void EndPreview() {
        gameObject.layer = m_pieceLayer;
        collider.enabled = true;
    }


    public virtual bool Place(Piece parent, Vector3 point, Vector3 up, float rotation) {
        if (parent == null) return false;

        ComputePosition(parent, point, up, rotation);

        if (!IsClearToBuild(parent)) return false;

        rigidbody.isKinematic = true;
        parent.Attach(this);
        m_rememberedRotation = rotation;
        renderer.SetNormal();

        return true;
    }

    public float GetRememberedRotation() => m_rememberedRotation;

    public virtual bool Pick() {
        DropAllChildren();
        rigidbody.isKinematic = true;
        if (m_parent != null) {
            m_parent.Detach(this);
        }

        return true;
    }

    protected void DropAllChildren() {
        var children = m_children.ToArray();
        foreach (var w in children) w.Drop();
    }

    public virtual void Drop() {
        DropAllChildren();
        renderer.SetLoose();
        rigidbody.isKinematic = false;
        if (m_parent != null) {
            m_parent.Detach(this);
        }

    }

    #region Editor 

    [Button]
    private void AutoAssignComponents() {
        if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();
        if (collider == null) collider = GetComponent<Collider>();
        if (renderer == null) renderer = GetComponent<PieceRenderer>();
    }

    private bool IsPrefab =>
#if UNITY_EDITOR
    gameObject.IsPrefabDefinition();
#else
    false;
#endif

    private bool ValidateId() {
        return ObjectRegistry.GetPiece(objectId) == this || !IsPrefab;
    }

    #endregion


}
