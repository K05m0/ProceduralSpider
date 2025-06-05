using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class bodyController : MonoBehaviour
{
    public GameObject[] legTargets;
    public GameObject[] legCubes;

    public GameObject spider;

    public float MaxDistance = 2.5f;
    public int legMovementSmoothness = 5;
    public int bodySmoothness = 5;
    public int velocitySmoothness = 3;

    public float overStepMultiplier = 1.3f;
    public int waitTime = 2;
    public float spiderJitterCutoff = 0.1f;
    public float stepHeight = 0.5f;
    bool currentLeg = true;
    Vector3 lastBodyUp;
    Vector3[] legPositions;
    Vector3[] legOriginalPositions;

    Vector3 velocity;
    Vector3 lastSpiderPosition;
    Vector3 lastVelocity;

    List<int> oppositeLegIndex = new List<int>();
    List<int> nextIndexToMove = new List<int>();
    List<int> indexMoving = new List<int>();



    void Start()
    {
        lastSpiderPosition = spider.transform.position;

        legPositions = new Vector3[legTargets.Length];
        legOriginalPositions = new Vector3[legTargets.Length];

        for (int i = 0; i < legTargets.Length; i++)
        {

            legPositions[i] = legTargets[i].transform.position;
            legOriginalPositions[i] = legPositions[i];

            if (currentLeg)
            {
                oppositeLegIndex.Add(i + 1);
                currentLeg = false;
            }
            else
            {
                oppositeLegIndex.Add(i - 1);
                currentLeg = true;
            }
        }

        lastBodyUp = transform.up;
    }


    void FixedUpdate()
    {
        velocity = spider.transform.position - lastSpiderPosition;
        velocity = velocity + velocitySmoothness * lastVelocity;
        velocity = velocity / (velocitySmoothness + 1);
        moveLegs();
        rotateBody();
        lastSpiderPosition = spider.transform.position;
        lastVelocity = velocity;
    }


    void moveLegs()
    {
        for (int i = 0; i < legTargets.Length; i++)
        {
            if (Vector3.Distance(legTargets[i].transform.position, legCubes[i].transform.position) >= MaxDistance)
            {
                if (!nextIndexToMove.Contains(i) && !indexMoving.Contains(i)) nextIndexToMove.Add(i);
            }
            else if (!indexMoving.Contains(i))
            {
                legTargets[i].transform.position = legOriginalPositions[i];
            }

        }

        if (nextIndexToMove.Count == 0 || indexMoving.Count != 0) return;

        Vector3 targetposition = legCubes[nextIndexToMove[0]].transform.position;
        targetposition = targetposition + Mathf.Clamp(velocity.magnitude * overStepMultiplier, 0, 2) * (legCubes[nextIndexToMove[0]].transform.position - legTargets[nextIndexToMove[0]].transform.position) + velocity * overStepMultiplier;


        StartCoroutine(step(nextIndexToMove[0], targetposition, false));

    }


    IEnumerator step(int index, Vector3 moveTo, bool isOpposite)
    {
        if (!isOpposite)
        {
            moveOppositeLeg(oppositeLegIndex[index]);
        }

        if (nextIndexToMove.Contains(index)) nextIndexToMove.Remove(index);
        if (!indexMoving.Contains(index)) indexMoving.Add(index);

        Vector3 startingPosition = legOriginalPositions[index];

        for (int i = 1; i <= legMovementSmoothness; i++)
        {

            legTargets[index].transform.position = Vector3.Lerp(startingPosition, moveTo + new Vector3(0, Mathf.Sign(i / (legMovementSmoothness + spiderJitterCutoff) * Mathf.PI) * stepHeight, 0), i / legMovementSmoothness);
            yield return new WaitForFixedUpdate();
        }

        legOriginalPositions[index] = moveTo;

        for (int i = 0; i <= waitTime; i++) yield return new WaitForFixedUpdate();

        if (indexMoving.Contains(index)) indexMoving.Remove(index);
    }

    void moveOppositeLeg(int index)
    {
        Vector3 targetposition = legCubes[index].transform.position;
        targetposition = targetposition + Mathf.Clamp(velocity.magnitude * overStepMultiplier, 0, 2) * (legCubes[index].transform.position - legTargets[index].transform.position) + velocity * overStepMultiplier;
        StartCoroutine(step(index, targetposition, true));
    }

    void rotateBody()
    {
        Vector3 v1 = legTargets[0].transform.position - legTargets[1].transform.position;
        Vector3 v2 = legTargets[2].transform.position - legTargets[3].transform.position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = Vector3.Lerp(lastBodyUp, -normal, 1f / bodySmoothness);
        transform.up = up;
        lastBodyUp = up;
    }




}

