using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "turret", menuName = "Gameplay/Turret")]
    public class TurretInfo : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Icon;
        public GameObject Prefab;
        public int Cost;

        [HideInInspector]
        public float Cooldown;
        public Vector2Int PositionInBuildingMenu;
    }
}
