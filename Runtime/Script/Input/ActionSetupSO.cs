using UnityEngine;

namespace David6.ShooterFramework
{
    public enum eInputMode
    {
        Press,      // 버튼을 누르는 순간(Performed) 한 번만 호출
        LongPress,  // 지정된 시간(n초) 이상 눌렀을 때 한 번 호출
        Hold,       // 버튼을 누르고 있는 동안 매 프레임(또는 매 입력 이벤트)마다 호출
        Toggle,     // 버튼을 누를 때마다 상태를 토글하고 그때 한 번 호출
        BufferPress,// 버튼을 눌러도 즉시 호출하지 않고, 지정된 짧은 시간 동안 보류했다가 꺼내 호출
    }

    [CreateAssetMenu(fileName = "ActionSetupSO", menuName = "Scriptable Objects/ActionSetupSO")]
    public class ActionSetupSO : ScriptableObject
    {
        public string ActionName;
        public eInputMode InputMode;
        [Tooltip("LongPress 임계 시간")]
        public float LongPressDuration = 0.2f;
        [Tooltip("BufferPress 유효 시간")]
        public float BufferTime = 0.2f;
    }
}
