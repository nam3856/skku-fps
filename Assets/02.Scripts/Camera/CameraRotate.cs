using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour
{
    public float RotationSpeed = 5f;

    #region Recoil Settings
    private Vector2 _currentRecoil;
    private Vector2 _recoilVelocity;
    [SerializeField] private float recoilRecoverySpeed = 10f; // 클수록 빠르게 회복
    #endregion

    private float _rotationX, _rotationY;

    private void Start()
    {
        var angles = transform.eulerAngles;
        _rotationX = angles.y;
        _rotationY = angles.x;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance?.State != GameState.Run) return;
        if (CameraFollow.Instance.CurrentView == CameraMode.Quater)
        {
            transform.rotation = Quaternion.Euler(60, -45, 2f);
            return;
        }
        // 마우스 입력
        _rotationX += Input.GetAxis("Mouse X") * RotationSpeed;
        _rotationY -= Input.GetAxis("Mouse Y") * RotationSpeed;
        _rotationY = Mathf.Clamp(_rotationY, -80f, 80f);

        // 반동 복구 (0 으로 스무딩)
        _currentRecoil = Vector2.SmoothDamp(
            _currentRecoil,
            Vector2.zero,
            ref _recoilVelocity,
            1f / recoilRecoverySpeed
        );

        // 최종 회전 = 입력 회전 + 반동 오프셋
        float finalPitch = _rotationY + _currentRecoil.x;
        float finalYaw = _rotationX + _currentRecoil.y;
        transform.rotation = Quaternion.Euler(finalPitch, finalYaw, 0f);


        Transform player = CameraFollow.Instance.transform;  // 카메라의 부모가 아니라 플레이어 찾아야 함
        if (CameraFollow.Instance != null && CameraFollow.Instance.Targets != null)
        {
            player = CameraFollow.Instance.Targets[0];
        }
        Vector3 playerRotation = new Vector3(0, finalYaw, 0);
        player.rotation = Quaternion.Euler(playerRotation);
    }

    /// <summary>
    /// WeaponRecoil 에서 호출.
    /// recoilX: pitch 방향(상하) 반동 크기
    /// recoilY: yaw 방향(좌우) 랜덤 범위
    /// </summary>
    public void ApplyRecoil(float recoilX, float recoilY)
    {
        // 즉시 반영 또는 살짝 지연을 원하면 코루틴으로 변경하세요
        _currentRecoil += new Vector2(recoilX, recoilY);
    }
}
