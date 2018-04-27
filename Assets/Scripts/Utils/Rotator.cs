﻿using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class Rotator : MonoBehaviour
    {
        public float RotationSpeed = 90f;
        public Vector3 RotationDirection = new Vector3(0f, 1f, 0f);
        public Space RotationSpace = Space.World;

        void Update()
        {
            gameObject.transform.Rotate(RotationDirection * RotationSpeed * Time.deltaTime, RotationSpace);
        }
    }
}
