using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Tag", menuName = "Game/Tag")]
public class PropTag : ScriptableObject {
    [System.Serializable]
    public struct PropProbability {
        public Prop prop;
        [AllowNesting][MinValue(0.001f)] public float width;

        public PropProbability(Prop prop) : this() {
            this.prop = prop;
        }
    }

    [ValidateInput(nameof(ValidateProps), "Not all tags include this object!")]
    public List<PropProbability> objects;

    public Prop GetRandomProp() {
        float chanceSum = CalculateFullWidth();
        float randomValue = Random.Range(0, 1f);
        int selected = 0;
        while (selected < objects.Count()) {
            randomValue -= objects[selected].width / chanceSum;
            if (randomValue <= 0) break;
            selected++;
        }
        return objects[selected].prop;
    }

    public bool Contains(Prop p) {
        return objects.Where(a => a.prop == p).Count() > 0;
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