using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "newWeaponItem", menuName = "Item/Weapon Item")]
public class WeaponItem : Item
{
    #region Weapon Information

    [Header("Weapon Information")]
    public WeaponType weapon;
    public HolsterPosition holsterPosition;
    public bool isPrimary = false;

    #endregion

    #region Weapon Animation

    [Header("Weapon Animation")]
    public AnimatorOverrideController weaponAnimatorOverrideController;
    public float holdWeaponWeight = 1f;

    #endregion

    #region Weapon IK Settings

    [Header("IK Settings")]
    public Vector3 SpineOffset;
    public Vector3 ChestOffset;
    public Vector3 HeadOffset;

    #endregion

    #region Weapon Shoot Configuration

    [Header("Weapon Shoot Configuration")]
    public LayerMask hitMask;
    public float firerate = 0.25f;
    public int bulletsPerShot = 1;
    public float maxSpreadTime = 1f;
    public Vector3 shootSpread = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 minSpread = Vector3.zero;
    public float recoilRecoverySpeed = 1f;

    #endregion

    #region Weapon trail Configuration

    [Header("Weapon Trail Configuration")]
    public Material trailMaterial;
    public AnimationCurve trailWidthCurve;
    public float trailDuration = 0.5f;
    public float trailMinVertexDistance = 0.1f;
    public Gradient trailColor;
    public float trailMissDistance = 100;
    public float simulationSpeed = 100f;

    #endregion

    #region Weapon Damage Configuration

    [Header("Weapon Damage Configuration")]
    public MinMaxCurve damageCurve;

    #endregion

    #region Weapon Audio Configuration

    [Header("Weapon Audio Configuration")]
    public AudioClip shootAudioClip;
    public AudioClip reloadAudioClip;
    public AudioClip equipAudioClip;
    public AudioClip emptyClipAudioClip;

    #endregion

    #region Weapon Ammo Configuration

    [Header("Ammo Configuration")]
    public bool autoReload;

    public int MaxAmmo;
    public int ClipSize;

    public int CurrentAmmo;
    public int CurrentClipAmmo;

    #endregion

    #region Object Pool

    private GameObject poolHolder;

    private ObjectPool<TrailRenderer> trailPool;

    #endregion

    #region Private Variables

    private MonoBehaviour activeMonobehaviour;
    private Animator weaponAnimator;
    private ParticleSystem shootParticleSystem;
    private AudioSource weaponAudioSource;

    private float lastShootTime;

    #endregion

    #region Initialize Functions

    public void Initialize(MonoBehaviour MonoBehaviour, ParticleSystem ParticleSystem, AudioSource AudioSource, Animator WeaponAnimator, Transform bulletPoolHolder)
    {
        //Filling the references
        activeMonobehaviour = MonoBehaviour;
        shootParticleSystem = ParticleSystem;
        weaponAudioSource = AudioSource;
        weaponAnimator = WeaponAnimator;

        //Ammo Initialization
        CurrentAmmo = MaxAmmo;
        CurrentClipAmmo = ClipSize;

        lastShootTime = 0; //In Unity Editor this is not properly reset, but in Build it does reset

        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        poolHolder = bulletPoolHolder.gameObject;
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        instance.transform.SetParent(poolHolder.transform, true);
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailColor;
        trail.material = trailMaterial;
        trail.widthCurve = trailWidthCurve;
        trail.time = trailDuration;
        trail.minVertexDistance = trailMinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    #endregion

    #region Shoot Functions

    public void Shoot()
    {
        
        if (Time.time > firerate + lastShootTime)
        {
            lastShootTime = Time.time;

            CurrentClipAmmo--;

            shootParticleSystem.Play();
            PlayShootAudio(weaponAudioSource, shootAudioClip);
            weaponAnimator.SetTrigger("Shoot");

            for (int i = 0; i < bulletsPerShot; i++)
            {
                Vector3 spreadAmount = GetSpread();

                Vector3 shootDirection = shootParticleSystem.transform.forward + spreadAmount;

                shootDirection.Normalize();

                if (Physics.Raycast(shootParticleSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, hitMask))
                {
                    activeMonobehaviour.StartCoroutine(PlayTrail(shootParticleSystem.transform.position, hit.point, hit));
                }
                else
                {
                    activeMonobehaviour.StartCoroutine(PlayTrail(shootParticleSystem.transform.position, shootParticleSystem.transform.position + (shootDirection * trailMissDistance), new RaycastHit()));
                }
            }
        }
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit hit)
    {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.emitting = true;
        instance.transform.position = StartPoint;
        yield return null;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(StartPoint, EndPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        yield return new WaitForSeconds(trailDuration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);

    }

    private Vector3 GetSpread(float shootTime = 0)
    {
        Vector3 spread = Vector3.zero;
        spread = new Vector3(Random.Range(-shootSpread.x, shootSpread.x), Random.Range(-shootSpread.y, shootSpread.y), Random.Range(-shootSpread.z, shootSpread.z));
        return spread;
    }

    #endregion

    #region Audio Functions

    private void PlayShootAudio(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlayEmptyClipAudio()
    {
        weaponAudioSource.clip = emptyClipAudioClip;
        weaponAudioSource.Play();
    }

    public void PlayEquipAudio()
    {
        weaponAudioSource.clip = equipAudioClip;
        weaponAudioSource.Play();
    }

    #endregion

    #region Ammo Functions

    public void Reload()
    {
        int maxReloadAmount = Mathf.Min(ClipSize, CurrentAmmo);
        int availBulletsInCurrentClip = ClipSize - CurrentClipAmmo;
        int reloadAmount = Mathf.Min(maxReloadAmount, availBulletsInCurrentClip);

        CurrentClipAmmo = CurrentClipAmmo + reloadAmount;
        CurrentAmmo -= reloadAmount;
    }

    #endregion

}
