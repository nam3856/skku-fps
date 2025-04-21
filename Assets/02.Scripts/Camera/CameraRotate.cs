using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float rotationSpeed = 5f;
    private float _rotationY = 0f;
    private float _rotationX = 0f;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _rotationX = angles.y;
        _rotationY = angles.x;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _rotationX += mouseX * rotationSpeed;
        _rotationY -= mouseY * rotationSpeed;
        _rotationY = Mathf.Clamp(_rotationY, -80f, 80f);

        transform.rotation = Quaternion.Euler(_rotationY, _rotationX, 0f);
    }
}
