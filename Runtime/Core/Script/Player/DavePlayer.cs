using UnityEngine;

namespace David6.ShooterFramework
{
    public enum CameraMode { FirstPerson, ThirdPerson }
    public enum BodyPart { Head, Body, Arm }

    public partial class DavePlayer : MonoBehaviour
    {
        #region Serialize field

        [Header("제어할 오브젝트 및 트렌스폼")]
        //[Tooltip("플레이어가 제어할 캐릭터 컨트롤러")]
        //[SerializeField] private PlayerController Character;
        //[Tooltip("플레이어가 제어하는 카메라 스크립트")]
        //[SerializeField] private DavidCharacterCamera OrbitCamera;

        #endregion

        #region Fields

        private CameraMode _currentCameraMode = CameraMode.FirstPerson;

        #endregion


        #region Mono 기본 함수

        private void Start()
        {
            //UpdateCursorState();
            UpdateCameraMode();
        }

        private void Update()
        {
            InputConditionUpdate();
        }

        #endregion

        /// <summary>
        /// 게임 포커스 제어.
        /// </summary>
        private void UpdateCursorState()
        {
            Cursor.visible = !_cursorLocked;
            Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        /// <summary>
        /// 현재 카메라 모드에 맞춰 시각적인 부분 세팅 (Controller에서 진행)
        /// </summary>
        private void UpdateCameraMode()
        {
            // if (FPSArm == null)
            // {
            //     Log.AttentionPlease("FPS Arm 없음.");
            // }
            switch (_currentCameraMode)
            {
                case CameraMode.FirstPerson:
                //OrbitCamera.TargetDistance = 0;
                //OrbitCamera.FollowPointFraming = Vector2.zero;
                //MakeTransparent(BodyPart.Head, BodyPart.Arm);
                //FPSArm.SetActive(true);
                break;

                case CameraMode.ThirdPerson:
                //OrbitCamera.TargetDistance = CameraTargetDistance;
                //OrbitCamera.FollowPointFraming = this.FollowPointFraming;
                //RestoreAll();
                //FPSArm.SetActive(false);
                break;
            }
        }


        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            Vector3 lookInputVector = new Vector3(_axisLook.x, _axisLook.y, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }
        }
    }
}
