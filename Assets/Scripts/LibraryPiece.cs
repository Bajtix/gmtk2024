using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class LibraryPiece : MonoBehaviour {
    public string objectId;
    [Required] public new Rigidbody rigidbody;
    [Required] public new PieceRenderer renderer;
    private readonly int m_libaryPieceLayer = 8, m_previewLayer = 7;

    private int m_animationPlaying;
    private float m_animationProgress;
    private Vector3 m_animationInitialPosition;
    private Quaternion m_animationInitialRotation;
    private Vector3 m_randomPositionOffset;

    public bool IsAnimating => m_animationPlaying != 0;

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
        m_animationInitialPosition = transform.position;
        m_animationInitialRotation = transform.rotation;
        m_animationPlaying = 1;
        m_animationProgress = 0;
        BeginDrag();
    }

    public void SendDown(Vector3 position, Quaternion rotation) {
        m_randomPositionOffset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        m_animationInitialPosition = position;
        m_animationInitialRotation = rotation;
        m_animationPlaying = -1;
        m_animationProgress = 0;
        BeginDrag();
    }

    public Piece GetPiece() {
        return ObjectRegistry.GetPiece(objectId);
    }

    private void Update() {
        if (m_animationPlaying != 0) {
            rigidbody.isKinematic = true;

            m_animationProgress += Time.deltaTime;

            if (m_animationProgress >= 1) {
                if (m_animationPlaying > 0) {
                    Destroy(gameObject);
                } else {
                    m_animationPlaying = 0;
                    EndDrag();
                }
            }

            transform.position =
                Vector3.Lerp(
                    (Vector3)SceneConstants.Instance.PieceAnimationSpline.EvaluatePosition(m_animationPlaying > 0 ? m_animationProgress : 1 - m_animationProgress) + m_randomPositionOffset,
                    Vector3.Lerp(m_animationInitialPosition, m_animationPlaying > 0 ? SceneConstants.Instance.BuildPlateSpawner.position : SceneConstants.Instance.CollectionSpawner.position + m_randomPositionOffset, m_animationProgress),
                    Mathf.Abs(m_animationProgress - 0.5f) * 2f
                );
            transform.rotation = Quaternion.Lerp(m_animationInitialRotation, Quaternion.identity, m_animationProgress);
        } else {
            rigidbody.isKinematic = false;
        }
    }

}
