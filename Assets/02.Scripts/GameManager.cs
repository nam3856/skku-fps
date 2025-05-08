using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Run,
    Over,
    Pause
}
public class GameManager : MonoBehaviour
{
    private GameState _currentState;
    public GameState State
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
        }
    }

    public event Action<GameState> StateChanged;
    public static GameManager Instance;
    public float WaitTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _currentState = GameState.Ready;
        Time.timeScale = 0f;

        StartCoroutine(Play());
    }
    private IEnumerator Play()
    {
        yield return new WaitForSecondsRealtime(WaitTime);
        _currentState = GameState.Run;
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        PopupManager.Instance.TryOpen(EPopupType.Popup_Option, closeCallback: Resume);
        _currentState = GameState.Pause;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        switch (CameraFollow.Instance.CurrentView)
        {
            case CameraMode.FPS: Cursor.lockState = CursorLockMode.Locked; break;
            case CameraMode.TPS: Cursor.lockState = CursorLockMode.Locked; break;
            case CameraMode.Quater: Cursor.lockState = CursorLockMode.Confined; break;
        }
        _currentState = GameState.Run;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        _currentState = GameState.Over;
        Time.timeScale = 0f;
        UI_System.Instance.ShowGameOver();
    }

    public void Retry()
    {
        _currentState = GameState.Run;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

}
