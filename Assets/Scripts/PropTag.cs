using System.Linq;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Tag", menuName = "Game/Tag")]
public class PropTag : ScriptableObject {
    [System.Serializable]
    public struct PropProbability {
        public Prop prop;
        [AllowNesting][MinValue(0.001f)] public float width;
    }

    [ValidateInput(nameof(ValidateProps), "Not all tags include this object!")]
    public PropProbability[] objects;

    public Prop GetRandomProp() {
        float chanceSum = CalculateFullWidth();
        float randomValue = Random.Range(0, 1);
        int selected = 0;
        while (randomValue > 0 && selected < objects.Count()) {
            randomValue -= objects[selected].width / chanceSum;
            selected++;
        }
        return objects[selected].prop;
    }

    public float CalculateFullWidth() => objects.Sum(prop => prop.width);


    #region Editor
    private bool ValidateProps() => objects == null ||
        objects.Where(
            w => w.prop != null &&
            !w.prop.tags.Contains(this)
        ).Count() <= 0;
    #endregion
}