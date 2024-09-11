using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour , ISaveable
{
    public Animator animator;
    public PlayerInputActions inputActions;
    public StatManager statManager;

    [Header("Movement")]
    [SerializeField] private float decelerationSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float isInteractingRotationSpeed;
    private Vector2 moveInput;
    private Vector3 lookDirection;

    [Header("Camera")]
    [SerializeField] Vector2 lookSpeed = new Vector2(25f, 15f);
    [SerializeField] Transform lookAtTarget;
    [SerializeField] Transform direction;
    float mouseX, mouseY;
    Vector2 lookInput;

    [Header("Weapon")]
    [SerializeField] Transform VFXParent;
    [SerializeField] Transform weaponParentHand;
    [SerializeField] Transform weaponParentBack;
    [SerializeField] WeaponRecipe weaponRecipe;
    private int weaponcounter = 0;
    [SerializeField] WeaponItemWithExtraData[] weaponItems = new WeaponItemWithExtraData[4];
    [SerializeField] GameObject currentWeaponObject;
    WeaponItem currentWeapon;
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
    public bool isGrounded;
    public bool weaponEquiped;
    public bool isSprinting;
    public bool canRotate;
    public bool isUsingController;

    [Header("ViewSwitch")]
    public bool isIso = false;
    [SerializeField] GameObject rdPersonCamera;
    [SerializeField] GameObject isoCamera;

    void Start()
    {
        //GetWeaponFromObject();
        SetWeapon(weaponItems[0]);
        ToggleCamera();
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();

        if(Input.GetKeyDown(KeyCode.O))
        {
            //statManager.ChangeStat(StatType.HealthPoint, -5);
            //GetWeaponFromObject();
        }
    }

    private void LateUpdate()
    {
        mouseX += lookInput.x * Time.deltaTime * lookSpeed.x;
        mouseY -= lookInput.y * Time.deltaTime * lookSpeed.y;
        mouseY = Mathf.Clamp(mouseY, -55, 75);
        lookAtTarget.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        direction.rotation = Quaternion.Euler(0, mouseX, 0);
        isInteracting = animator.GetBool("IsInteracting");
        comboCounter = animator.GetInteger("ComboCounter");
        canRotate = animator.GetBool("CanRotate");
        animator.SetFloat("AttackSpeed", statManager.GetCurrentStat(StatType.AttackSpeed));
    }

    private void OnEnable()
    {
        if (inputActions == null) inputActions = new PlayerInputActions();

        inputActions.InputAction.Move.performed += OnMovementInput;
        inputActions.InputAction.Move.canceled += OnMovementInput;

        inputActions.InputAction.Sprint.started += OnSprintInput;
        inputActions.InputAction.Sprint.canceled += OnSprintInput;

        inputActions.InputAction.Camera.performed += OnLookInput;
        inputActions.InputAction.Camera.canceled += OnLookInput;

        inputActions.InputAction.Dodge.performed += OnDodgeInput;

        inputActions.InputAction.Attack1.performed += OnAttack1Input;
        inputActions.InputAction.Attack2.performed += OnAttack2Input;
        inputActions.InputAction.SpecialAttack.performed += OnSpecialAttackInput;

        inputActions.InputAction.SwapWeapon.performed += OnWeaponSwapInput;

        inputActions.InputAction.SwapCamera.performed += OnCameraToggleInput;

        inputActions.InputAction.Consumable.performed += OnToggleWeaponInput;

        
        inputActions.InputAction.Enable();


        InputSystem.onActionChange += (obj, change) =>
        {
            if (change == InputActionChange.ActionPerformed)
            {
                var inputAction = (InputAction)obj;
                var lastControl = inputAction.activeControl;
                var lastDevice = lastControl.device;


                if(lastDevice.displayName.Contains("Controller") && !isUsingController)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    isUsingController = true;
                }
                else if(!lastDevice.displayName.Contains("Controller") && isUsingController)
                {
                    Cursor.lockState = isIso ? CursorLockMode.None : CursorLockMode.Locked;
                    isUsingController = false;
                }
            }
        };
    }

    private void OnDisable()
    {
        inputActions.InputAction.Move.performed -= OnMovementInput;
        inputActions.InputAction.Move.canceled -= OnMovementInput;

        inputActions.InputAction.Sprint.started -= OnSprintInput;
        inputActions.InputAction.Sprint.canceled -= OnSprintInput;

        inputActions.InputAction.Camera.performed -= OnLookInput;
        inputActions.InputAction.Camera.canceled -= OnLookInput;

        inputActions.InputAction.Dodge.performed -= OnDodgeInput;

        inputActions.InputAction.Attack1.performed -= OnAttack1Input;
        inputActions.InputAction.Attack2.performed -= OnAttack2Input;
        inputActions.InputAction.SpecialAttack.performed -= OnSpecialAttackInput;

        inputActions.InputAction.SwapWeapon.performed -= OnWeaponSwapInput;

        inputActions.InputAction.SwapCamera.performed -= OnCameraToggleInput;

        inputActions.InputAction.Consumable.performed -= OnToggleWeaponInput;
    }

    private void SetWeapon(WeaponItemWithExtraData weapon)
    {
        var tempWeapn = weapon.GetWeaponObject().GetWeaponItem();
        if (weapon == null || currentWeapon == tempWeapn || tempWeapn == null) return;
        if (currentWeaponObject != null) UnEquipeWeapon();

        currentWeaponObject = Instantiate(tempWeapn.weaponPrefab, weaponEquiped ? weaponParentHand : weaponParentBack);

        currentWeapon = tempWeapn;
        EquipeWeapon();
        
        if (currentWeapon.canChangeBladeColor)
        {
            var temp = currentWeaponObject.GetComponent<Renderer>().material;
            temp.color = currentWeapon.slashColor;
            temp.SetColor("_EmissionColor", currentWeapon.slashColor);
        }
    }

    private void GetWeaponFromObject()
    {
        weaponcounter++;

        if (weaponcounter >= weaponItems.Length) weaponcounter = 0;
    }

    private void UnEquipeWeapon()
    {
        foreach(StatChangeValue statChange in currentWeapon.statChangeValues)
        {
            statManager.ChangeStat(statChange.type, -statChange.value);
        }
        Destroy(currentWeaponObject);
        currentWeapon = null;
        currentPassivs = new List<Passiv>();
    }

    private void EquipeWeapon()
    {
        foreach (StatChangeValue statChange in currentWeapon.statChangeValues)
        {
            statManager.ChangeStat(statChange.type, statChange.value);
        }
        currentPassivs = currentWeapon.passivs;
    }

    private void HandleRotation()
    {
        if (!canRotate) return;
        if (isIso)
        {
            if(!isUsingController && isInteracting)
            {
                RotatedTowardsMouse();
                return;
            }  
            else
                lookDirection = (transform.position + new Vector3(moveInput.x, 0, moveInput.y).ToIso()) - transform.position;

        }
        else
        {
            lookDirection = direction.forward * moveInput.y;
            lookDirection += direction.right * moveInput.x;
        }

        if (lookDirection == Vector3.zero) return;

        var rot = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 
                             Time.deltaTime * (isInteracting ? isInteractingRotationSpeed : rotationSpeed));
    }

    private void RotatedTowardsMouse()
    {
        if (isIso)
        {
            if (isUsingController)
            {
                lookDirection = (transform.position + new Vector3(moveInput.x, 0, moveInput.y).ToIso()) - transform.position;
            }
            else
            {
                RaycastHit groundHit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out groundHit, 100, groundMask))
                {
                    lookDirection = groundHit.point - transform.position;
                }
                else
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 15f);
                    mousePos.y = transform.position.y;
                    lookDirection = mousePos - transform.position;
                }
            }
        }
        else
        {
            lookDirection = direction.forward * moveInput.y;
            lookDirection += direction.right * moveInput.x;
        }
        if (lookDirection == Vector3.zero) return;
        lookDirection.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }

    private void HandleMovement()
    {
        if (isInteracting) return;
        var currentMovementSpeed = animator.GetFloat("MovementSpeed");
        if (moveInput.magnitude == 0 && currentMovementSpeed > 0.2f)
            animator.SetFloat("MovementSpeed", Mathf.Lerp(currentMovementSpeed, moveInput.magnitude, Time.deltaTime * decelerationSpeed));
        else
            animator.SetFloat("MovementSpeed", Mathf.Clamp01(moveInput.magnitude) * (isSprinting ? 2 : 1) * statManager.GetCurrentStat(StatType.MoveSpeed));
    }

    public void PlayTargetAnimation(string name, bool isInteracting, float transitionTime = 0f)
    {
        animator.SetBool("IsInteracting", isInteracting);
        animator.CrossFade(name, transitionTime);
        this.isInteracting = isInteracting;
    }

    public void ToggleWeaponEquipe()
    {
        if (isInteracting) return;
        if (weaponEquiped)
            PlayTargetAnimation("Unequipe Weapon", false, 0.15f);
        else
            PlayTargetAnimation("Equipe Weapon", false, 0.15f);
    }
    public void SwapWeaponParent()
    {
        currentWeaponObject.transform.parent = weaponEquiped ? weaponParentBack : weaponParentHand;
        currentWeaponObject.transform.localPosition = Vector3.zero;
        currentWeaponObject.transform.localEulerAngles = Vector3.zero;
        weaponEquiped = !weaponEquiped;
        animator.SetBool("WeaponEquiped", weaponEquiped);
    }

    public void EnableCanCombo()
    {
        canCombo = true;
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnSprintInput(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
        if (isSprinting && weaponEquiped)
            ToggleWeaponEquipe();
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnDodgeInput(InputAction.CallbackContext context) 
    {
        if (isInteracting) return;
        HandleRotation();
        PlayTargetAnimation("Dodge", true);
        canRotate = false;
        animator.SetBool("CanRotate", canRotate);
    }

    private void OnAttack1Input(InputAction.CallbackContext context)
    {
        if (currentWeapon == null) return;

        if(currentWeapon.attack1.actionType == ActionType.SingleStrike)
            DoSingleStrike(currentWeapon.attack1);
        else 
            DoComboAttack(currentWeapon.attack1);
    }

    private void OnAttack2Input(InputAction.CallbackContext context)
    {
        if (currentWeapon == null) return;

        if (currentWeapon.attack2.actionType == ActionType.SingleStrike)
            DoSingleStrike(currentWeapon.attack2);
        else 
            DoComboAttack(currentWeapon.attack2);
    }

    private void OnSpecialAttackInput(InputAction.CallbackContext context)
    {
        if (currentWeapon == null) return;

        if (currentWeapon.specialAttack.actionType == ActionType.SingleStrike)
            DoSingleStrike(currentWeapon.specialAttack);
        else 
            DoComboAttack(currentWeapon.specialAttack);
    }

    private void OnWeaponSwapInput(InputAction.CallbackContext context)
    {
        if(isInteracting) return;

        Vector2 temp = context.ReadValue<Vector2>();

        if (temp == Vector2.up)
            SetWeapon(weaponItems[0]);
        else if(temp == Vector2.down)
            SetWeapon(weaponItems[1]);
        else if(temp == Vector2.left)
            SetWeapon(weaponItems[2]);
        else if(temp == Vector2.right)
            SetWeapon(weaponItems[3]);
        
    }

    private void OnToggleWeaponInput(InputAction.CallbackContext context)
    {
        ToggleWeaponEquipe();
    }

    private void DoSingleStrike(WeaponAction action)
    {
        if (isInteracting) return;
        if (!weaponEquiped)
        {
            ToggleWeaponEquipe();
            return;
        }
        currentWeaponAction = action;
        currentWeaponAttribute = action.actionAttributes[0];
        PlayTargetAnimation(currentWeaponAttribute.actionName, true,0.05f);
        canRotate = currentWeaponAttribute.canRotate;
        animator.SetBool("CanRotate", canRotate);
        RotatedTowardsMouse();
    }

    private void DoComboAttack(WeaponAction action)
    {
        if (isInteracting && !canCombo) return;
        if (!weaponEquiped)
        {
            ToggleWeaponEquipe();
            return;
        }
        if (comboCounter == 0)
        {
            currentWeaponAction = action;
            currentWeaponAttribute = action.actionAttributes[comboCounter++];
            PlayTargetAnimation(currentWeaponAttribute.actionName, true, 0.1f);
            comboCounter = comboCounter == action.actionAttributes.Count ? 0 : comboCounter;
            animator.SetInteger("ComboCounter", comboCounter);
            canCombo = false;
            canRotate = currentWeaponAttribute.canRotate;
            animator.SetBool("CanRotate", canRotate);
            RotatedTowardsMouse();
        }
        else
        {
            canCombo = false;
            doCombo = true;
        }
    }

    public void DoComboAttackEvent()
    {
        if (doCombo)
        {
            currentWeaponAttribute = currentWeaponAction.actionAttributes[comboCounter++];
            PlayTargetAnimation(currentWeaponAttribute.actionName, true, 0.05f);
            comboCounter = comboCounter == currentWeaponAction.actionAttributes.Count ? 0 : comboCounter;
            animator.SetInteger("ComboCounter", comboCounter);
            doCombo = false;
            canRotate = currentWeaponAttribute.canRotate;
            animator.SetBool("CanRotate", canRotate);
            RotatedTowardsMouse();
        }
        canCombo = false;
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
                Color baseColor = ps.main.startColor.color;
                Color newColor = currentWeapon.slashColor;
                newColor.a = baseColor.a;
                main.startColor = newColor;
            }

            temp.transform.parent = null;
            Destroy(temp, particleSystems[0].main.duration - 0.5f);
            canRotate = false;
            animator.SetBool("CanRotate", canRotate);
            var sortedInteractions = slash.interactions.OrderBy(x=> x.time).ToList();
            foreach(var interaction in sortedInteractions)
            {
                StartCoroutine(DoSlashInteraction(interaction));
            }
        }
    }

    private void OnCameraToggleInput(InputAction.CallbackContext context)
    {
        isIso = !isIso;
        ToggleCamera();
    }

    private void ToggleCamera()
    {
        isoCamera.SetActive(isIso);
        rdPersonCamera.SetActive(!isIso);
        Cursor.lockState = isIso ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void DoDamageInteraction(SlashInteraction interaction)
    {
        var targetList = Physics.OverlapSphere(transform.position, interaction.hitRange, enemyMask);
        targetList = targetList.Where(x => InHitAngle(x.transform, interaction)).ToArray();
        if (targetList.Length <= 0) return;

        DamagePackage damagePackage = new DamagePackage
        {
            damage = interaction.damage + statManager.GetCurrentStat(StatType.Attack) + currentWeapon.weaponDamage,
            attackType = interaction.attackType,
            popCondition = interaction.popCondition,
            conditionMultiplier = statManager.GetCurrentStat(StatType.conditionMultiplier),
            critRate = statManager.GetCurrentStat(StatType.CritRate),
            critDamage = statManager.GetCurrentStat(StatType.CritDamage)
        };

        damagePackage = UsePassivs(PassivType.PreDamage, damagePackage);
        //damagePackage.ApplyPassivs(currentPassivs.Where(x => x.GetPassivType() == PassivType.PreDamage).ToList(), statManager);

        DamagePackage returnPackage;
        foreach (var target in targetList)
        {
            if (target.gameObject.TryGetComponent(out IHitable hitable))
            {
                returnPackage = hitable.GetHit(damagePackage);
                returnPackage = UsePassivs(PassivType.PostDamage, returnPackage);
                //returnPackage.ApplyPassivs(currentPassivs.Where(x => x.GetPassivType() == PassivType.PostDamage).ToList(), statManager);
            }
        }
    }

    private DamagePackage UsePassivs(PassivType passivType, DamagePackage damagePackage)
    {
        var passivsToUse = currentPassivs.Where(x => x.GetPassivType() == passivType).ToList();
        foreach (var passiv in passivsToUse)
        {
            damagePackage = passiv.DoPassiv(statManager, damagePackage);
        }
        return damagePackage;
    }

    private bool InHitAngle(Transform target,SlashInteraction interaction)
    {
        Vector3 enemyPos = target.position;
        enemyPos.y = transform.position.y;
        float enemyangle = Vector3.Angle(transform.forward, (enemyPos - transform.position).normalized);
        if(interaction.isBox)
        {
            if (enemyangle > 90) return false;

            var distance = Vector3.Distance(enemyPos, transform.position);
            float dis = distance * Mathf.Cos((90 - enemyangle) * Mathf.Deg2Rad);
            return dis <= interaction.boxRange;
        }
        else
            return enemyangle <= interaction.hitAngle;
    }

    IEnumerator DoSlashInteraction(SlashInteraction interaction)
    {
        yield return new WaitForSeconds(interaction.time);
        DoDamageInteraction(interaction);
    }

    public object OnSave()
    {
        return weaponItems;
    }

    public void OnLoad(object state)
    {
        weaponItems = (WeaponItemWithExtraData[]) state;
    }
}

[Serializable]
public class WeaponItemWithExtraData
{
    public WeaponName weaponName;
    public List<StatChangeValue> statChangeValues;

    public WeaponObject GetWeaponObject()
    {
        return ScriptableData.instance.scriptableWeapons.Where(x=>x.weaponName == weaponName).FirstOrDefault();
    }
}

public class DamagePackage
{
    public AttackType attackType;
    public float damage;
    public float bonusDamage = 0;

    public float critRate;
    public float critDamage;
    public bool isCrit = false;

    public Condition condition;
    public bool popCondition;
    public float conditionMultiplier;

    public DamagePackage() { }

    public DamagePackage(DamagePackage damagePackage)
    {
        attackType = damagePackage.attackType;
        damage = damagePackage.damage;
        bonusDamage = damagePackage.bonusDamage;

        critRate = damagePackage.critRate;
        critDamage = damagePackage.critDamage;
        isCrit = damagePackage.isCrit;

        condition = damagePackage.condition;
        popCondition = damagePackage.popCondition;
        conditionMultiplier = damagePackage.conditionMultiplier;
    }

    //public void ApplyPassivs(List<Passiv> passivsToApply, StatManager statManager)
    //{
    //    foreach (var passiv in passivsToApply)
    //    {
    //        passiv.DoPassiv(statManager, this);
    //    }
    //}
}
