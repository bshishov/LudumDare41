using UnityEngine;

namespace Assets.Scripts.Turrets
{
    [RequireComponent(typeof(Turret))]
    [RequireComponent(typeof(Aoe))]
    public class RayTurret : MonoBehaviour {

        public GameObject Ray;

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
            if (Ray == null)
                return;

            if (_aoe.ObjectsInAoE == null || _aoe.ObjectsInAoE.Count == 0)
                return;

            var rayObj = GameObject.Instantiate(Ray, transform.position, Quaternion.identity);

            var ray = rayObj.GetComponent<Ray>();
            if (ray != null)
            {
                ray.SetPath(_aoe.ActivePath, _aoe);
            }
        }
    }
}
