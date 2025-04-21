using UnityEngine;
using System.Collections;
public class PlayerMove : MonoBehaviour
{
    [Header("이동 및 점프")]
    public float MoveSpeed = 7f; // 이동 속도
    public float JumpPower = 10f; // 점프 힘
    private float _defaultMoveSpeed; // 기본 이동 속도
    private float _sprintSpeed = 12f; // 스프린트 속도
    public bool _canDoubleJump = true; // 더블 점프 가능 여부
    private bool _wasGrounded = true; // 마지막에 땅에 닿았는지 여부

    [Header("스태미나")]
    public float MaxStemina = 100f; // 최대 스태미나
    public float CurrentStemina = 100f; // 현재 스태미나
    public float SteminaDepleteRate = 5f; // 스태미나 감소 속도
    public float SteminaRecoverRate = 2f; // 스태미나 회복 속도
    public float SteminaRecoverDelay = 2f; // 스태미나 회복 지연 시간
    public float SteminaRecoverDelayTime = 0f; // 스태미나 회복 지연 시간 카운트
    private bool _isRecovering => !Input.GetKey(KeyCode.LeftShift) && !_isRolling && !_isWallClimbing;

    [Header("롤링")]
    private bool _isRolling = false; // 구르기 상태
    [SerializeField] private float _rollDistance = 5f; // 구르기 거리
    [SerializeField] private float _rollDuration = 0.3f; // 구르기 지속시간

    [Header("벽타기")]
    [SerializeField] private float _wallSlideSpeed = 0.1f; // 벽타기 슬라이드 속도
    [SerializeField] private LayerMask _wallLayer; // 벽 레이어
    [SerializeField] private float _wallCheckDistance = 0.6f; // 벽 체크 거리
    private bool _isWallClimbing = false; // 벽타기 상태

    private float _yVelocity; // 점프 및 중력 적용을 위한 Y축 속도
    private const float GRAVITY = -9.81f; // 중력 값
    private CharacterController _controller; // 캐릭터 컨트롤러 컴포넌트

    #region Unity Methods
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _defaultMoveSpeed = MoveSpeed;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0, v).normalized;
        Vector3 move = Camera.main.transform.TransformDirection(input);
        move.y = 0f;
        move *= MoveSpeed;

        Sprint();
        HandleRoll();
        Jump();
        Climb();

        RecoverStemina();
        _wasGrounded = _controller.isGrounded;
        move.y = _yVelocity;

        if (!_isRolling)
            _controller.Move(move * Time.deltaTime);
    }
    #endregion

    #region 달리기
    private void Sprint()
    {

        if (_isRolling) return;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (UseStemina(SteminaDepleteRate * Time.deltaTime))
                MoveSpeed = _sprintSpeed;
            else
                MoveSpeed = _defaultMoveSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
            MoveSpeed = _defaultMoveSpeed;
    }
    #endregion

    #region 점프
    private void Jump()
    {

        if (_isRolling) return;
        if (_controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _canDoubleJump = true;
                _yVelocity = JumpPower;
            }
            else
            {
                _yVelocity = -1f;
            }
        }
        else
        {
            if (_wasGrounded)
            {
                _canDoubleJump = true;
            }

            if (Input.GetKeyDown(KeyCode.Space) && _canDoubleJump)
            {
                _canDoubleJump = false;
                _yVelocity = JumpPower;
            }
            else
            {
                _yVelocity += GRAVITY * Time.deltaTime;
            }
        }
    }

    #endregion

    #region 구르기
    private void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.E) && _controller.isGrounded && !_isRolling)
        {
            if (UseStemina(SteminaDepleteRate))
            {
                StartCoroutine(Roll());
            }
        }
    }

    private IEnumerator Roll()
    {
        _isRolling = true;

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 direction = transform.forward; 
        Vector3 targetPos = startPos + direction * _rollDistance;

        while (elapsed < _rollDuration)
        {
            float t = elapsed / _rollDuration;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            _controller.Move(newPos - transform.position);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _controller.Move(targetPos - transform.position);
        _isRolling = false;
    }

    #endregion

    #region 벽타기

    private void Climb()
    {

        if (_isRolling) return;
        if (IsNearWall() && Input.GetKey(KeyCode.F) && !_controller.isGrounded && UseStemina(SteminaDepleteRate * Time.deltaTime))
        {
            StartWallClimb();
        }
        else
        {
            StopWallClimb();
        }

        if (_isWallClimbing)
        {
            _yVelocity = -_wallSlideSpeed;
        }
        else
        {
            StopWallClimb();
            _yVelocity += GRAVITY * Time.deltaTime;
        }

    }
    private bool IsNearWall()
    {
        Vector3[] directions = { transform.forward, -transform.forward, transform.right, -transform.right };
        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, _wallCheckDistance, _wallLayer))
                return true;
        }
        return false;
    }
    private void StartWallClimb()
    {
        if (!_isWallClimbing)
        {
            _isWallClimbing = true;
            _yVelocity = 0f;
        }
    }
    private void StopWallClimb()
    {
        if (_isWallClimbing)
        {
            _isWallClimbing = false;
        }
    }
    #endregion

    #region 스태미나 관련
    private bool UseStemina(float amount)
    {
        if (CurrentStemina > amount)
        {
            CurrentStemina -= amount;
            UI_Game.Instance.SetStamina(CurrentStemina / MaxStemina);
            SteminaRecoverDelayTime = 0;
            return true;
        }
        return false;
    }

    private void RecoverStemina()
    {
        if (CurrentStemina < MaxStemina && _isRecovering)
        {
            SteminaRecoverDelayTime += Time.deltaTime;
            if (SteminaRecoverDelayTime >= SteminaRecoverDelay)
            {
                CurrentStemina += SteminaRecoverRate * Time.deltaTime;
                if (CurrentStemina > MaxStemina)
                {
                    CurrentStemina = MaxStemina; // 스태미나가 최대치를 넘지 않도록
                }
                UI_Game.Instance.SetStamina(CurrentStemina / MaxStemina);
            }
        }
    }
    #endregion
}
