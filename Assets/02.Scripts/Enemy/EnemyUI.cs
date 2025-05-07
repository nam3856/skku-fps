using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EnemyUI : MonoBehaviour
{
    public Image HPFill;
    public TextMeshProUGUI StateText;
    private Transform _camera;
    private void OnEnable()
    {
        _camera = Camera.main.transform;
    }
    private void LateUpdate()
    {
        transform.forward = _camera.forward;
    }

    public void SetHealthFill(float amount)
    {
        HPFill.fillAmount = amount;
    }

    public void SetStateText(string state)
    {
        StateText.text = state;
    }
}
