using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class PlayerCheats : MonoBehaviour
    {
        public KeyCode AddSoulsKey;
        public KeyCode SpawnDummyKey;
        public KeyCode RemoveDummyKey;

        public GameObject DummyPrefab;

        private GameObject _currentDummy;

        void Start ()
        {

        }
        
        void Update()
        {
            if (Input.GetKeyDown(AddSoulsKey) && Input.GetKey(KeyCode.RightControl))
                AddSouls();

            if (Input.GetKeyDown(SpawnDummyKey) && Input.GetKey(KeyCode.RightControl))
                SpawnDummy();

            if (Input.GetKeyDown(RemoveDummyKey) && Input.GetKey(KeyCode.RightControl))
                RemoveDummy();
        }

        private void AddSouls()
        {
            var builder = GetComponent<PlayerBuilder>();
            if(builder == null)
                return;

            builder.AddSouls(1000);
        }

        private void SpawnDummy()
        {
            if (DummyPrefab == null)
                return;

            if(_currentDummy != null)
                Destroy(_currentDummy);

            _currentDummy = Instantiate(DummyPrefab, transform.position, Quaternion.identity);
        }

        private void RemoveDummy()
        {
            if (_currentDummy != null)
            {
                Destroy(_currentDummy);   
            }
        }
    }
}
