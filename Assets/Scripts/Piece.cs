using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Piece : MonoBehaviour {
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private MeshRenderer m_renderer;
    [SerializeField] private Collider m_collider;
    private readonly int m_pieceLayer = 6, m_previewLayer = 7;
    public float scoreWeight = 1;
    [ReadOnly][SerializeField] private BuildPlate m_buildPlate;

    public virtual void BeginHighlight() {

    }

    public virtual void Highlight() {

    }

    public virtual void EndHighlight() {

    }

    public virtual void BeginPreview() {
        gameObject.layer = m_previewLayer;
    }

    public virtual void Preview(BuildPlate plate, Vector3 point, Vector3 up, float rotation) {
        var determinedRotation = Quaternion.FromToRotation(Vector3.up, up) * Quaternion.AngleAxis(rotation, Vector3.up);
        m_rb.MoveRotation(determinedRotation);
        m_rb.MovePosition(point);

    }

    public virtual void EndPreview() {
        gameObject.layer = m_pieceLayer;
    }

    public virtual bool Place(BuildPlate plate, Vector3 point, Vector3 up, float rotation) {
        var determinedRotation = Quaternion.FromToRotation(Vector3.up, up) * Quaternion.AngleAxis(rotation, Vector3.up);
        m_rb.MoveRotation(determinedRotation);
        m_rb.MovePosition(point);

        m_rb.isKinematic = true;
        m_buildPlate = plate;
        m_buildPlate.AddPiece(this);


        return true;
    }

    public virtual bool Pick() {
        m_rb.isKinematic = true;
        if (m_buildPlate != null) {
            m_buildPlate.RemovePiece(this);
            m_buildPlate = null;
        }

        return true;
    }

    public virtual void Drop() {
        m_rb.isKinematic = false;
        if (m_buildPlate != null) {
            m_buildPlate.RemovePiece(this);
            m_buildPlate = null;
        }

    }



}
