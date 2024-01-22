using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Animator animator;
    private bool doorOpened;

    private void Start()
    {
        animator = GetComponent<Animator>();

        doorOpened = false;
    }

    public void OnInteract()
    {
        if (doorOpened == false)
        {
            doorOpened = true;
            animator.SetBool("OpenDoor", true);
            StartCoroutine(CloseDoor());
        }
    }

    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(2f);
        animator.SetBool("OpenDoor", false);
        yield return new WaitForSeconds(1f);
        doorOpened = false;
    }
}
