using UnityEngine;

public class RecoilService : MonoBehaviour
{
    [SerializeField] private CameraRotate _cameraRotate;
    [Header("Recoil Strength")]
    [SerializeField] private float _recoilPitch = 1f;      // x축(상하) 반동 세기
    [SerializeField] private float _recoilYawVariance = 0.1f; // y축(좌우) 반동 랜덤 범위
    [SerializeField] private float _recoilMultiplier = 1f;  // 전체 곱셈 계수

    /// <summary>
    /// 발사 시 호출
    /// </summary>
    public void AddRecoil()
    {
        float pitchImpulse = _recoilPitch * _recoilMultiplier;
        float yawImpulse = Random.Range(-_recoilYawVariance, _recoilYawVariance)
                             * _recoilMultiplier;

        _cameraRotate.ApplyRecoil(pitchImpulse, yawImpulse);
    }
}
