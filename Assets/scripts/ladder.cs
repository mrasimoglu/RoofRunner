using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ladder : MonoBehaviour
{
    public int stepCount;
    public bool dir;
    public int currentStep;
    public Vector3[] steps;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Init(Vector3 playerPos)
    {
        if (Vector3.Distance(playerPos, transform.TransformPoint(steps[0])) < Vector3.Distance(playerPos, transform.TransformPoint(steps[stepCount - 1])))
        {
            currentStep = 0;
            return 0;
        }
        else
        {
            currentStep = stepCount - 2;
            return stepCount - 2;
        }
    }

    public bool MoveUp()
    {
        dir = true;
        if (currentStep < stepCount - 2)
            currentStep++;
        else
            return true;
        return false;
    }

    public bool MoveDown()
    {
        dir = false;
        if (currentStep > 0)
            currentStep--;
        else
            return true;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        foreach (var a in steps)
        {
            Gizmos.DrawCube(transform.TransformPoint(a), new Vector3(.3f, .3f, .3f));
        }
    }
}
