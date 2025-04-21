using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset = new Vector3(0, 1.5f, -2.2f); // 캐릭터 뒤에서 약간 위

    private void LateUpdate()
    {
        if (Target != null)
        {
            // 회전을 기준으로 오프셋 방향 계산
            transform.position = Target.position + Target.rotation * Offset;
        }
    }
}