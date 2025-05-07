using System;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(AmmoManager))]
public class FireService : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem[] muzzleEffects;
    [SerializeField] private GameObject tracerPrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shotSound;

    [Header("Dependencies")]
    [SerializeField] private AmmoManager ammoManager;
    [SerializeField] private RecoilService recoilService;


    private PlayerCombatController _playerCombatController;
    private PlayerCombatDataSO _combatDataSO;
    private Animator[] _animators;

    private void Start()
    {
        _playerCombatController = GetComponent<PlayerCombatController>();
        _combatDataSO = _playerCombatController.combatDataSO;
        _animators = _playerCombatController.animators;
    }

    public void SingleFire()
    {
        if (!ammoManager.UseAmmo()) return;
        PlayShotEffects();
        recoilService.AddRecoil();
        PerformRaycast();
    }

    private void PlayShotEffects()
    {
        audioSource.PlayOneShot(shotSound);
        int idx = (int)CameraFollow.Instance.CurrentView;

        foreach(var anim in _animators)
        {
            anim.SetTrigger("SHOOT");
        }
        muzzleEffects[idx].Play();
    }

    private void PerformRaycast()
    {
        Ray ray;
        switch (CameraFollow.Instance.CurrentView)
        {
            case CameraMode.FPS:
                ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                break;
            case CameraMode.TPS:
                var fp = firePoints[(int)CameraMode.TPS];
                ray = new Ray(fp.position, fp.forward);
                break;
            case CameraMode.Quater:
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                break;
            default:
                ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                break;
        }

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out var hit, raycastDistance, raycastMask))
        {
            MaterialType materialType = HitEffectPool.Instance.GetMaterialTypeFromTag(hit.collider.tag);

            ParticleSystem hitEffect = HitEffectPool.Instance.GetHitEffect(materialType);
            if (hitEffect != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle")){

                    hitEffect.transform.SetParent(hit.collider.transform);
                }
                hitEffect.transform.position = hit.point;
                hitEffect.transform.forward = hit.normal;
                hitEffect.Play();

                hitEffect.gameObject.GetComponent<CFX_HitEffectMarker>().materialType = materialType;

                StartCoroutine(HitEffectPool.Instance.ReturnHitEffectAfterPlay(hitEffect, materialType));
            }
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                var dmg = new Damage
                {
                    amount = 1,
                    type = DamageType.Normal,
                    origin = ray.origin,
                    knockbackForce = _combatDataSO.KnockbackByType[(int)DamageType.Normal]
                };
                target.TakeDamage(dmg);
            }

            Debug.Log($"Hit object: {hit.collider.gameObject.name} with material: {materialType}");
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * raycastDistance;
        }
        SpawnTracer(ray, targetPoint);
    }

    private void SpawnTracer(Ray ray, Vector3 target)
    {
        var tracer = Instantiate(tracerPrefab);
        var lr = tracer.GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.SetPositions(new[] { firePoints[(int)CameraFollow.Instance.CurrentView].position, target });
        Destroy(tracer, 0.1f);
    }
}
