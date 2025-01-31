using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "newSubWeaponItem", menuName = "Item/Sub Weapon Item")]
public class SubWeaponItem : Item
{
    #region Weapon Information

    [Header("Weapon Information")]
    public SubWeaponType subWeaponType;
    public HolsterPosition holsterPosition;

    #endregion

    #region Weapon Throw

    [Header("Weapon Throw")]
    public float throwForce;
    public float maxExpodeTimer;

    #endregion

    #region Weapon Animation

    [Header("Weapon Animation")]
    public AnimatorOverrideController subWeaponOverrideController;

    #endregion

    #region Weapon Damage

    [Header("Weapon Damage Config")]
    public float maxDamageArea;
    public LayerMask hitMask;

    #endregion

    #region Private Variables

    private MonoBehaviour activeMonoBehaviour;
    private ParticleSystem explodeParticleSystem;
    private SphereCollider weaponCollider;

    private float currentExplodeTimer;

    private GameObject poolHolder;
    private ObjectPool<Rigidbody> bombPool;

    #endregion

    #region Initialize Functions

    public void Initialize(MonoBehaviour monoBehaviour, ParticleSystem particleSystem, SphereCollider collider)
    {
        activeMonoBehaviour = monoBehaviour;
        explodeParticleSystem = particleSystem;
        weaponCollider = collider;

        poolHolder = new GameObject("Bomb Pool Holder");
        bombPool = new ObjectPool<Rigidbody>(CreateBomb);
    }

    private Rigidbody CreateBomb()
    {
        GameObject instance = Instantiate(itemModel);
        instance.transform.SetParent(poolHolder.transform, true);
        Rigidbody rigidbody = instance.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        return rigidbody;
    }

    #endregion

    #region Explode Functions

    public void Explode()
    {
        currentExplodeTimer = 0;
        activeMonoBehaviour.StartCoroutine(ExpandHurtBox());
    }

    private IEnumerator ExpandHurtBox()
    {
        Rigidbody bomb = bombPool.Get();
        bomb.gameObject.SetActive(true);
        yield return null;

        weaponCollider.gameObject.SetActive(false);
        bomb.useGravity = true;
        bomb.AddForce(Vector3.forward * throwForce);

        currentExplodeTimer += Time.deltaTime;

        if (currentExplodeTimer >= maxExpodeTimer)
        {
            explodeParticleSystem.Play();

            Collider[] hit = new Collider[10];

            Physics.OverlapSphereNonAlloc(explodeParticleSystem.transform.position, maxDamageArea, hit, hitMask);

            yield return null;
        }
    }

    #endregion

}
