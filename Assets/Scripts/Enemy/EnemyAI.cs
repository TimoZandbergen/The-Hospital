using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        public NavMeshAgent agent;

        public Transform player;

        public LayerMask whatIsGround, whatIsPlayer;

        public float health;

        //Patrolling
        public Vector3 walkPoint;
        private bool _walkPointSet;
        public float walkPointRange;

        //Attacking
        public float timeBetweenAttacks;
        private bool _alreadyAttacked;

        //States
        public float sightRange, attackRange;
        public bool playerInSightRange, playerInAttackRange;

        private void Awake()
        {
            player = GameObject.Find("Player").transform;
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            //check for sight or attack range
            var position = transform.position;
            playerInSightRange = Physics.CheckSphere(position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(position, attackRange, whatIsPlayer);

            if (!playerInSightRange && playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        private void Patroling()
        {
            if (!_walkPointSet) SearchWalkPoint();

            if (_walkPointSet)
                agent.SetDestination(walkPoint);

            Vector3 distaneToWalkPoint = transform.position - walkPoint;

            if (distaneToWalkPoint.magnitude < 1f)
                _walkPointSet = false;
        }

        private void SearchWalkPoint()
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            var position = transform.position;
            walkPoint = new Vector3(position.x + randomX, position.y, position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                _walkPointSet = true;
        }

        private void AttackPlayer()
        {
            agent.SetDestination(player.position);
        }

        private void ChasePlayer()
        {
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            if (!_alreadyAttacked)
            {
                /// attack code here
                //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity)
                    //.GetComponent<Rigidbody>();
                //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                //rb.AddForce(transform.up * 8f, ForceMode.Impulse);



                ///
                _alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        private void ResetAttack()
        {
            _alreadyAttacked = false;
        }

        private void TakeDamage(int damage)
        {
            health -= damage;
            
            if(health <= 0) Invoke(nameof(DestroyEnemy), .5f);
        }

        private void DestroyEnemy()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, sightRange);
        }
    }
}
