using UnityEngine;

[RequireComponent(typeof(AmmoManager))]
public class GrenadeService : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float maxChargeTime = 3f;

    [Header("References")]
    [SerializeField] private Transform throwPoint;

    [Header("Dependencies")]
    [SerializeField] private AmmoManager ammoManager;
    [SerializeField] private GranadePool grenadePool;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip holdSound;
    [SerializeField] private AudioClip[] throwSounds;

    private bool isCharging;
    private float chargeTimer;

    private void Update()
    {
        if (GameManager.Instance?.State != GameState.Run) return;
        if (Input.GetMouseButtonDown(1) && ammoManager.CurrentGrenade > 0)
        {
            audioSource.PlayOneShot(holdSound);
            isCharging = true;
            chargeTimer = 0f;
        }
        else if (Input.GetMouseButtonUp(1) && isCharging)
            Throw();

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            if (chargeTimer >= maxChargeTime) Throw();
        }
    }

    private void Throw()
    {
        if (!ammoManager.UseGrenade()) return;
        isCharging = false;

        audioSource.PlayOneShot(throwSounds[Random.Range(0, throwSounds.Length)]);
        var go = grenadePool.GetGrenade();
        go.transform.position = throwPoint.position;
        var rb = go.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Camera.main.transform.forward * Mathf.Lerp(throwForce, throwForce * 2, chargeTimer / maxChargeTime), ForceMode.Impulse);
    }
}
