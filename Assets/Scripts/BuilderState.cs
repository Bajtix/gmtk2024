using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;


public class BuilderState : PlayerState {
    [SerializeField][ReadOnly] private Piece m_previewedPiece;
    [SerializeField][ReadOnly] private Piece m_highlightedPiece;
    [SerializeField][ReadOnly] private float m_rotation;
    [SerializeField] private BuildPlate m_buildPlate;
    [SerializeField] private LayerMask m_mask;
    [SerializeField][Required] private CollectionState m_collectionState;

    private List<Piece> m_allPieces = new List<Piece>();

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
                    m_rotation = m_highlightedPiece.GetRememberedRotation();
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

        m_buildPlate.transform.Rotate(Vector3.up, 5 * Time.deltaTime * Input.GetAxis("Horizontal"));
    }

    public void SpawnPiece(Piece p) {
        var piece = Instantiate(p.Original.gameObject).GetComponent<Piece>();
        piece.transform.position = SceneConstants.Instance.BuildPlateSpawner.position;
        m_allPieces.Add(piece);
    }

    public void ReturnPiece(Piece p) {
        m_allPieces.Remove(p);
        var piece = p.Original;
        m_collectionState.SpawnPiece(piece).SendDown(p.transform.position, p.transform.rotation);
        Destroy(p.gameObject);
    }

    public void ClearPlate() {
        m_buildPlate.DestroyAllChildren();
        var arr = m_allPieces.Where(w => w.Root is not BuildPlate).ToArray();
        for (int i = 0; i < arr.Length; i++) {
            ReturnPiece(arr[i]);
        }
    }

    public void ReturnAllPieces() {
        var arr = m_allPieces.Where(w => w.Root is not BuildPlate).ToArray();
        for (int i = 0; i < arr.Length; i++) {
            ReturnPiece(arr[i]);
        }
    }

    public override void StateLeave() {
        base.StateLeave();


        if (m_previewedPiece == null) return;
        m_previewedPiece.EndPreview();
        m_previewedPiece.Drop();
        m_previewedPiece = null;

    }
}
