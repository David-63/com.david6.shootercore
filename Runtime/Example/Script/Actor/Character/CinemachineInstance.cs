using Unity.Cinemachine;
using UnityEngine;

namespace David6.ShooterFramework
{
    public class CinemachineInstance : MonoBehaviour
    {
        public static CinemachineInstance Instance;

        public CinemachineCamera FirstPersonCamera;
        public CinemachineCamera ThirdPersonCamera;
        private bool _firstPerson;
        
        private float defaultFovFps = 60;
        private float defaultFovTps = 60;
        
        private float lastFov = 0;
        private float oldFov = 60;
        private float fovSmoothing = 0;
        private float fovSmoothT = 0;

        // stored third person values
        private Vector3 _smoothing;
        private float _cameraDistance;


        public CinemachineInstance()
		{
			Instance = this;
		}
		
		private void Awake()
		{
			defaultFovFps = FirstPersonCamera.Lens.FieldOfView;            
			defaultFovTps = ThirdPersonCamera.Lens.FieldOfView;
			SetFirstPerson(_firstPerson);
		}

		private void Update()
		{
			if (fovSmoothT > 0)
			{
				fovSmoothT -= fovSmoothing * Time.deltaTime;
				// smooth fov change
				if (FirstPerson)
				{
					FirstPersonCamera.Lens.FieldOfView = Mathf.Lerp(oldFov, lastFov, 1 - fovSmoothT);
					if (fovSmoothT < 0.001f)
					{
						FirstPersonCamera.Lens.FieldOfView = lastFov;
						lastFov = 0;
					}
				}
				else
				{
					ThirdPersonCamera.Lens.FieldOfView = Mathf.Lerp(oldFov, lastFov, 1 - fovSmoothT);
					if (fovSmoothT < 0.001f)
					{
						ThirdPersonCamera.Lens.FieldOfView = lastFov;
						lastFov = 0;
					}
				}
				
			}
		}

		public bool FirstPerson
		{
			get => _firstPerson;
			set
			{
				if (_firstPerson == value) return;
				_firstPerson = value;
				SetFirstPerson(value);
			}
		}
		
		private void SetFirstPerson(bool value)
		{
			if (value)
			{
				FirstPersonCamera.gameObject.SetActive(true);
				ThirdPersonCamera.gameObject.SetActive(false);
				if (lastFov > 0)
					FirstPersonCamera.Lens.FieldOfView = lastFov;
				else 
					FirstPersonCamera.Lens.FieldOfView = defaultFovFps;
				ThirdPersonCamera.Lens.FieldOfView = defaultFovTps;
			}
			else
			{
				FirstPersonCamera.gameObject.SetActive(false);
				ThirdPersonCamera.gameObject.SetActive(true);
				FirstPersonCamera.Lens.FieldOfView = defaultFovFps;
				if (lastFov > 0)
					ThirdPersonCamera.Lens.FieldOfView = lastFov;
				else
					ThirdPersonCamera.Lens.FieldOfView = defaultFovTps;
				
			}
		}

		public void Follow(Transform target)
		{
			FirstPersonCamera.Follow = target;
			ThirdPersonCamera.Follow = target;
		}
		
		public void SetFov(float fov, float fovTime = 0)
		{
			if (fov == lastFov) return;
			lastFov = fov;
			
			if (fovTime > 0)
			{
				fovSmoothT = 1;
				fovSmoothing = 1f / fovTime;
			}

			if (FirstPerson)
			{
				oldFov = FirstPersonCamera.Lens.FieldOfView;
				if (fovTime <= 0)
					FirstPersonCamera.Lens.FieldOfView = fov;
			}
			else
			{
				oldFov = ThirdPersonCamera.Lens.FieldOfView;
				if (fovTime <= 0)
					ThirdPersonCamera.Lens.FieldOfView = fov;
			}
		}

		public void ResetFov(float fovTime = 0) => SetFov(FirstPerson ? defaultFovFps : defaultFovTps, fovTime);
    }    
}
