using UnityEngine;

namespace Assets.Scripts.Turrets
{
    [RequireComponent(typeof(Turret))]
    [RequireComponent(typeof(Aoe))]
    public class RayTurret : MonoBehaviour
    {
        public Vector3 SourcePoint;
        public GameObject Ray;

        private Turret _turret;
        private Aoe _aoe;
        private Buffable _buffable;

        void Start()
        {
            _buffable = GetComponent<Buffable>();
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
                ray.DamageMultiplier = _buffable.DamageMultiplier;

                if (_buffable.AttackModifiers.Count > 0)
                    ray.HitEffects.AddRange(_buffable.AttackModifiers);

                var path = _aoe.ActivePath.ToArray();
                path[0] = transform.TransformPoint(SourcePoint);

                // THIS CALL SHOULD BE THE LAST
                // otherwise it will not apply damage multipliers and attack modifiers
                ray.SetPath(path, _aoe);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.TransformPoint(SourcePoint), 0.5f);
        }
    }
}
