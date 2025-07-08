

using UnityEngine;
using UnityEngine.InputSystem;


namespace David6.shootercore.Input
{
    /// <summary>
    /// Represents a profile for input settings.
    /// </summary>
    [CreateAssetMenu(fileName = "InputSettingProfile", menuName = "Configs/Input/InputSettingProfile")]
    public class DInputSettingProfile : ScriptableObject
    {
        public InputActionAsset InputActions; // Input actions asset reference

    }
}