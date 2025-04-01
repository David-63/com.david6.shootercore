using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace David6.ShooterFramework
{
    [InitializeOnLoad]
    public class AddressableAutoSetup
    {
        static AddressableAutoSetup()
        {
            // 에디터가 완전히 로딩된 후에 실행
            EditorApplication.delayCall += SetupAddressableGroups;
        }

        static void SetupAddressableGroups()
        {
            // 현재 프로젝트의 AddressableAssetSettings를 가져옴 (없으면 생성)
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings를 찾을 수 없습니다.");
                return;
            }

            // 예시 그룹 이름 (패키지에서 관리하고 싶은 그룹 이름)
            string groupName = "ShooterCoreGroup";
            var group = settings.FindGroup(groupName);
            if (group == null)
            {
                // BundledAssetGroupSchema 등 필요한 스키마를 추가해 그룹 생성
                group = settings.CreateGroup(groupName, false, false, false, null, 
                    typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
                Debug.Log($"Addressable 그룹 '{groupName}'이 생성되었습니다.");
            }
            else
            {
                Debug.Log($"Addressable 그룹 '{groupName}'이 이미 존재합니다.");
            }

            // 패키지 내 PlayerManager 에셋의 경로를 지정.
            string assetPath = "Packages/com.david6.shootercore/Runtime/Example/GameData/PlayerManager.asset";
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError("에셋의 GUID를 찾지 못했습니다. 경로를 확인해 주세요: " + assetPath);
                return;
            }



            // 에셋을 해당 그룹에 등록하거나 이동
            var entry = settings.CreateOrMoveEntry(guid, group);
            if (entry != null)
            {
                // 여기서 원하는 Addressable 이름을 지정 (예: "PlayerManager")
                entry.address = "PlayerManager";
                Debug.Log("PlayerManager 에셋이 그룹에 등록되었습니다.");
            }
            EditorUtility.SetDirty(settings);
        }
    }
}