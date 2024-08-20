using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private BuilderState m_buildState;
    [SerializeField] private CollectionState m_collectionState;
    [SerializeField] private BuildPlate m_buildPlate;
    [SerializeField] private Prop m_viewerPlate;
    [SerializeField] private TextMeshProUGUI m_scoreDisplay;

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

    public void CalculateScore() {
        FindObjectOfType<Player>().SetState(1);
        FindObjectOfType<Player>().enabled = false;
        StartCoroutine(nameof(DoScoring));
    }



    private IEnumerator DoScoring() {
        yield return null;

        float score = 0;

        void Score(float s) {
            m_scoreDisplay.text = $"{score:0.00}";
        }

        Vector3 GetPositionRelative(Transform to, Vector3 pos) {
            return to.InverseTransformPoint(pos);
        }

        var props = m_viewerPlate.GetAllDescendants();
        var pieces = m_buildPlate.GetAllDescendants().ToList();

        foreach (var entry in ObjectRegistry.Instance.registryEntries) {
            var selectedProps = props.Where(a => a.objectId == entry.name).ToList();
            var selectedPieces = pieces.Where(a => a.objectId == entry.name).ToList();

            while (selectedPieces.Count > 0 && selectedProps.Count > 0) {

                Prop smallestProp = null;
                Piece smallestPiece = null;
                float smallestDst = float.PositiveInfinity;
                for (int i = 0; i < selectedProps.Count; i++) {
                    var prop = selectedProps[i];
                    var closestOne = selectedPieces.MinBy(a => Vector3.Distance(GetPositionRelative(m_buildPlate.transform, a.transform.position), GetPositionRelative(m_viewerPlate.transform, prop.transform.position)));
                    Debug.Log($"Piece: {closestOne.gameObject.name}, pos: {GetPositionRelative(m_buildPlate.transform, closestOne.transform.position)}");
                    Debug.Log($"Prop: {prop.gameObject.name}, pos: {GetPositionRelative(m_viewerPlate.transform, prop.transform.position)}");
                    float dst = selectedPieces.Min(a => Vector3.Distance(GetPositionRelative(m_buildPlate.transform, a.transform.position), GetPositionRelative(m_viewerPlate.transform, prop.transform.position)));
                    //najbliższy piece dla danego propa
                    if (dst < smallestDst) {
                        smallestDst = dst;
                        smallestProp = selectedProps[i];
                        smallestPiece = closestOne;
                    }
                }
                selectedPieces.Remove(smallestPiece);
                selectedProps.Remove(smallestProp);

                smallestPiece.renderer.SetHighlighted();
                smallestProp.transform.localScale = Vector3.one * 0.5f;
                Debug.Log($"Found smallest dst between {smallestPiece.gameObject.name} and {smallestProp.gameObject.name} to be {smallestDst}");
                Score(Mathf.Clamp(10 - smallestDst, 0, 10));
                yield return new WaitForSeconds(0.2f);
            }

            // pokaż przycisk back
        }

        // foreach (var prop in props) {
        //     var matchingObjectsInPieces = pieces.Where(w => w.objectId == prop.objectId);
        //     if (matchingObjectsInPieces.Count() == 0) {
        //         Score(-10); //missing object
        //         continue;
        //     }

        //     closestOne.renderer.SetHighlighted();
        //     pieces.Remove(closestOne);
        //     Debug.Log(closestOne.gameObject.name);
        // }
    }

    private void Start() {
        BeginRound();
    }
}
