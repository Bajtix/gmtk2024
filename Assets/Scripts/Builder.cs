using UnityEngine;

public class Builder : PlayerState {
    [SerializeField] private Piece m_previewedPiece;
    [SerializeField] private Piece m_highlightedPiece;
    [SerializeField] private float m_rotation;
    [SerializeField] private BuildPlate m_buildPlate;
    [SerializeField] private LayerMask m_mask;

    public override void StateFixedUpdate() {
        var ray = m_camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 100, m_mask)) return;

        if (m_previewedPiece == null) {
            var hovered = hit.collider.GetComponent<Piece>();
            if (hovered != m_highlightedPiece) {
                if (m_highlightedPiece != null) m_highlightedPiece.EndHighlight();
                m_highlightedPiece = hovered;
                if (m_highlightedPiece != null) m_highlightedPiece.BeginHighlight();
            }
            if (m_highlightedPiece != null) m_highlightedPiece.Highlight();
        } else {

            m_previewedPiece.Preview(hit.collider.GetComponent<Piece>(), hit.point, hit.normal, m_rotation);

            //redundant.
            if (m_highlightedPiece != null) {
                m_highlightedPiece.EndHighlight();
                m_highlightedPiece = null;
            }
        }
    }

    public override void StateUpdate() {

        if (m_previewedPiece == null) {
            if (Input.GetButtonDown("Fire2")) {
                if (m_highlightedPiece == null) return;
                if (m_highlightedPiece.Pick()) {
                    m_highlightedPiece.EndHighlight();
                    m_highlightedPiece.BeginPreview();
                    m_previewedPiece = m_highlightedPiece;
                    m_highlightedPiece = null;
                }
            }
        } else {
            m_rotation += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 4000;
            if (Input.GetButtonDown("Fire1")) {
                var ray = m_camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit, 100, m_mask)) return;
                if (m_previewedPiece.Place(hit.collider.GetComponent<Piece>(), hit.point, hit.normal, m_rotation)) {
                    m_previewedPiece.EndPreview();
                    m_previewedPiece = null;
                }
            }
            if (Input.GetButtonDown("Fire2")) {
                m_previewedPiece.EndPreview();
                m_previewedPiece.Drop();
                m_previewedPiece = null;
            }
        }

    }

    public override void StateLeave() {
        base.StateLeave();
        if (m_previewedPiece == null) return;
        m_previewedPiece.Drop();
        m_previewedPiece = null;
    }
}
