using UnityEngine;

public enum CameraMode
{
    FPS,
    TPS,
    Quater
}

public class CameraFollow : MonoBehaviour
{
    #region Singleton
    public static CameraFollow Instance { get; private set; }
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Transform[] _targets;
    public Transform[] Targets => _targets;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _gunPrefab;
    [SerializeField] private GameObject[] _playerPrefabs;
    #endregion

    #region Camera Settings
    [Header("Camera Settings")]
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private float _maxTpsY = 4f;
    [SerializeField] private float _minTpsY = 0.2f;
    #endregion

    #region Offset Settings
    [Header("Offset Settings")]
    [SerializeField] private Vector3[] _normalOffsets;
    [SerializeField] private Vector3[] _sprintOffsets;
    #endregion

    #region Private Fields
    private Vector3 _currentOffset;
    private float _tpsVerticalOffset;
    private CameraMode _currentView = CameraMode.FPS;
    #endregion



    #region Properties
    public CameraMode CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            _currentOffset = _normalOffsets[(int)value];
            UpdateVisibility();
        }
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _currentOffset = _normalOffsets[(int)CameraMode.FPS];
        _animator = _targets[0].GetComponentInChildren<Animator>();
        UpdateVisibility();
    }

    private void Update()
    {
        if (GameManager.Instance?.State != GameState.Run) return;
        HandleCameraModeChange();

    }

    private void LateUpdate()
    {
        if (GameManager.Instance?.State != GameState.Run) return;
        UpdateCameraPosition();
    }
    #endregion

    #region Private Methods
    private void HandleCameraModeChange()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            CurrentView = (CameraMode)(((int)CurrentView + 1) % 3);
        }
    }

    private void UpdateCameraPosition()
    {
        bool isSprinting = _animator != null && _animator.GetBool("Sprint");
        Vector3 desiredOffset = isSprinting ? _sprintOffsets[(int)CurrentView] : _normalOffsets[(int)CurrentView];
        float smoothTime = Time.deltaTime * _smoothSpeed;

        if (CurrentView == CameraMode.TPS)
        {
            UpdateTpsCamera(desiredOffset, smoothTime);
        }
        else if(CurrentView == CameraMode.FPS)
        {
            Cursor.lockState = CursorLockMode.Locked;
            UpdateFPSCamera(desiredOffset, smoothTime);
        }
        else
        {

            Cursor.lockState = CursorLockMode.Confined;
            UpdateQuaterCamera(desiredOffset, smoothTime);
        }
    }

    private void UpdateTpsCamera(Vector3 desiredOffset, float smoothTime)
    {
        _currentOffset.x = Mathf.Lerp(_currentOffset.x, desiredOffset.x, smoothTime);
        _currentOffset.z = Mathf.Lerp(_currentOffset.z, desiredOffset.z, smoothTime);
        _currentOffset.y = Mathf.Lerp(_currentOffset.y, desiredOffset.y, smoothTime);

        Vector3 newPosition = _targets[0].position + _targets[0].rotation * _currentOffset;
        transform.position = newPosition;
    }

    private void UpdateFPSCamera(Vector3 desiredOffset, float smoothTime)
    {
        _currentOffset = Vector3.Lerp(_currentOffset, desiredOffset, smoothTime);
        transform.position = _targets[0].position + _targets[0].rotation * _currentOffset;
    }

    private void UpdateQuaterCamera(Vector3 desiredOffset, float smoothTime)
    {
        _currentOffset = Vector3.Lerp(_currentOffset, desiredOffset, smoothTime);
        transform.position = _targets[0].position + _currentOffset;
    }

    private void UpdateVisibility()
    {
        if (_gunPrefab != null)
        {
            _gunPrefab.SetActive(_currentView == CameraMode.FPS);
            foreach(var player in _playerPrefabs)
            {
                player.SetActive(_currentView != CameraMode.FPS);
            }
        }
    }
    #endregion

    #region Public Methods

    
    #endregion
}