using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Collection : PlayerState {
    [SerializeField][ReadOnly] private LibraryPiece m_draggedPiece;
    [SerializeField][ReadOnly] private LibraryPiece m_highlightedPiece;
    [SerializeField] private float m_dragStrength = 10;
    [SerializeField] private LayerMask m_mask;
    [SerializeField] private Builder m_builderState;
    [SerializeField] private Transform m_pieceSpawnPoint;

    [SerializeField] private Piece[] m_debugStartPieces;

    private void Start() {
        foreach (var m in m_debugStartPieces) {
            SpawnPiece(m);
        }
    }

    public LibraryPiece SpawnPiece(Piece p) {
        var go = Instantiate(p.gameObject);
        go.transform.position = m_pieceSpawnPoint.position;
        var lp = go.AddComponent<LibraryPiece>();
        var pp = go.GetComponent<Piece>();
        lp.renderer = pp.renderer;
        lp.rigidbody = pp.rigidbody;
        lp.linkedPiece = p;
        Destroy(pp);
        return lp;
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
                if (m_highlightedPiece == null) return;
                m_highlightedPiece.EndHighlight();
                m_highlightedPiece.BeginDrag();
                m_draggedPiece = m_highlightedPiece;
                m_highlightedPiece = null;

            }

            if (Input.GetButtonDown("Fire1")) {
                if (m_highlightedPiece == null) return;
                m_highlightedPiece.EndHighlight();
                m_highlightedPiece.SendUp();
                m_builderState.SpawnPiece(m_highlightedPiece.linkedPiece);
            }
        } else {
            if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire1")) {
                m_draggedPiece.EndDrag();
                m_draggedPiece = null;
            }
        }

    }

    public override void StateLeave() {
        base.StateLeave();
        if (m_draggedPiece == null) return;
        m_draggedPiece.EndDrag();
        m_draggedPiece = null;
    }
}
