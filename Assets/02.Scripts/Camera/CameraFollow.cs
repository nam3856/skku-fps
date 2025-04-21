using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset; 

    private void LateUpdate()
    {
        if (Target != null)
        {
            // 회전을 기준으로 오프셋 방향 계산
            transform.position = Target.position + Target.rotation * Offset;
        }
    }
}