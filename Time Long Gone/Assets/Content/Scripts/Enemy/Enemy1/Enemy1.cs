using Content.Scripts.Camera;
using Content.Scripts.Variables;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

namespace Enemy
{
    public class Enemy1 : MonoBehaviour, IEnemy
    {
        [SerializeField] private NavMeshAgent navAgent;
        [SerializeField] private Animator animator;
        [Space(10)]
        [SerializeField] private float MaxHealth;
        [SerializeField] private float StunTime;
        [SerializeField] private float ParryTime;
        [SerializeField] private int SequenceParryCount;
        [SerializeField] private int MinThrownProj;
        [SerializeField] private int MaxThrownProj;
        [SerializeField] private GameObject Projectile;
        [SerializeField] private Transform ProjectilesParent;
        [SerializeField][Range(0, 1)] private float VulnerableOnParryChance;
        [SerializeField] private AnimationCurve jumpHeightCurve;
        [SerializeField] private AnimationCurve jumpDistanceCurve;
        private List<StateMachine> Stages;

        public float Health { get; private set; }
        public EnemyStatus Status { get; private set; }
        public int Stage { get; private set; }
        public bool ActiveAI { get; set; } = true;

        [SerializeField][HideInInspector] private StateMachine currSM;
        private FloatVariable TimeToRemember;
        private FloatVariable TimeBetweenEntries;
        private BoolVariable IsRewinding;
        private Player.Player player;
        private LinkedList<Vector3> playerPositions = new LinkedList<Vector3>();
        private List<Vector3> projectilesSpots;
        private LinkedList<float> pastHealth = new LinkedList<float>();
        private Vector3 escapeTargetPos;
        private int sequenceParries = 0;
        private LinkedList<TimeEntry> timeEntries = new LinkedList<TimeEntry>();
        private int maxentries;
        private int entries;

        void Awake()
        {
            IsRewinding = GameLogic.Instance.IsRewinding;
            TimeToRemember = GameLogic.Instance.TimeToRemember;
            TimeBetweenEntries = GameLogic.Instance.TimeBetweenEntries;
            Stages = Level1DataManager.Instance.EnemyStateMachines;
            Stage = 0;
            currSM = Stages[Stage];
            Health = MaxHealth;
            Status = EnemyStatus.Passive;
        }

        void Start()
        {
            entries = 0;
            maxentries = (int)(TimeToRemember.Value / TimeBetweenEntries.Value);
            projectilesSpots = GameObject.FindGameObjectsWithTag("Projectiles Spot").Select(o => o.transform.position).ToList();
            currSM.Start();
            currSM.GetCurrentState().StateEnter(this);
            player = FindObjectOfType<Player.Player>();
            for (int i = 0; i < 40; i++) {
                playerPositions.AddLast(player.transform.position);
                pastHealth.AddLast(Health);
            }
            StartCoroutine(UpdateRandomParameters());
            StartCoroutine(CalculatePlayerAvgDeltaPos());
            StartCoroutine(Cycle());
        }

        void Update()
        {
            if (!ActiveAI || IsRewinding.Value) return;
            animator.SetFloat("Randomizer", UnityEngine.Random.value);
            animator.SetFloat("MovementSpeed", navAgent.velocity.magnitude);
            UpdateSM();
            currSM.Tick(this);
        }
        void UpdateSM()
        {
            currSM.SetFloat("Distance", Vector3.Distance(transform.position, player.transform.position));
            currSM.SetFloat("TimeSinceRest", currSM.GetFloat("TimeSinceRest") + Time.deltaTime);
            currSM.SetFloat("TimeSinceCharge", currSM.GetFloat("TimeSinceCharge") + Time.deltaTime);
            currSM.SetFloat("DistanceToProjectileSpot", projectilesSpots.Select(p => Vector3.Distance(transform.position, p)).Min());
            currSM.SetFloat("AngleToPlayer", Vector3.SignedAngle(player.transform.position - transform.position, transform.forward, Vector3.up));
        }

        public void PlayAnimation(string anim, float crossfade) => animator.CrossFade(anim, crossfade);

        #region Corutines
        IEnumerator UpdateRandomParameters()
        {
            YieldInstruction cycle = new WaitForSeconds(0.5f);
            while (true)
            {
                currSM.SetFloat("RestingChance", UnityEngine.Random.value);
                currSM.SetFloat("JumpAttackChance", UnityEngine.Random.value);
                currSM.SetFloat("ChanceToEscape", UnityEngine.Random.value);
                currSM.SetFloat("ChanceForProjectiles", UnityEngine.Random.value);
                currSM.SetFloat("RepulsingChance", UnityEngine.Random.value);
                currSM.SetFloat("ChanceToBreak", UnityEngine.Random.value);
                currSM.SetFloat("StrongAttackChance", UnityEngine.Random.value);
                currSM.SetFloat("BackAttackChance", UnityEngine.Random.value);
                currSM.SetFloat("SequenceChance", UnityEngine.Random.value);
                currSM.SetFloat("AreaAttackChance", UnityEngine.Random.value);
                yield return cycle;
            }
        }
        IEnumerator CalculatePlayerAvgDeltaPos()
        {
            YieldInstruction cycle = new WaitForSeconds(0.1f);
            while (true)
            {
                pastHealth.RemoveFirst();
                pastHealth.AddLast(Health);
                playerPositions.RemoveFirst();
                playerPositions.AddLast(player.transform.position);
                float distance = 0;
                foreach (Vector3 pos in playerPositions) distance += Vector3.Distance(pos, playerPositions.First.Value);
                currSM.SetFloat("PlayerAvgDeltaPos", distance / 40);
                currSM.SetFloat("HealthDelta", pastHealth.First.Value - Health);
                yield return cycle;
            }
        }
        IEnumerator JumpTowardsPlayer()
        {
            navAgent.ResetPath();
            IncrementCombo();
            var pos = player.transform.position;
            pos.y -= player.GetComponent<CharacterController>().height / 2;
            transform.LookAt(pos);
            var startpos = transform.position;
            var distance = Vector3.Distance(transform.position, pos);
            //var time = distance / 15;
            var time = 2.05f;
            //animator.SetFloat("JumpSpeed", 1.25f / time);
            float currTime = 0;
            while (currTime < time)
            {
                Vector3 currpos = Vector3.Lerp(startpos, pos, jumpDistanceCurve.Evaluate(currTime / time));
                currpos.y += jumpHeightCurve.Evaluate(currTime / time) * (distance / 4);
                navAgent.Warp(currpos);
                currTime += Time.deltaTime;
                yield return null;
            }
            transform.DOLookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), 0.5f);
            yield return new WaitForSeconds(1f);
            currSM.SetBool("IsAttacking", false);
        }
        IEnumerator SequenceAttack()
        {
            sequenceParries = 0;
            while (true)
            {
                if (!currSM.GetBool("IsAttacking"))
                {
                    yield break;
                }
                if (sequenceParries >= SequenceParryCount)
                {
                    currSM.SetBool("IsParried", true);
                    currSM.SetBool("IsAttacking", false);
                    StopCoroutine(ResetAttack());
                    Status = EnemyStatus.Vulnerable;
                    yield return new WaitForSeconds(ParryTime / 2);
                    if (Status == EnemyStatus.Stunned)
                    {
                        currSM.SetBool("IsParried", false);
                        yield break;
                    }
                    Status = EnemyStatus.Parried;
                    yield return new WaitForSeconds(ParryTime / 2);
                    currSM.SetBool("IsParried", false);
                    Status = EnemyStatus.Passive;
                }
                yield return null;
            }
        }
        IEnumerator ThrowProjectiles()
        {
            yield return null;
            bool restoring = false;
            GameObject restoredProjectile = null;
            int projToThrow = UnityEngine.Random.Range(MinThrownProj, MaxThrownProj);
            for(int i = 0; i<ProjectilesParent.childCount; i++)
            {
                if(ProjectilesParent.GetChild(i).name.Contains(Projectile.name))
                {
                    restoring = true;
                    restoredProjectile = ProjectilesParent.GetChild(i).gameObject;
                    if (currSM.GetInt("ProjectilesThrown") >= projToThrow) projToThrow = currSM.GetInt("ProjectilesThrown") + 1;
                    break;
                }
            }
            
            Vector3 spot = projectilesSpots.
                Aggregate((min, next) => Vector3.Distance(transform.position, min) < Vector3.Distance(transform.position, next) ? min : next);
            spot.y = transform.position.y;
            while (currSM.GetInt("ProjectilesThrown") < projToThrow)
            {
                Rigidbody rb;
                GameObject proj;
                if (!restoring)
                {
                    transform.DOLookAt(spot, 1f);
                    if (Vector3.SignedAngle((spot - transform.position), transform.forward, Vector3.up) > 0) animator.Play("RotatingRight");
                    else animator.Play("RotatingLeft");
                    yield return new WaitForSeconds(1f);
                    animator.Play("PickUp");
                    yield return new WaitForSeconds(0.2f);
                    proj = GameObject.Instantiate(Projectile);
                    proj.transform.SetParent(ProjectilesParent, true);
                    rb = proj.GetComponent<Rigidbody>();
                    rb.detectCollisions = false;
                    proj.transform.position = new Vector3(ProjectilesParent.position.x, ProjectilesParent.position.y, ProjectilesParent.position.z);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    proj = restoredProjectile;
                    rb = proj.GetComponent<Rigidbody>();
                    rb.detectCollisions = false;
                    restoring = false;
                }
                transform.DOLookAt(player.transform.position, 0.5f);
                if (Vector3.SignedAngle((player.transform.position - transform.position), transform.forward, Vector3.up) > 0) 
                    animator.Play("RotatingRight");
                else animator.Play("RotatingLeft");
                yield return new WaitForSeconds(1f);
                animator.Play("Throw");
                yield return new WaitForSeconds(0.5f);
                proj.transform.SetParent(null, true);
                rb.isKinematic = false;
                rb.detectCollisions = true;

                Vector3 direction = player.transform.position - proj.transform.position;
                var h = direction.y;
                direction.y = 0;
                var dist = direction.magnitude;
                var a = UnityEngine.Random.Range(10f, 45f) * Mathf.Deg2Rad;
                direction.y = dist * Mathf.Tan(a);
                dist += h / Mathf.Tan(a);
                var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
                rb.velocity = vel * direction.normalized;
                rb.angularVelocity = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value).normalized * UnityEngine.Random.Range(1f,5f);
                currSM.SetInt("ProjectilesThrown", currSM.GetInt("ProjectilesThrown") + 1);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
            }
            currSM.SetBool("IsThrowing", false);
        }
        #endregion

        #region Attacks
        public void IncrementCombo() => currSM.SetInt("AttacksInCombo", currSM.GetInt("AttacksInCombo") + 1);
        public void BasicAttack()
        {
            animator.SetTrigger("BasicAttack");
            /*var currAnim = animator.GetCurrentAnimatorStateInfo(0);
            if (!currAnim.IsName("BasicAttack1") && !currAnim.IsName("BasicAttack2")
                && !currAnim.IsName("BasicAttack3") && !currAnim.IsName("BasicAttack4"))
            {
                PlayAnimation("BasicAttack1", 0.1f);
            }
            else animator.SetTrigger("BasicAttack");*/
        }
        public void CountBreakTime() => currSM.SetFloat("BreakTime", currSM.GetFloat("BreakTime") + Time.deltaTime);
        public void WaitForAttackEnd() => StartCoroutine(ResetAttack());
        public void StartSequenceAttack() => StartCoroutine(SequenceAttack());
        public void ThrowingProjectiles() => StartCoroutine(ThrowProjectiles());
        #endregion

        #region Movement
        public void LookAtPlayer(float timeToRotate) => transform.DOLookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), timeToRotate);
        public void Chase() => navAgent.SetDestination(player.transform.position);
        public void ChangeAgentSpeed(float speed) => navAgent.speed = speed;
        public void Jump() => StartCoroutine(JumpTowardsPlayer());
        public void FindEscapeTarget()
        {
            NavMeshHit hit;
            for (int i = 0; i++ < 50;)
            {
                Vector3 randDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
                float randDist = UnityEngine.Random.Range(10, 20);
                var pos = player.transform.position + randDir * randDist;
                if (NavMesh.SamplePosition(pos, out hit, 5, NavMesh.AllAreas)
                    && !Physics.Raycast(transform.position, (hit.position - transform.position).normalized, Vector3.Distance(transform.position, hit.position)))
                {
                    escapeTargetPos = hit.position;
                    return;
                }
            }
            escapeTargetPos = player.transform.position;
        }
        public void Escape()
        {
            if (navAgent.remainingDistance < 5) FindEscapeTarget();
            navAgent.SetDestination(escapeTargetPos);
        }
        public void StopAgent() { navAgent.ResetPath(); navAgent.velocity = Vector3.zero; }
        public void RunToProjectiles()
        {
            navAgent.SetDestination(projectilesSpots.
                Aggregate((min, next) => Vector3.Distance(transform.position, min) < Vector3.Distance(transform.position, next) ? min : next));
        }
        #endregion

        #region Receiving
        public void ReceiveHit(float damage)
        {
            if (Status != EnemyStatus.Untouchable) {
                Health -= damage;
                animator.CrossFade("Shake", 0.2f, 1);
                if (Health <= MaxHealth - (Stage + 1) * (MaxHealth / Stages.Count))
                    StartCoroutine(NextStage());
            }
        }

        public void ReceiveParry()
        {
            if (currSM.GetCurrentState().stateName == "ThrowingProjectiles") return;
            if (currSM.GetCurrentState().stateName == "SequencionalAttack")
            {
                sequenceParries++;
                return;
            }
            Status = EnemyStatus.Parried;
            currSM.SetBool("IsParried", true);
            currSM.SetBool("IsAttacking", false);
            StopCoroutine(ResetAttack());
            StartCoroutine(ResetParry());
        }

        public void ReceiveStun()
        {
            if (Status == EnemyStatus.Vulnerable)
            {
                CinemachineSwitcher.Instance.Switch(true);
                Status = EnemyStatus.Stunned;
                currSM.SetBool("IsParried", false);
                currSM.SetBool("IsStunned", true);
                currSM.SetBool("IsAttacking", false);
                StopCoroutine(ResetAttack());
                StartCoroutine(ResetStun());
            }
        }
        #endregion

        #region Resets
        IEnumerator ResetAttack()
        {
            yield return null;
            while (true)
            {
                if (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.value);
                    currSM.SetBool("IsAttacking", false);
                    yield break;
                }
                yield return null;
            }
        }
        IEnumerator ResetStun()
        {
            yield return new WaitForSeconds(StunTime - 1);
            CinemachineSwitcher.Instance.Switch(false);
            PlayAnimation("StunEnd", 0.1f);
            yield return new WaitForSeconds(1);
            currSM.SetBool("IsStunned", false);
            Status = EnemyStatus.Passive;
        }
        IEnumerator ResetParry()
        {
            if (UnityEngine.Random.value <= VulnerableOnParryChance) Status = EnemyStatus.Vulnerable;
            yield return new WaitForSeconds(ParryTime / 2);
            if (Status == EnemyStatus.Stunned) {
                currSM.SetBool("IsParried", false);
                yield break;
            }
            Status = EnemyStatus.Parried;
            yield return new WaitForSeconds(ParryTime / 2);
            currSM.SetBool("IsParried", false);
            Status = EnemyStatus.Passive;
        }

        void ResetAI() => ActiveAI = true;
        #endregion

        public void StopAI(float seconds)
        {
            ActiveAI = false;
            Invoke(nameof(ResetAI), seconds);
        }
        public void SetStatus(string statusName)
        {
            EnemyStatus tmp;
            if (Enum.TryParse<EnemyStatus>(statusName, out tmp))
            {
                Status = tmp;
            }
        }
        IEnumerator NextStage()
        {
            if (Stage + 1 >= Stages.Count) yield break;
            currSM.SetBool("SwitchingStage", true);
            Status = EnemyStatus.Untouchable;
            yield return new WaitForSeconds(3f);
            Status = EnemyStatus.Passive;
            Stage++;
            currSM = Stages[Stage];
            currSM.Start();
        }

        #region Rewinding
        public float RewindTimeNeeded()
        {
            var entry = timeEntries.Last.Value;
            if ((entry.state.stateName == "JumpAttack" || entry.state.stateName == "SequenceAttack") && entry.stateTime > TimeBetweenEntries.Value)
            {
                return entry.stateTime - TimeBetweenEntries.Value;
            }
            else
            {
                return 0f;
            }
        }
        void OnEnable() => IsRewinding.OnValueChange += HandleRewind;
        void OnDisable() => IsRewinding.OnValueChange -= HandleRewind;
        private void HandleRewind()
        {
            if (IsRewinding.Value)
            {
                navAgent.enabled = false;
                transform.DOKill();
                StopAllCoroutines();
                StartCoroutine(Cycle());
            }
            else
            {
                navAgent.enabled = true;
                var entry = timeEntries.Last.Value;
                Health = entry.hp;
                if (Health > MaxHealth - Stage * (MaxHealth / Stages.Count))
                {
                    Stage--;
                    currSM = Stages[Stage];
                }
                currSM.SetCurrentState(entry.state, this);
                for(int i = 0; i < entry.parameters.Length; i++)
                {
                    if (entry.parameters[i] is float) (currSM.parameters[i] as FloatParameter).value = (float)entry.parameters[i];
                    else if (entry.parameters[i] is int) (currSM.parameters[i] as IntParameter).value = (int)entry.parameters[i];
                    else (currSM.parameters[i] as BoolParameter).value = (bool)entry.parameters[i];
                }
                StartCoroutine(UpdateRandomParameters());
                StartCoroutine(CalculatePlayerAvgDeltaPos());
                if (currSM.GetBool("IsParried")) StartCoroutine(ResetParry());
                else if (currSM.GetBool("IsStunned")) StartCoroutine(ResetStun());
                else if (currSM.GetBool("IsAttacking")) StartCoroutine(ResetAttack());
            }
        }
        IEnumerator Cycle()
        {
            YieldInstruction waitBetween = new WaitForSeconds(TimeBetweenEntries.Value);
            while (true)
            {
                if (IsRewinding.Value)
                {
                    if (entries > 1)
                    {
                        timeEntries.RemoveLast();
                        entries--;
                    }
                    yield return waitBetween;
                }
                else
                {
                    timeEntries.AddLast(new TimeEntry(Health, currSM));
                    entries++;
                    if(entries > maxentries)
                    {
                        timeEntries.RemoveFirst();
                        entries--;
                    }
                    yield return waitBetween;
                }
            }
        }

        private struct TimeEntry
        {
            public float hp;
            public SMState state;
            public float stateTime;
            public object[] parameters;
            public TimeEntry(float health, StateMachine machine)
            {
                hp = health;
                state = machine.GetCurrentState();
                stateTime = state.timeInState;
                parameters = new object[machine.parameters.Count];
                for(int i = 0; i < parameters.Length; i++)
                {
                    var tmp = machine.parameters[i];
                    if (tmp is FloatParameter) parameters[i] = (tmp as FloatParameter).value as object;
                    else if (tmp is IntParameter) parameters[i] = (tmp as IntParameter).value as object;
                    else parameters[i] = (tmp as BoolParameter).value as object;
                }
            }
        }
        #endregion

    }
}
