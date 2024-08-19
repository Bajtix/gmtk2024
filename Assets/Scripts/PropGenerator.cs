using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class PropGenerator : MonoBehaviour {
    [Required][SerializeField] private Prop m_owner;
    [SerializeField][Expandable] private PropTag m_objects;
    [SerializeField] private float m_chance = 1;
    [SerializeField] private int m_maxAttempts = 10;
    [SerializeField] private bool m_lockGroup;
    [ShowIf(nameof(m_lockGroup))][SerializeField] private int m_generatorGroup;

    public Prop Generate() {
        if (m_owner.IsGeneratorGroupLocked(m_generatorGroup) && m_lockGroup) {
            return null;
        }

        if (Random.Range(0, 1f) >= m_chance) {
            return null;
        }

        int attempt = 0;
        Prop instance = null;

        while (instance == null && attempt < m_maxAttempts) {
            var prop = m_objects.GetRandomProp();
            instance = Instantiate(prop.gameObject, transform.position, transform.rotation, m_owner.Root.transform).GetComponent<Prop>();
            instance.gameObject.name = instance.gameObject.name.Replace("(Clone)", $" - {System.Guid.NewGuid().ToString()[0..5]}");
            foreach (var a in m_owner.Root.GetAllDescendants().Prepend(m_owner.Root)) {
                bool intersect = Physics.ComputePenetration(instance.collider,
                    instance.transform.position,
                    instance.transform.rotation,
                    a.collider,
                    a.transform.position,
                    a.transform.rotation,
                    out _, out float resolveDst
                );

                if (intersect && resolveDst > 0.001f) {
                    Debug.Log($"Prop {instance.name} cannot be here, as it collides with {a.name}. (Attempt {attempt})");
                    DestroyImmediate(instance.gameObject);
                    break;
                }
            }
            attempt++;
        }

        if (instance == null) {
            Debug.Log("No object was placed.");
        } else if (m_lockGroup) {
            m_owner.LockGeneratorGroup(m_generatorGroup);
        }

        return instance;
    }

    private void OnDrawGizmos() {
        //Gizmos.DrawIcon(transform.position, "spawn");

        if (m_lockGroup) Gizmos.color = Color.HSVToRGB(m_generatorGroup % 4 / 4f, 1f, .8f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.2f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero + Vector3.up * 0.15f, Vector3.one * 0.1f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero + Vector3.up * 0.225f, Vector3.one * 0.05f);
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
