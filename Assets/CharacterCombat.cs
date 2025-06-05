using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // LPM
        {
            animator.SetTrigger("Chainsaw");
        }

        if (Input.GetMouseButtonDown(1)) // PPM
        {
            animator.SetTrigger("Cannon");
        }
    }
}
