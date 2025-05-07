using UnityEngine;
using UnityEngine.UI;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Button _enlarge;
    [SerializeField] private Button _reduct;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxSize;
    [SerializeField] private float _minSize;
    [SerializeField] private float _yOffset = 10f;
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _enlarge.onClick.AddListener(Enlarge);
        _reduct.onClick.AddListener(Reduct);
    }
    private void LateUpdate()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Run) return;
        Vector3 pos = _player.position;
        pos.y = pos.y + _yOffset;
        transform.position = pos;
        if (Input.GetKeyDown(KeyCode.PageUp)) Enlarge();
        if (Input.GetKeyDown(KeyCode.PageDown)) Reduct();

        Vector3 newEulerAngles = _player.eulerAngles;
        newEulerAngles.x = 90;
        newEulerAngles.z = 0;
        transform.eulerAngles = newEulerAngles;
    }

    private void Enlarge()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Run) return;
        _camera.orthographicSize = Mathf.Max(_minSize, _camera.orthographicSize - 1f);
    }
    private void Reduct()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Run) return;
        _camera.orthographicSize = Mathf.Min(_maxSize, _camera.orthographicSize + 1f);
    }
}
