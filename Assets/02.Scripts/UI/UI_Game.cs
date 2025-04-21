using UnityEngine;

public class UI_Game : MonoBehaviour
{
    public static UI_Game Instance;
    [SerializeField] private RectTransform fillRect; // Fill 오브젝트
    [SerializeField] private float maxWidth = 600f;  // 최대 스테미너일 때의 Fill Width

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
    public void SetStamina(float staminaRatio) // 0 ~ 1 사이 값
    {
        var size = fillRect.sizeDelta;
        size.x = maxWidth * Mathf.Clamp01(staminaRatio);
        fillRect.sizeDelta = Vector2.Lerp(fillRect.sizeDelta, new Vector2(maxWidth * staminaRatio, fillRect.sizeDelta.y), Time.deltaTime * 10f);
    }

}
