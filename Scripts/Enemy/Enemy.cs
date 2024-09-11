using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public Collider damageCollider;
    public StatManager statManager;

    [Header("Movement")]
    [SerializeField] private float decelerationSpeed;
    [SerializeField] private float rotationSpeed;
    private Vector3 lookDirection;
    IEnumerator moveTowardsTarget;

    public Vector3 target;

    [Header("Find Target")]
    public float lookRange;
    public LayerMask targetMask;
    public float viewAngle;
    public float stopDistance;

    [Header("Weapon")]
    [SerializeField] Transform VFXParent;
    [SerializeField] Transform weaponParentHand;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] GameObject currentWeaponObject;
    Weapon currentWeapon;
    private ParticleSystem[] particleSystems = new ParticleSystem[0];

    [Header("Combat")]
    public int comboCounter = 0;
    public bool canCombo;
    public bool doCombo;
    public WeaponAction currentWeaponAction;
    public ActionAttribute currentWeaponAttribute;
    public Slash currentSlash;
    public LayerMask enemyMask;
    public LayerMask groundMask;

    [Header("Passivs")]
    public List<Passiv> currentPassivs;

    [Header("Flags")]
    public bool isInteracting;
    public bool canRotate;


    void Start()
    {
        SetWeapon(weaponPrefab);
    }

    void Update()
    {
        if(CheckForPlayer())
        {
            if (moveTowardsTarget == null)
            {
                if(Vector3.Distance(target, transform.position) >= stopDistance + 0.5f)
                {
                    moveTowardsTarget = MoveTowardsTarget();
                    StartCoroutine(moveTowardsTarget);
                }
                else
                {
                    WeaponAction nextWeaponAction = currentWeapon.attack1;
                    switch(Random.Range(0,3))
                    {
                        case 0:
                            nextWeaponAction = currentWeapon.attack1;
                            break;
                        case 1:
                            nextWeaponAction = currentWeapon.attack2;
                            break;
                        case 2:
                            nextWeaponAction = currentWeapon.specialAttack;
                            break;

                    }
                    DoSingleStrike(nextWeaponAction);
                }
                
            }
        }
        else
        {
            target = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        isInteracting = animator.GetBool("IsInteracting");
        canRotate = animator.GetBool("CanRotate");
    }

    private void OnEnable()
    {
        statManager.SubOrUnSubToCurrentChange(StatType.HealthPoint, OnHealthChange, true);
    }

    private void OnDisable()
    {
        statManager.SubOrUnSubToCurrentChange(StatType.HealthPoint, OnHealthChange, false);
    }

    private void SetWeapon(GameObject weapon)
    {
        if (weapon == null || currentWeaponObject == weapon) return;
        if (currentWeaponObject != null) Destroy(currentWeaponObject);

        currentWeaponObject = Instantiate(weapon, weaponParentHand);
        currentWeapon = currentWeaponObject.GetComponent<Weapon>();
        if (currentWeapon.canChangeBladeColor)
        {
            var temp = currentWeaponObject.GetComponent<Renderer>().material;
            temp.color = currentWeapon.slashColor;
            temp.SetColor("_EmissionColor", currentWeapon.slashColor);
        }
    }

    private bool CheckForPlayer()
    {
        var targets = Physics.OverlapSphere(transform.position, lookRange, targetMask);

        if (targets.Length == 0) return false;

        if (Vector3.Angle(transform.forward, targets[0].transform.position - transform.position) > viewAngle) return false;


        target = targets[0].transform.position;
        return true;
    }

    private void RotatedTowardsTarget()
    {
        if(!canRotate) return;
        if(target == Vector3.zero) return;
        var targetPos = target;
        targetPos.y = transform.position.y;
        lookDirection = targetPos - transform.position;
        if (lookDirection == Vector3.zero) return;
        var rot = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot,Time.deltaTime * rotationSpeed);
    }

    public void PlayTargetAnimation(string name, bool isInteracting, float transitionTime = 0f)
    {
        animator.SetBool("IsInteracting", isInteracting);
        animator.CrossFade(name, transitionTime);
        this.isInteracting = isInteracting;
    }

    private void DoSingleStrike(WeaponAction action)
    {
        if (isInteracting) return;
        currentWeaponAction = action;
        currentWeaponAttribute = action.actionAttributes[0];
        PlayTargetAnimation(currentWeaponAttribute.actionName, true, 0.05f);
        RotatedTowardsTarget();
        canRotate = currentWeaponAttribute.canRotate;
        animator.SetBool("CanRotate", canRotate);
    }

    public void DoVisuelEffect()
    {
        if (currentWeaponAttribute == null) return;

        Slash slash = currentWeapon.Slashes.FirstOrDefault(x => x.type == currentWeaponAttribute.slashType);
        if (slash != null)
        {
            currentSlash = slash;
            var temp = Instantiate(slash.slashVFX, VFXParent);

            particleSystems = temp.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                //Color baseColor = ps.main.startColor.color;
                Color newColor = currentWeapon.slashColor;
                newColor.a = 0.3f;
                main.startColor = newColor;
            }

            temp.transform.parent = null;
            Destroy(temp, particleSystems[0].main.duration - 0.5f);
            canRotate = false;
            animator.SetBool("CanRotate", canRotate);
            var sortedInteractions = slash.interactions.OrderBy(x => x.time).ToList();
            foreach (var interaction in sortedInteractions)
            {
                StartCoroutine(DoSlashInteraction(interaction));
            }
        }
    }

    public void EnableCanCombo()
    {
        canCombo = true;
    }

    public void DoComboAttackEvent()
    {
        return;
    }

    private void DoDamageInteraction(SlashInteraction interaction)
    {
        var targetList = Physics.OverlapSphere(transform.position, interaction.hitRange, enemyMask);
        targetList = targetList.Where(x => InHitAngle(x.transform, interaction)).ToArray();
        if (targetList.Length < 0) return;

        DamagePackage damagePackage = new DamagePackage
        {
            damage = interaction.damage,
            attackType = interaction.attackType,
            popCondition = interaction.popCondition,
            conditionMultiplier = statManager.GetCurrentStat(StatType.conditionMultiplier),
            critRate = statManager.GetCurrentStat(StatType.CritRate),
            critDamage = statManager.GetCurrentStat(StatType.CritDamage)
        };

        damagePackage = UsePassivs(PassivType.PreDamage, damagePackage);

        DamagePackage returnPackage;
        foreach (var target in targetList)
        {
            if (target.gameObject.TryGetComponent(out IHitable hitable))
            {
                returnPackage = hitable.GetHit(damagePackage);
                returnPackage = UsePassivs(PassivType.PostDamage, returnPackage);
            }
        }
    }

    private bool InHitAngle(Transform target, SlashInteraction interaction)
    {
        Vector3 enemyPos = target.position;
        enemyPos.y = transform.position.y;
        float enemyangle = Vector3.Angle(transform.forward, (enemyPos - transform.position).normalized);
        if (interaction.isBox)
        {
            if (enemyangle > 90) return false;

            var distance = Vector3.Distance(enemyPos, transform.position);
            float dis = distance * Mathf.Cos((90 - enemyangle) * Mathf.Deg2Rad);
            return dis <= interaction.boxRange;
        }
        else
            return enemyangle <= interaction.hitAngle;
    }

    private void Death()
    {
        damageCollider.enabled = false;
        Destroy(gameObject, 2);
        isInteracting = true;
        //PlayTargetAnimation("Death", true, 0.5f);
    }

    public void OnHealthChange(float currentHealth)
    {
        if (currentHealth <= 0) Death();
    }

    private DamagePackage UsePassivs(PassivType passivType, DamagePackage damagePackage)
    {
        var passivToUse = currentPassivs.Where(x => x.GetPassivType() == passivType).ToList();
        foreach (var passiv in passivToUse)
        {
            damagePackage = passiv.DoPassiv(statManager, damagePackage);
        }
        return damagePackage;
    }

    IEnumerator DoSlashInteraction(SlashInteraction interaction)
    {
        yield return new WaitForSeconds(interaction.time);
        DoDamageInteraction(interaction);
    }

    IEnumerator MoveTowardsTarget()
    {
        while(true)
        {
            if (Vector3.Distance(target, transform.position) <= stopDistance)
                break;
            RotatedTowardsTarget();
            animator.SetFloat("MovementSpeed", statManager.GetCurrentStat(StatType.MoveSpeed));
            yield return null;
        }
        animator.SetFloat("MovementSpeed", 0f);
        moveTowardsTarget = null;
    }
}
