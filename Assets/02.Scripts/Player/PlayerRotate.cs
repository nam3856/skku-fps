using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 5f;
    private float _rotationX = 0;

    private void Update()
    {
        if (CameraFollow.Instance.CurrentView == CameraMode.Quater)
        {
            HandleQuarterViewRotation();
        }
        //else
        //{
        //    HandleNormalRotation();
        //}
    }

    //private void HandleNormalRotation()
    //{

    //    float mouseX = Input.GetAxis("Mouse X");
    //    _rotationX += mouseX * RotationSpeed;
    //    transform.rotation = Quaternion.Euler(0f, -_rotationX, 0f);
    //}

    private void HandleQuarterViewRotation()
    {
        // 마우스 커서 잠금 해제
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 마우스 위치로 캐릭터 회전
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y; // Y축은 고정
            
            // 캐릭터가 타겟을 바라보도록 회전
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }
        }
    }
}
