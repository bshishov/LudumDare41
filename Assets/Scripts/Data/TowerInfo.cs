using UnityEngine;

namespace Assets.Scripts.Data
{
    public enum AoEType
    {
        Segments,
        SegmentsBothSides,
        Spherical,
    }

    public enum TowerEffectType
    {
        ShootProjectile,
        Ray,
        ApplyBuff,
    }

    public class TowerInfo : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Icon;
        public GameObject Prefab;

        public AoEType AoEType;
        public TowerEffectType EffectType;
        public int Cost;
    }
}
