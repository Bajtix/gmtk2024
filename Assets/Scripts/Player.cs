using NaughtyAttributes;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField][Required] private Camera m_camera;
    [SerializeField][ReadOnly] private int m_state = int.MinValue;
    [SerializeField] private PlayerState[] m_states;
    [SerializeField] private float m_transitionSpeed = 6;
    [SerializeField] private AudioSource woosh;

    private PlayerState CurrentState => m_states[m_state];
    private Transform m_stateCameraTransform;
    private float m_stateCameraFov;

    private void SetState(int id, bool force = false) {
        if (id == m_state && !force) return;
        CurrentState.StateLeave();
        m_state = id;
        m_stateCameraFov = CurrentState.cameraFov;
        m_stateCameraTransform = CurrentState.cameraTransform != null ? CurrentState.cameraTransform : CurrentState.transform;
        CurrentState.StateEnter(m_camera);
        woosh.Play();
    }

    private void Start() {
        SetState(1, true);
    }

    private void Update() {
        transform.SetPositionAndRotation(
            Vector3.Lerp(transform.position, m_stateCameraTransform.position, Time.deltaTime * m_transitionSpeed),
            Quaternion.Lerp(transform.rotation, m_stateCameraTransform.rotation, Time.deltaTime * m_transitionSpeed)
        );
        m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, m_stateCameraFov, Time.deltaTime * m_transitionSpeed);

        if (Input.GetButtonDown("MoveDown")) {
            if (m_state <= 0) return;
            SetState(m_state - 1);
        }

        if (Input.GetButtonDown("MoveUp")) {
            if (m_state >= m_states.Length - 1) return;
            SetState(m_state + 1);
        }

        CurrentState.StateUpdate();
    }

    private void FixedUpdate() {
        CurrentState.StateFixedUpdate();
    }
}
