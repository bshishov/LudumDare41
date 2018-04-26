using UnityEngine;

namespace Assets.Scripts.Turrets
{
    [RequireComponent(typeof(Turret))]
    [RequireComponent(typeof(Aoe))]
    public class ProjectileTurret : MonoBehaviour
    {
        public GameObject Projectile;

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
            if (Projectile == null)
                return;

            if (_aoe.ObjectsInAoE == null || _aoe.ObjectsInAoE.Count == 0)
                return;

            var projectileObj = GameObject.Instantiate(Projectile, transform.position, Quaternion.identity);
            var pathFollow = projectileObj.GetComponent<PathFollow>();
            if (pathFollow != null)
            {
                pathFollow.Speed *= _buffable.SpeedMultiplier;
                pathFollow.Go(_aoe.ActivePath);
            }

            var projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                if (_buffable.AttackModifiers.Count > 0)
                    projectile.HitEffects.AddRange(_buffable.AttackModifiers);
            }
        }
    }
}
