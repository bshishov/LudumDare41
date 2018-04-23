using Assets.Scripts.Data;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerBuilder : MonoBehaviour
    {
        public float Resource;
        public KeyCode BuildKey;
        public KeyCode OpenBuildingMenuKey;

        private CharacterController _characterController;

        void Start ()
        {
            _characterController = GetComponent<CharacterController>();
        }
        
        void Update ()
        {
            if (Input.GetKeyDown(OpenBuildingMenuKey))
                UIBuildingMenu.Instance.Toggle();
            
            if (Input.GetKeyDown(BuildKey))
                TryBuild();
        }

        private void TryBuild()
        {
            if(!UIBuildingMenu.Instance.IsActive)
                return;

            var activeSelection = UIBuildingMenu.Instance.ActiveSelection;
            if (activeSelection == null)
                return;
            if (Resource < activeSelection.Cost)
            {
                UINotifications.Instance.Show(transform, "Not enough souls", Color.red);
                return;
            }

            Resource -= activeSelection.Cost;
            PlaceTurret(activeSelection);
        }

        private void PlaceTurret(TurretInfo turret)
        {
            if(!_characterController.isGrounded)
                return;

            if (turret.Prefab == null)
            {
                Debug.LogWarning("No prefab for turret");
                return;
            }

            var coords = PlacementGrid.Instance.WorldToCoords(transform.position - new Vector3(0, 0.5f, 1));
            var placementPosition = PlacementGrid.Instance.CenterOfCell(coords, 1f);

            GameObject.Instantiate(turret.Prefab, placementPosition, Quaternion.identity);
        }
    }
}
