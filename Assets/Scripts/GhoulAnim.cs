using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class GhoulAnim : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    private void Start() {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        anim.SetFloat("speed", agent.velocity.magnitude);
    }


}
