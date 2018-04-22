using UnityEngine;

namespace Assets.Scripts.Turrets
{
    [RequireComponent(typeof(Turret))]
    [RequireComponent(typeof(Aoe))]
    public class ZoneTurret : MonoBehaviour
    {
        public GameObject ZoneObj;

        private Turret _turret;
        private Aoe _aoe;

        void Start()
        {
            _turret = GetComponent<Turret>();
            _aoe = GetComponent<Aoe>();
            _turret.OnFire += TurretOnOnFire;
        }

        private void TurretOnOnFire()
        {
            if (ZoneObj == null)
                return;

            if (_aoe.ActivePath == null)
            {
                Debug.LogWarning("No path for turret", gameObject);
                return;
            }

            foreach (var point in _aoe.ActivePath)
                GameObject.Instantiate(ZoneObj, point, Quaternion.identity);
        }
    }
}
