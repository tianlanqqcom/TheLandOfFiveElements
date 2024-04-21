using UnityEngine;
using UnityEngine.AI;

namespace Script.GameFramework.Test
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshTest : MonoBehaviour
    {
        NavMeshAgent agent;
        public Transform target;
        // Start is called before the first frame update
        void Start()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            agent.SetDestination(target.localPosition);
        }
    }
}
