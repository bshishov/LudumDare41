using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "turret", menuName = "Assets/GamePlay/Turret")]
    public class TurretInfo : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Icon;
        public GameObject Prefab;
        public int Cost;
        public int Cooldown;
    }
}
