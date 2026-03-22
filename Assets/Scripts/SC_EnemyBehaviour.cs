using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private GameObject destino;
    [SerializeField] private NavMeshAgent agent;
    
    [Header("Attack")]
    [SerializeField] private Transform hittingPoint;
    [SerializeField] private float hitRadius;
    [SerializeField] private LayerMask whatIsDamagable;
    private Animator anim;
    
    [Header("Audio")]
    [SerializeField] private AudioClip miceShout;
    private AudioSource audioSource;
    
   
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        agent.SetDestination(destino.transform.position);
        
        //Para saber cuando el agent llega a su destino
        if(DestinationReached())
        {
            FaceToTarget();
            
            audioSource.PlayOneShot(miceShout);
            anim.SetBool("shouting", true);
        }

    }

    private void FaceToTarget()
    {
        
        Vector3 targetDirection = (destino.transform.position - transform.position);
        targetDirection.y = 0f;
        Quaternion rotationToTarget = Quaternion.LookRotation(targetDirection);
        transform.rotation = rotationToTarget;
    }

        //Se ejecuta cuando se genera el daño.
        private void DoDamage()
        {
           Physics.OverlapSphere(hittingPoint.position, hitRadius, whatIsDamagable);
           
        }
        
        private bool DestinationReached()
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        }
        
        
        //Se ejecuta cuando acaba la animación
        private void OnHitFinished()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Debug.Log("Se fué!");
                anim.SetBool("shouting", false);
                agent.isStopped = false;
            }
            //1. Calcurlar la distancia a mi objetivo (saber si se ha ido de mi rango 
            //Si es así, pongo la animación a false
        }
}