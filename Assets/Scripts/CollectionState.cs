using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CollectionState : PlayerState {
    [SerializeField][ReadOnly] private LibraryPiece m_draggedPiece;
    [SerializeField][ReadOnly] private LibraryPiece m_highlightedPiece;
    [SerializeField] private float m_dragStrength = 10;
    [SerializeField] private LayerMask m_mask;
    [SerializeField][Required] private BuilderState m_builderState;
    [SerializeField][Required] private Transform m_drawer, m_drawerClosedPoint, m_drawerOpenPoint;

    private List<LibraryPiece> m_allPieces = new();

    private void Start() {
        m_drawer.position = m_drawerClosedPoint.position;
    }

    private void Update() {
        m_drawer.position = Vector3.Lerp(m_drawer.position, m_active ? m_drawerOpenPoint.position : m_drawerClosedPoint.position, Time.deltaTime * 2f);
    }

    public LibraryPiece SpawnPiece(Piece p) {
        p = p.Original;
        var go = Instantiate(p.gameObject);
        go.transform.position = SceneConstants.Instance.CollectionSpawner.position;
        var lp = go.AddComponent<LibraryPiece>();
        var pp = go.GetComponent<Piece>();
        go.transform.SetParent(SceneConstants.Instance.CollectionSpawner);
        lp.renderer = pp.renderer;
        lp.rigidbody = pp.rigidbody;
        lp.objectId = pp.objectId;
        Destroy(pp);
        m_allPieces.Add(lp);
        return lp;
    }

    public void DestroyAllPieces() {
        for (int i = 0; i < m_allPieces.Count; i++) {
            Destroy(m_allPieces[0].gameObject);
        }
        m_allPieces.Clear();
    }

    public override void StateFixedUpdate() {
        var ray = m_camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 100, m_mask)) return;

        if (m_draggedPiece == null) {
            var hovered = hit.collider.GetComponent<LibraryPiece>();
            if (hovered != m_highlightedPiece) {
                if (m_highlightedPiece != null) m_highlightedPiece.EndHighlight();
                m_highlightedPiece = hovered;
                if (m_highlightedPiece != null) m_highlightedPiece.BeginHighlight();
            }
            if (m_highlightedPiece != null) m_highlightedPiece.Highlight();
        } else {

            m_draggedPiece.Drag(hit.point, m_dragStrength);

            //redundant.
            if (m_highlightedPiece != null) {
                m_highlightedPiece.EndHighlight();
                m_highlightedPiece = null;
            }
        }
    }

    public override void StateUpdate() {

        if (m_draggedPiece == null) {
            if (Input.GetButtonDown("Fire2")) {
                if (m_highlightedPiece == null || m_highlightedPiece.IsAnimating) return;
                m_highlightedPiece.EndHighlight();
                m_highlightedPiece.BeginDrag();
                m_draggedPiece = m_highlightedPiece;
                m_highlightedPiece = null;

            }

            if (Input.GetButtonDown("Fire1")) {
                if (m_highlightedPiece == null || m_highlightedPiece.IsAnimating) return;
                m_highlightedPiece.EndHighlight();
                m_highlightedPiece.SendUp();
                m_builderState.SpawnPiece(m_highlightedPiece.GetPiece());
            }
        } else {
            if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire1")) {
                m_draggedPiece.EndDrag();
                m_draggedPiece = null;
            }
        }

    }

    public override void StateEnter(Camera camera) {
        base.StateEnter(camera);
        m_builderState.ReturnAllPieces();
    }

    public override void StateLeave() {
        base.StateLeave();
        if (m_draggedPiece == null) return;
        m_draggedPiece.EndDrag();
        m_draggedPiece = null;
    }
}
