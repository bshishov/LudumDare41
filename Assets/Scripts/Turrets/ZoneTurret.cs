using UnityEngine;

namespace Assets.Scripts.Turrets
{
    [RequireComponent(typeof(Turret))]
    [RequireComponent(typeof(Aoe))]
    [RequireComponent(typeof(Buffable))]
    public class ZoneTurret : MonoBehaviour
    {
        public GameObject ZoneObj;

        private Turret _turret;
        private Aoe _aoe;
        private Buffable _buffable;

        void Start()
        {
            _turret = GetComponent<Turret>();
            _aoe = GetComponent<Aoe>();
            _turret.OnFire += TurretOnOnFire;
            _buffable = GetComponent<Buffable>();
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
            {
                var zoneObj = GameObject.Instantiate(ZoneObj, point, Quaternion.identity);

                var zone = zoneObj.GetComponent<Zone>();
                if(zone != null)
                    zone.DamageMultiplier = _buffable.DamageMultiplier;
            }
        }
    }
}
