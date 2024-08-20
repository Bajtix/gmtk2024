using NaughtyAttributes;
using UnityEngine;

public class PlayerState : MonoBehaviour {
    public Transform cameraTransform;
    public float cameraFov = 60f;
    protected Camera m_camera;
    protected bool m_active;

    public virtual void StateEnter(Camera camera) {
        m_active = true;
        m_camera = camera;
    }

    public virtual void StateUpdate() {

    }

    public virtual void StateFixedUpdate() {

    }

    public virtual void StateLeave() {
        m_active = false;
        m_camera = null;
    }

    [Button]
    private void TestMainCamera() {
        if (cameraTransform != null) {
            Camera.main.transform.position = cameraTransform.position;
            Camera.main.transform.rotation = cameraTransform.rotation;
        } else {
            Camera.main.transform.position = transform.position;
            Camera.main.transform.rotation = transform.rotation;
        }

        Camera.main.fieldOfView = cameraFov;
    }
}