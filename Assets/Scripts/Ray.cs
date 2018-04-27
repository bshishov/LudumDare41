using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(LineRenderer))]
    public class Ray : MonoBehaviour
    {
        public float TTL = 2f;

        [TagSelector]
        public string Target = Tags.Enemy;

        private LineRenderer _line;

        [SerializeField]
        public List<Effect> HitEffects;

        public float DamageMultiplier;

        void Start()
        {
            _line = GetComponent<LineRenderer>();
            _line.useWorldSpace = true;
            Destroy(gameObject, TTL);
        }
        
        public void SetPath(Vector3[] path, Aoe aoe)
        {
            if (_line == null)
                _line = GetComponent<LineRenderer>();

            _line.positionCount = path.Length;
            _line.SetPositions(path);

            if (aoe.ObjectsInAoE != null)
            {
                foreach (var o in aoe.ObjectsInAoE)
                {
                    if(o == null)
                        continue;

                    if (o.CompareTag(Target))
                    {
                        var enemy = o.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            foreach (var hitEffect in HitEffects)
                            {
                                enemy.ApplyEffect(hitEffect);
                            }
                        }
                    }
                }
            }
        }
    }
}
