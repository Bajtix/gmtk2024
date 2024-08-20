using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Registry", menuName = "Game/Registry")]
public class ObjectRegistry : ScriptableObject {
    private static ObjectRegistry m_instance;
    public static ObjectRegistry Instance {
        get {
            if (m_instance == null) {
                m_instance = Resources.Load<ObjectRegistry>("Registry");
            }
            return m_instance;
        }
    }

    [System.Serializable]
    public struct RegistryEntry {
        public string name;
        public Prop prop;
        public Piece piece;
    }

    public List<RegistryEntry> registryEntries;

    public static Piece GetPiece(string id) {
        var found = Instance.registryEntries.Where(a => a.name == id);
        if (found.Count() == 0) return null;
        return found.First().piece;
    }

    public static Prop GetProp(string id) {
        var found = Instance.registryEntries.Where(a => a.name == id);
        if (found.Count() == 0) return null;
        return found.First().prop;
    }

    [Button]
    public void FixEmptyIds() {
        foreach (var w in registryEntries) {
            if (w.prop != null && string.IsNullOrWhiteSpace(w.prop.objectId)) w.prop.objectId = w.name;
            if (w.piece != null && string.IsNullOrWhiteSpace(w.piece.objectId)) w.piece.objectId = w.name;
        }
    }


}
