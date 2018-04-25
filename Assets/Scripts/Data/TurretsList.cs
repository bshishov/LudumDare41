using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "turrets", menuName = "Gameplay/TurretsList")]
    public class TurretsList : ScriptableObject
    {
        [SerializeField]
        public TurretInfo[] Turrets;
    }
}
