using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "turrets", menuName = "Assets/GamePlay/TurretsList")]
    public class TurretsList : ScriptableObject
    {
        [SerializeField]
        public TurretInfo[] Turrets;
    }
}
