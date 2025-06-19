using UnityEngine;


namespace Dave6.ShooterFramework.Provider
{
    public interface ICharacterControllerProvider
    {
        public CharacterController GetController();
    }
}