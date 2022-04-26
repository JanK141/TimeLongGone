using UnityEngine;

namespace Content.Scripts.Camera
{
    public class CameraScript : MonoBehaviour
    {
        [SerializeField] private GameObject mainCamera;

        public enum View
        {
            Arena,
            Player,
        }

        public View ActiveView { get; set; }
        public Transform mainCameraTransform;

        public static CameraScript Instance;

        [HideInInspector] public ArenaCameraSet arenaSet;

        [HideInInspector] public PlayerCameraSet playerCameraSet;


        private void Awake()
        {
            if (Instance == null) Instance = this;
            arenaSet = GetComponent<ArenaCameraSet>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            mainCameraTransform = mainCamera.GetComponent<Transform>();
            playerCameraSet = GetComponent<PlayerCameraSet>();
        }
    }
}