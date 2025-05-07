using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class bodyController : MonoBehaviour
{
    public GameObject[] legTargets;
    public GameObject[] legCubes;

    public float MaxDistance = 2.5f;
    public int legMovementSmoothness = 5;

   Vector3[] legPositions;

   Vector3[] legOriginalPositions;

   List<int> nextIndexToMove = new List<int>();
   List<int> indexMoving = new List<int>();
    


    void Start()
    {
        legPositions = new Vector3[legTargets.Length];
        legOriginalPositions = new Vector3[legTargets.Length];

        for(int i = 0; i < legTargets.Length; i++)
        {
            
            legPositions[i] = legTargets[i].transform.position;
            legOriginalPositions[i] = legPositions[i];
        }
    }

   
    void FixedUpdate()
    {
        moveLegs();
    }


    void moveLegs()
    {
        for(int i = 0; i < legTargets.Length; i++) 
        {
            if(Vector3.Distance(legTargets[i].transform.position, legCubes[i].transform.position) >= MaxDistance)
            {
                if(!nextIndexToMove.Contains(i) && !indexMoving.Contains(i)) nextIndexToMove.Add(i);
            }
            else if (!indexMoving.Contains(i)) {
                legTargets[i].transform.position = legOriginalPositions[i];
            }
            
        }

        if(nextIndexToMove.Count == 0 || indexMoving.Count != 0) return;

        Vector3 targetposition = legCubes[nextIndexToMove[0]].transform.position;
        StartCoroutine(step(nextIndexToMove[0], targetposition));

    }

    
    IEnumerator step(int index, Vector3 moveTo) 
    {
        if (nextIndexToMove.Contains(index)) nextIndexToMove.Remove(index);
        if (!indexMoving.Contains(index)) indexMoving.Add(index);

        Vector3 startingPosition = legOriginalPositions[index];

        for(int i = 1; i <= legMovementSmoothness; i++) {

            legTargets[index].transform.position = Vector3.Lerp(startingPosition, moveTo, i / legMovementSmoothness);
            yield return new WaitForFixedUpdate();
        }

        legOriginalPositions[index] = moveTo;

        if(indexMoving.Contains(index)) indexMoving.Remove(index);
    }

}

