using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 5f;

    private float _rotationX = 0;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        _rotationX += mouseX * RotationSpeed;
        transform.rotation = Quaternion.Euler(0f, _rotationX, 0f);
    }
}
