using System.Collections;
using UnityEngine;

public enum GameState
{
    Ready,
    Run,
    Over
}
public class GameManager : MonoBehaviour
{
    public GameState State;
    public static GameManager Instance;
    public float WaitTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        State = GameState.Ready;
        Time.timeScale = 0f;

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        yield return new WaitForSecondsRealtime(WaitTime);
        State = GameState.Run;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        State = GameState.Over;
        Time.timeScale = 0f;
        SystemUI.Instance.ShowGameOver();
    }

}
