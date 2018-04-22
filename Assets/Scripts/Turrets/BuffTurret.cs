using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Turrets
{
    [RequireComponent(typeof(Turret))]
    [RequireComponent(typeof(Aoe))]
    public class BuffTurret : MonoBehaviour
    {
        public BuffInfo[] Buffs;

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
            if(Buffs == null || Buffs.Length == 0)
                return;

            if(_aoe.ObjectsInAoE == null || _aoe.ObjectsInAoE.Count == 0)
                return;

            foreach (var o in _aoe.ObjectsInAoE)
            {
                var buffable = o.GetComponent<Buffable>();
                if (buffable != null)
                {
                    foreach (var buffInfo in Buffs)
                    {
                        buffable.AddBuff(buffInfo);
                    }
                }
            }
        }
    }
}
