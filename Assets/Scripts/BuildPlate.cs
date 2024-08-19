using System.Collections.Generic;
using UnityEngine;

public class BuildPlate : MonoBehaviour {
    [SerializeField] private List<Piece> m_pieces;

    public Material pieceMaterial;

    public void AddPiece(Piece p) {
        m_pieces.Add(p);
        p.transform.SetParent(transform);
    }

    public void RemovePiece(Piece p) {
        m_pieces.Remove(p);
        p.transform.SetParent(null);
    }


}
