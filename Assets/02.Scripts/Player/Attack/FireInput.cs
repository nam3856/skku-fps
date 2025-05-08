using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FireService))]
public class FireInput : MonoBehaviour
{
    [SerializeField] private bool useBurst = true;
    [SerializeField] private float burstInterval = 0.1f;

    [Header("Dependencies")]
    [SerializeField] private FireService fireService;

    private bool isBursting;
    private float lastBurstTime;

    private void Update()
    {
        if (GameManager.Instance?.State != GameState.Run) return;
        if (useBurst) HandleBurst();
        else if (Input.GetMouseButtonDown(0) && !isBursting)
            fireService.SingleFire();
    }

    private void HandleBurst()
    {
        if (Input.GetMouseButton(0) && !isBursting && Time.time >= lastBurstTime + burstInterval)
            StartCoroutine(BurstRoutine());
    }

    private IEnumerator BurstRoutine()
    {
        isBursting = true;
        lastBurstTime = Time.time;
        while (Input.GetMouseButton(0))
        {
            fireService.SingleFire();
            yield return new WaitForSeconds(burstInterval);
        }
        isBursting = false;
    }
}
