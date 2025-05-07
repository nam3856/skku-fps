using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType
{
    Concrete,
    Metal,
    Wood,
    Dirt,
    Sand,
    SoftBody,
    Default
}

public class HitEffectPool : MonoBehaviour
{
    [System.Serializable]
    public class MaterialEffect
    {
        public MaterialType materialType;
        public ParticleSystem effectPrefab;
    }

    [SerializeField] private List<MaterialEffect> materialEffects = new List<MaterialEffect>();
    [SerializeField] private int poolSize = 10;

    private Dictionary<MaterialType, Queue<ParticleSystem>> effectPools = new Dictionary<MaterialType, Queue<ParticleSystem>>();
    private static HitEffectPool instance;

    public static HitEffectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<HitEffectPool>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("HitEffectPool");
                    instance = obj.AddComponent<HitEffectPool>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var materialEffect in materialEffects)
        {
            if (materialEffect.effectPrefab == null) continue;

            Queue<ParticleSystem> pool = new Queue<ParticleSystem>();
            for (int i = 0; i < poolSize; i++)
            {
                ParticleSystem effect = Instantiate(materialEffect.effectPrefab, transform);
                effect.gameObject.SetActive(false);
                pool.Enqueue(effect);
            }
            effectPools[materialEffect.materialType] = pool;
        }
    }

    public ParticleSystem GetHitEffect(MaterialType materialType)
    {
        if (!effectPools.ContainsKey(materialType))
        {
            Debug.LogWarning($"No effect pool found for material type: {materialType}");
            return null;
        }

        Queue<ParticleSystem> pool = effectPools[materialType];
        if (pool.Count > 0)
        {
            ParticleSystem effect = pool.Dequeue();
            effect.gameObject.SetActive(true);
            return effect;
        }
        else
        {
            // 풀이 비어있으면 새로 생성
            var materialEffect = materialEffects.Find(x => x.materialType == materialType);
            if (materialEffect != null && materialEffect.effectPrefab != null)
            {
                ParticleSystem newEffect = Instantiate(materialEffect.effectPrefab, transform);
                newEffect.gameObject.SetActive(true);
                return newEffect;
            }
        }
        return null;
    }

    public void ReturnHitEffect(ParticleSystem effect, MaterialType materialType)
    {
        if (!effectPools.ContainsKey(materialType))
        {
            Destroy(effect.gameObject);
            return;
        }

        effect.transform.SetParent(transform);
        effect.gameObject.SetActive(false);
        effectPools[materialType].Enqueue(effect);
    }

    public IEnumerator ReturnHitEffectAfterPlay(ParticleSystem effect, MaterialType materialType)
    {
        yield return new WaitForSeconds(effect.main.duration);
        ReturnHitEffect(effect, materialType);
    }
    public void ReturnAllChildrenUnder(Transform parent)
    {
        CFX_HitEffectMarker[] markers = parent.GetComponentsInChildren<CFX_HitEffectMarker>(true);
        foreach (var marker in markers)
        {
            var ps = marker.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ReturnHitEffect(ps, marker.materialType);
            }
        }
    }
    public MaterialType GetMaterialTypeFromTag(string tag)
    {
        switch (tag.ToLower())
        {
            case "concrete":
                return MaterialType.Concrete;
            case "metal":
                return MaterialType.Metal;
            case "wood":
                return MaterialType.Wood;
            case "dirt":
                return MaterialType.Dirt;
            case "sand":
                return MaterialType.Sand;
            case "softbody":
                return MaterialType.SoftBody;
            default:
                return MaterialType.Default;
        }
    }
} 