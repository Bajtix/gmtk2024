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
}