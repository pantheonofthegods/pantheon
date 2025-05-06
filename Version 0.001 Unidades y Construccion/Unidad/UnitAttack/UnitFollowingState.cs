using UnityEngine;
using UnityEngine.AI;

public class UnitFollowingState : StateMachineBehaviour
{
    AttackController attackController;
    NavMeshAgent agent;
    public float attackingDistance = 1f;
    public float optimalAttackDistance = 0.8f; // Distancia ideal para atacar
    public float attackDelay = 0.5f; // Tiempo de preparación antes de atacar
    private float attackTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.GetComponent<AttackController>();
        agent = animator.GetComponent<NavMeshAgent>();
        attackController.SetFollowMaterial();

        // Configuración del agente
        agent.stoppingDistance = optimalAttackDistance; // Se detendrá a esta distancia
        agent.autoBraking = true;
        attackTimer = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
            return;
        }

        if (!animator.GetComponent<UnitMovement>().isCommandToMove)
        {
            // Movimiento hacia el objetivo
            agent.SetDestination(attackController.targetToAttack.position);

            // Rotación hacia el objetivo
            Vector3 direction = (attackController.targetToAttack.position - animator.transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                animator.transform.rotation = Quaternion.Slerp(
                    animator.transform.rotation,
                    lookRotation,
                    Time.deltaTime * 10f
                );
            }

            // Lógica de ataque con tiempo de preparación
            float distance = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

            if (distance <= attackingDistance)
            {
                attackTimer += Time.deltaTime;

                if (attackTimer >= attackDelay)
                {
                    animator.SetBool("isAttacking", true);
                    attackTimer = 0; // Resetear el temporizador
                }
            }
            else
            {
                attackTimer = 0; // Resetear si se aleja
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Resetear valores al salir del estado
        if (agent != null)
        {
            agent.stoppingDistance = 0f;
        }
        attackTimer = 0;
    }
}