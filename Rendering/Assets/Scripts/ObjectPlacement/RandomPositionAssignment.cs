using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPositionAssignment : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] objects; 

    public enum FixedCoord { X,Y,Z };
    public FixedCoord fixedCoord = FixedCoord.Y; 
    private Vector3 fixedCoordVec;
    private Vector3 fixedRotation; 
    public bool randomRotationX = false;
    public bool randomRotationY = true;
    public bool randomRotationZ = false;

    public Plane plane;

    public Transform[] poses;


    private Vector3[] positions;

    private int[] posIndices; 
    public int switchDelayFrames = 1;
    private int numFramesSinceLastSwitch = 0; 

    void Start()
    {
        positions = new Vector3[poses.Length]; 
        for(int i = 0; i< poses.Length;++i)
        {
            positions[i] = poses[i].position;
        }


        Debug.Assert(objects.Length <= positions.Length); 
        switch(fixedCoord)
        {
            case FixedCoord.X:
                {
                    fixedCoordVec = Vector3.right;
                    break;
                }
            case FixedCoord.Y:
                {
                    fixedCoordVec = Vector3.up;
                    break;
                }
            case FixedCoord.Z:
                {
                    fixedCoordVec = Vector3.forward;
                    break;
                }
        }
        fixedRotation = new Vector3(randomRotationX ? 1 : 0, randomRotationY ? 1 : 0, randomRotationZ ? 1 : 0);

        posIndices = new int[positions.Length];
        for (int i = 0; i < positions.Length; ++i)
        {
            posIndices[i] = i;
        }

    }


    public void PreRender()
    {
        if (numFramesSinceLastSwitch-- == 0)
            numFramesSinceLastSwitch = switchDelayFrames;    
        else
            return; 



        //shuffle
        for (int t = 0; t < posIndices.Length; t++)
        {
            int tmp = posIndices[t];
            int r = Random.Range(t, posIndices.Length);
            posIndices[t] = posIndices[r];
            posIndices[r] = tmp;
        }
        string roll = ""; 
        for(int i = 0; i < objects.Length; ++i)
        {
            int idx = posIndices[i];
            roll += idx + " "; 
            Vector3 pos = objects[i].transform.position;
            pos.Scale(fixedCoordVec);
            Vector3 newPos = positions[idx]; 
            newPos.Scale(Vector3.one - fixedCoordVec);

            objects[i].transform.position = pos + newPos; 
            objects[i].transform.rotation = Quaternion.Euler(objects[i].transform.rotation.eulerAngles + Vector3.Scale(Random.rotation.eulerAngles, fixedRotation)); 
           
        }
        //Debug.Log(roll); 
    }

}
