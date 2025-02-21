using UnityEditor.Build;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator animator;
    public bool isPushTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isPushTrigger)
            {
                animator.SetBool("inPushZone", true);
            }
            else
            {
                animator.SetBool("inPullZone", true);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isPushTrigger)
            {
                animator.SetBool("inPushZone", false);
            }
            else
            {
                animator.SetBool("inPullZone", false);
            }
        }
    }
}
