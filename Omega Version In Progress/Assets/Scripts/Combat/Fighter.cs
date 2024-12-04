using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Omega.Movement;
using Omega.Core;
using Omega.Saving;
using Omega.Attributes;
using Omega.Stats;
using Omega.LazyValue;
using Omega.Inventories;

namespace Omega.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        //we set up a range for future weapons but also to stop player close to-
        //- the enemy and not inside it
        //Attack speed
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] float autoAttackRange = 4f;

        //Creating a variable target
        //later changed from transform to Health class
        Health target;
        Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if(equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if(target == null) return;
            if(target.Dead())
            {
                target = FindNewTargetInRange();
                if(target == null) return;
            }
    
            if(!GetInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);   
            }    
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if(weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public Transform GetHandTransform(bool isRightHand)
        {
            if(isRightHand)
            {
                return rightHandTransform;
            }
            else
            {
                return leftHandTransform;
            }
        }

        //AttachWeapon method when you trigget attack animation on target
        private void AttackBehaviour()
        {
            //we tell the player to look where it's attacking
            transform.LookAt(target.transform);
            if(timeSinceLastAttack > timeBetweenAttacks) 
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private Health FindNewTargetInRange()
        {
            Health best = null;
            float bestDistance = Mathf.Infinity;
            foreach (var candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(transform.position, candidate.transform.position);
                if(candidateDistance < bestDistance)
                {
                    best = candidate;
                    bestDistance = candidateDistance;
                }
            }
            return best;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up);
            foreach (var hit in raycastHits)
            {
                Health health = hit.transform.GetComponent<Health>();
                if(health == null) continue;
                if(health.Dead()) continue;    
                if(health.gameObject == gameObject) continue;    
                yield return health;       
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        //Animation event Melee weapon - called within the animator
        //Also when the target takes damage
        void Hit()
        {
            if(target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            BaseStats targetBaseStats = target.GetComponent<BaseStats>();
            if(targetBaseStats != null)
            {
                float defence = targetBaseStats.GetStat(Stat.Defence);
                damage /= 1 + defence / damage;
            }

            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            
            if(currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        //Animation event projectile
        void Shoot()
        {
            //Recall Hit() method
            Hit();
        }

        //creating a CanAttack() method to decide when to ignore enemies-
        //-example when they are dead
        public bool CanAttack(GameObject combatTarget)
        {
            //the if statemen below was moved from the PC script
            //if(target == null) continue; - changed to the below one
            if(combatTarget == null) {return false;}
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.Dead();
        }

        //boolean to check range from player to targe is less than weaponRange.
        private bool GetInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetRange();
        }

        //method to attack
        public void Attack(GameObject combatTarget)
        {
            //we call StartAction() from AS script before attacking
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        //method to cancel attack
        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        } 
    }
}