using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private BuilderState m_buildState;
    [SerializeField] private CollectionState m_collectionState;

    public void BeginRound() {
        m_buildState.ClearPlate();
        m_collectionState.DestroyAllPieces();
        SceneConstants.Instance.RootViewPlate.Generate();
        var pieces = SceneConstants.Instance.RootViewPlate.GetAllDescendants()
            .Where(a => !string.IsNullOrWhiteSpace(a.objectId))
            .Select(a => ObjectRegistry.GetPiece(a.objectId));
        foreach (var piece in pieces) {
            if (piece == null) continue;
            m_collectionState.SpawnPiece(piece);
        }
    }

    private void Start() {
        BeginRound();
    }
}
