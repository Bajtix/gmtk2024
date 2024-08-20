using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LibraryPiece : MonoBehaviour {
    public new Rigidbody rigidbody;
    public new PieceRenderer renderer;
    public Piece linkedPiece;
    private readonly int m_libaryPieceLayer = 8, m_previewLayer = 7;

    private void Start() {
        renderer.SetLoose();
    }

    public void BeginHighlight() {
        renderer.SetHighlighted();
    }

    public void Highlight() {

    }

    public void EndHighlight() {
        renderer.SetLoose();
    }

    public void BeginDrag() {
        gameObject.layer = m_previewLayer;
        renderer.SetNormal();
    }

    public void Drag(Vector3 destination, float forceMultiplier) {
        rigidbody.AddForce((destination - rigidbody.position) * forceMultiplier);
    }

    public void EndDrag() {
        gameObject.layer = m_libaryPieceLayer;
        renderer.SetLoose();
    }

    public void SendUp() {
        gameObject.SetActive(false);
    }

    public void SendDown() {
        gameObject.SetActive(true);
    }


}
