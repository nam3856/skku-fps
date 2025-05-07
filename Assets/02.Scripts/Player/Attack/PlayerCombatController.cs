// PlayerCombatController.cs
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCombatController : MonoBehaviour
{
    [Header("서비스 참조")]
    [SerializeField] private FireInput fireInput;
    [SerializeField] private ReloadService reloadService;
    [SerializeField] private MeleeService meleeService;

    [Header("모델 오브젝트")]
    [SerializeField] private GameObject[] gunModels;
    [SerializeField] private GameObject[] swordModels;
    [SerializeField] private GameObject[] playerModels;

    [Header("애니메이터")]
    public Animator[] animators;

    public PlayerCombatDataSO combatDataSO;
    private enum WeaponType { Gun, Sword }
    private WeaponType currentWeapon;
    private CameraMode _lastView;
    private bool _canAction => _playerMove.CanDoAnything();
    private PlayerMove _playerMove;
    private Vector3[] _originalRotation;
    private Vector3[] _originalPosition;
    private void Start()
    {
        _playerMove = GetComponentInParent<PlayerMove>();
        _originalRotation = new Vector3[2];
        _originalPosition = new Vector3[2];

        for (int i = 0; i < 2; i++)
        {
            _originalPosition[i] = playerModels[i].transform.localPosition;
            _originalRotation[i] = playerModels[i].transform.localEulerAngles;
        }

        EquipGun();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipGun();
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipSword();

        if (currentWeapon == WeaponType.Sword && Input.GetMouseButtonDown(0) && _canAction)
        {
            meleeService.Attack();
        }

        var curView = CameraFollow.Instance.CurrentView;
        if (curView != _lastView)
        {
            if (currentWeapon == WeaponType.Gun) EquipGun();
            else EquipSword();
            _lastView = curView;
        }
    }

    private void EquipGun()
    {
        currentWeapon = WeaponType.Gun;
        var activeModel = gunModels[(int)CameraFollow.Instance.CurrentView];
        for (int i = 0; i < 2; i++)
        {
            playerModels[i].transform.localPosition = _originalPosition[i];
            playerModels[i].transform.localEulerAngles = _originalRotation[i];
        }

        foreach (var go in gunModels)
        {
            if (go != null)
                go.SetActive(go == activeModel);
        }

        foreach (var go in swordModels)
        {
            if (go != null)
                go.SetActive(false);
        }
        UI_PlayerStat.Instance.ChangeWeapon((int)currentWeapon);
        foreach (var animator in animators)
        {
            animator.SetLayerWeight(1, 1);
            animator.SetLayerWeight(2, 1);
            animator.SetLayerWeight(3, 0);


        }
        fireInput.enabled = true;
        reloadService.enabled = true;
        meleeService.enabled = false;
    }

    private void EquipSword()
    {
        currentWeapon = WeaponType.Sword;
        var activeModel = swordModels[(int)CameraFollow.Instance.CurrentView];

        playerModels[0].transform.localPosition = new Vector3(-0.04f, -1.4f, 0);
        playerModels[0].transform.localEulerAngles = new Vector3(0, 0.13f, 0);
        playerModels[1].transform.localEulerAngles = Vector3.zero;
        foreach (var go in swordModels)
        {
            if (go != null)
                go.SetActive(go == activeModel);
        }
        foreach (var go in gunModels)
        {
            if (go != null)
                go.SetActive(false);
        }
        UI_PlayerStat.Instance.ChangeWeapon((int)currentWeapon);
        foreach (var animator in animators)
        {

            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 0); 
            animator.SetLayerWeight(3, 1);
            animator.Play("Intro");
        }
        fireInput.enabled = false;
        reloadService.enabled = false;
        meleeService.enabled = true;
    }
}
