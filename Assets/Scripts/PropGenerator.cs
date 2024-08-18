using UnityEngine;
using NaughtyAttributes;

public class PropGenerator : MonoBehaviour {
    [SerializeField] private Prop m_owner;
    [Expandable] public PropTag objects;
    public float chance = 1;
    public int maxAttempts = 10;

    public Prop Generate() {
        if (Random.Range(0, 1f) >= chance) return null;
        int attempt = 0;
        Prop instance = null;

        while (instance == null && attempt < maxAttempts) {
            var prop = objects.GetRandomProp();
            instance = Instantiate(prop.gameObject, transform.position, transform.rotation, m_owner.Root.transform).GetComponent<Prop>();
            instance.gameObject.name = instance.gameObject.name.Replace("(Clone)", $" - {System.Guid.NewGuid().ToString()[0..5]}");
            foreach (var a in m_owner.Root.GetAllDescendants()) {
                bool intersect = Physics.ComputePenetration(instance.collider, instance.transform.position, instance.transform.rotation, a.collider, a.transform.position, a.transform.rotation, out _, out float resolveDst);
                if (intersect && resolveDst > 0.001f) {
                    Debug.Log($"Prop {instance.name} cannot be here, as it collides with {a.name}. (Attempt {attempt})");
                    DestroyImmediate(instance.gameObject);
                    break;
                }
            }
            attempt++;
        }

        return instance;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "spawn", false);
    }

    #region Editor Helpers
    [Button]
    private bool AutoAssignOwner() {
        if (transform.parent == null) return false;
        if (m_owner == null) m_owner = transform.GetComponentInParent<Prop>();
        return m_owner != null;
    }

    #endregion
}
