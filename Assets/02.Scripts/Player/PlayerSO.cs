using UnityEngine;
[CreateAssetMenu(fileName = "PlayerMovementData", menuName = "ScriptableObjects/PlayerMovementData", order = 1)]
public class PlayerSO : ScriptableObject
{
    [Header("이동 및 점프")]
    public float MoveSpeed = 3f; // 이동 속도
    public float JumpPower = 5f; // 점프 힘
    public float SprintSpeed = 8f; // 스프린트 속도

    [Header("스태미나")]
    public float MaxStemina = 100f; // 최대 스태미나
    public float SteminaDepleteRate = 5f; // 스태미나 감소 속도
    public float SteminaRecoverRate = 2f; // 스태미나 회복 속도
    public float SteminaRecoverDelay = 2f; // 스태미나 회복 지연 시간

    [Header("대쉬")]
    public float DashDistance = 5f; // 대쉬 거리
    public float DashDuration = 0.3f; // 대쉬 지속시간

    [Header("벽타기")]
    public float _wallSlideSpeed = 0.1f; // 벽타기 슬라이드 속도
    public LayerMask _wallLayer; // 벽 레이어
    public float _wallCheckDistance = 0.6f; // 벽 체크 거리
}
