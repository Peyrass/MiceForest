using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject destino;
    [SerializeField] private NavMeshAgent agent;
    
    [Header("Ataques")]
    [SerializeField] private Transform hittingPoint;
    [SerializeField] private float hitRadius;
    [SerializeField] private LayerMask whatIsDamagable;
    private Animator anim;
    
    
   
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

            Debug.Log("He llegado al destino");
            anim.SetBool("hitting", true);
        }

    }

    private void FaceToTarget()
    {
        //1. Sacar dirección a mi objetivo (Destino - origen)
        Vector3 targetDirection = (destino.transform.position - transform.position);
        //2. Discriminar la "y" de la dirección, osea, ponerla a 0
        targetDirection.y = 0f;
        //3. Se transforma la dirección a una rotación
        Quaternion rotationToTarget = Quaternion.LookRotation(targetDirection);
        //4. Le das la rotación calculada en el paso 3 al transform.rotation del enemigo.
        transform.rotation = rotationToTarget;
    }

        //Se ejecuta cuando se genera el daño.
        private void DoDamage()
        {
           Physics.OverlapSphere(hittingPoint.position, hitRadius, whatIsDamagable);
            //for (COMPLETA)
            Debug.Log("Golpeado!");
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
                anim.SetBool("hitting", false);
                agent.isStopped = false;
            }
            //1. Calcurlar la distancia a mi objetivo (saber si se ha ido de mi rango 
            //Si es así, pongo la animación a false
        }
}