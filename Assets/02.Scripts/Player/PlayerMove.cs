using UnityEngine;
using System.Collections;
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerSO _playerSO; // 플레이어 데이터 스크립터블 오브젝트
    [Header("이동 및 점프")]
    private float _moveSpeed;  // 이동 속도
    private float _jumpPower => _playerSO.JumpPower; // 점프 힘
    private float _defaultMoveSpeed => _playerSO.MoveSpeed; // 기본 이동 속도
    private float _sprintSpeed => _playerSO.SprintSpeed; // 스프린트 속도
    private bool _canDoubleJump = true; // 더블 점프 가능 여부
    private bool _wasGrounded = true; // 마지막에 땅에 닿았는지 여부
    private Vector3 _lastPosition;
    private bool _isJumping = false;
    private bool _isSprinting = false;

    [Header("스태미나")]
    public float MaxStemina => _playerSO.MaxStemina; // 최대 스태미나
    public float CurrentStemina; // 현재 스태미나
    public float SteminaDepleteRate => _playerSO.SteminaDepleteRate; // 스태미나 감소 속도
    public float SteminaRecoverRate => _playerSO.SteminaRecoverRate; // 스태미나 회복 속도
    public float SteminaRecoverDelay => _playerSO.SteminaRecoverDelay; // 스태미나 회복 지연 시간
    public float SteminaRecoverDelayTime = 0f; // 스태미나 회복 지연 시간 카운트
    private bool _isRecovering => !Input.GetKey(KeyCode.LeftShift) && !_isDashing && !_isWallClimbing;

    [Header("구르기")]
    private bool _isDashing = false; // 대쉬 상태
    private float _dashDistance => _playerSO.DashDistance; // 대쉬 거리
    private float _dashDuration => _playerSO.DashDuration; // 대쉬 지속 시간

    [Header("벽타기")]
    private CollisionFlags _lastCollision;
    private float _wallSlideSpeed => _playerSO._wallSlideSpeed; // 벽타기 슬라이드 속도
    private LayerMask _wallLayer => _playerSO._wallLayer; // 벽 레이어
    private float _wallCheckDistance => _playerSO._wallCheckDistance; // 벽 체크 거리
    private bool _isWallClimbing = false; // 벽타기 상태

    private float _yVelocity; // 점프 및 중력 적용을 위한 Y축 속도
    private const float GRAVITY = -9.81f; // 중력 값
    private CharacterController _controller; // 캐릭터 컨트롤러 컴포넌트
    [SerializeField]private Animator[] _animator;
    private MeleeService _meleeService;

    public bool CanDoAnything()
    {
        return !_meleeService.IsSwinging && !_isDashing;
    }

    public bool HideDummy => _isDashing || _isWallClimbing || _isSprinting || _isJumping;



    #region Unity Methods
    private void Awake()
    {
        _moveSpeed = _playerSO.MoveSpeed;
        _controller = GetComponent<CharacterController>();
        _lastPosition = transform.position;
        CurrentStemina = _playerSO.MaxStemina;
    }
    private void Start()
    {
        _meleeService = GetComponentInChildren<MeleeService>();
    }
    void Update()
    {

        if (GameManager.Instance?.State != GameState.Run) return;
        Vector3 worldDelta = transform.position - _lastPosition;
        Vector3 localDelta = transform.InverseTransformDirection(worldDelta);
        float velocityX = localDelta.x / Time.deltaTime;
        float velocityZ = localDelta.z / Time.deltaTime;
        _animator[(int)CameraFollow.Instance.CurrentView].SetFloat("VelocityX", velocityX);
        _animator[(int)CameraFollow.Instance.CurrentView].SetFloat("VelocityZ", velocityZ);
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0, v).normalized;

        // 카메라의 y축 회전만 반영
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDir;
        if (CanDoAnything()|| !_controller.isGrounded)
        {
            moveDir = camForward * input.z + camRight * input.x;
        }
        else
        {
            moveDir = Vector3.zero;
        }


        moveDir *= _moveSpeed;

        UpdateSprintState();
        HandleRoll();
        Jump();
        Climb();

        RecoverStemina();
        foreach(var anim in _animator)
        {
            anim.SetBool("IsGround", _controller.isGrounded);
        }
        _wasGrounded = _controller.isGrounded;
        _isJumping = !_controller.isGrounded;
        moveDir.y = _yVelocity;

        _lastPosition = transform.position;
        if (new Vector2(velocityX, velocityZ).magnitude > 0.1f)
        {
            _animator[(int)CameraFollow.Instance.CurrentView].SetBool("IsMoving", true);
        }
        else
        {
            _animator[(int)CameraFollow.Instance.CurrentView].SetBool("IsMoving", false);
        }
        if (!_isDashing)
        {
            _lastCollision = _controller.Move(moveDir * Time.deltaTime);
        }


    }
    #endregion

    #region 달리기
    private void UpdateSprintState()
    {
        if (!CanDoAnything()) return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v);
        if (Input.GetKey(KeyCode.LeftShift) && input.magnitude > 0.1f)
        {
            if (UseStemina(SteminaDepleteRate * Time.deltaTime))
            {
                _isSprinting = true;
                _moveSpeed = _sprintSpeed;
                _animator[(int)CameraFollow.Instance.CurrentView].SetBool("Sprint", true);
            }
            else
            {
                _isSprinting = false;
                _moveSpeed = _defaultMoveSpeed;
                _animator[(int)CameraFollow.Instance.CurrentView].SetBool("Sprint", false);
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || input.magnitude <= 0.1f)
        {
            _isSprinting = false;
            _animator[(int)CameraFollow.Instance.CurrentView].SetBool("Sprint", false);
            _moveSpeed = _defaultMoveSpeed;
        }
    }
    #endregion

    #region 점프
    private void Jump()
    {

        if (_controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && CanDoAnything())
            {
                _canDoubleJump = true;
                _yVelocity = _jumpPower;
                _animator[(int)CameraFollow.Instance.CurrentView].SetTrigger("Jump");
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

            if (Input.GetKeyDown(KeyCode.Space) && _canDoubleJump && CanDoAnything())
            {
                _canDoubleJump = false;
                _yVelocity = _jumpPower;

                _animator[(int)CameraFollow.Instance.CurrentView].SetTrigger("Jump");
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
        if (Input.GetKeyDown(KeyCode.E) && _controller.isGrounded && !_isDashing)
        {
            if (UseStemina(SteminaDepleteRate))
            {
                foreach(var anim in _animator)
                {
                    anim.SetTrigger("Roll");
                }
                StartCoroutine(Roll());
            }
        }
    }

    private IEnumerator Roll()
    {
        _isDashing = true;

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 direction = transform.forward; 
        Vector3 targetPos = startPos + direction * _dashDistance;

        while (elapsed < _dashDuration)
        {
            float t = elapsed / _dashDuration;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            _controller.Move(newPos - transform.position);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _controller.Move(targetPos - transform.position);
        _isDashing = false;
    }

    #endregion

    #region 벽타기

    private void Climb()
    {
        //bool touchingWall = (_lastCollision & CollisionFlags.Sides) != 0;
        bool isFalling = !_controller.isGrounded && _yVelocity < 0;
        bool holdingClimb = Input.GetKey(KeyCode.F);

        if (!CanDoAnything()) return;
        if (IsNearWall() && holdingClimb && isFalling && UseStemina(SteminaDepleteRate * Time.deltaTime))
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

    #region 스테미너 관련
    private bool UseStemina(float amount)
    {
        if (CurrentStemina > amount)
        {
            CurrentStemina -= amount;
            UI_PlayerStat.Instance.SetStamina(CurrentStemina / MaxStemina);
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
                    CurrentStemina = MaxStemina; // 스테미너가 최대치를 넘지 않도록
                }
                UI_PlayerStat.Instance.SetStamina(CurrentStemina / MaxStemina);
            }
        }
    }
    #endregion
}
