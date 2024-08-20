using UnityEngine;
using UnityEngine.Splines;

public class SceneConstants : Singleton<SceneConstants> {
    public Transform BuildPlateSpawner, CollectionSpawner;
    public SplineContainer PieceAnimationSpline;
    public Prop RootViewPlate;
}