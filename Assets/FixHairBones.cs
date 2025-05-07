using UnityEngine;

public class FixHairBones : MonoBehaviour
{
    public SkinnedMeshRenderer hairRenderer; // 머리카락 스킨드메시
    public Transform boneRoot; // .head 본 (또는 spine_03 이런)

    private void Awake()
    {
        if (hairRenderer != null && boneRoot != null)
        {
            hairRenderer.rootBone = boneRoot;
            hairRenderer.bones = boneRoot.GetComponentsInChildren<Transform>();
        }
    }
}