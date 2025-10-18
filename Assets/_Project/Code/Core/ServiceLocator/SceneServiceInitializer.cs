using UnityEngine;

using _Project.Code.Gameplay.Input;


namespace _Project.Code.Core.ServiceLocator
{
    public class SceneServiceInitializer : MonoBehaviour
    {
        [SerializeField] private InputService _inputService;
        //[SerializeField] private CameraService _cameraService;

        private void Awake()
        {
            if (_inputService != null)
            {
                ServiceLocator.Register(_inputService);
            }

            //if (_cameraService != null)
            {
                //ServiceLocator.Register(_cameraService);
            }
        }
    }
}
