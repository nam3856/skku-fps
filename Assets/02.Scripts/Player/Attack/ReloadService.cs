using UnityEngine;

public class ReloadService : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float reloadDuration = 2f;
    [SerializeField] private PlayerCombatDataSO combatData;

    [Header("Dependencies")]
    [SerializeField] private AmmoManager ammoManager;
    [SerializeField] private Animator[] animators;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip reloadSound;

    private bool isReloading;
    private float timer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && ammoManager.CurrentAmmo < combatData.MaxAmmo && !isReloading)
            StartReload();
        if (isReloading)
        {
            timer += Time.deltaTime;
            if(Input.GetKeyDown(KeyCode.Mouse0) && ammoManager.CurrentAmmo > 0)
            {
                isReloading = false;
                StartCoroutine(UI_PlayerStat.Instance.CancelReload());
            }
            if (timer >= reloadDuration)
            {
                ammoManager.Reload();
                isReloading = false;
            }
        }
    }

    private void StartReload()
    {
        isReloading = true; timer = 0;
        int idx = (int)CameraFollow.Instance.CurrentView;
        animators[idx].SetTrigger("RELOAD");
        audioSource.PlayOneShot(reloadSound);
        StartCoroutine(UI_PlayerStat.Instance.StartReload(reloadDuration));
    }
}
