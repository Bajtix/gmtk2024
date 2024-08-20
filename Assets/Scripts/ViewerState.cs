
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class ViewerState : PlayerState {
    [SerializeField][ReadOnly] private int m_current = 0;
    [SerializeField][Required] private TMPro.TextMeshProUGUI m_cameraNumbertext;
    private Transform[] m_possibleAngles;
    [SerializeField][Required] private Transform m_cameraViews;
    [SerializeField][Required] private Transform m_cam;

    private void Start() {
        m_possibleAngles = m_cameraViews.GetComponentsInChildren<Transform>().Where(m => m != m_cameraViews).ToArray();
        SetCamera(0);
    }

    public override void StateEnter(Camera camera) {
        base.StateEnter(camera);
    }

    public override void StateUpdate() {
        if (Input.GetButtonDown("MoveRight")) SetCamera(m_current + 1);
        if (Input.GetButtonDown("MoveLeft")) SetCamera(m_current - 1);
    }

    public void SetCamera(int index) {
        if (index < 0) index = m_possibleAngles.Length - 1;
        m_current = index % m_possibleAngles.Length;
        m_cam.transform.SetPositionAndRotation(m_possibleAngles[m_current].position, m_possibleAngles[m_current].rotation);
    }
}
