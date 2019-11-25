using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 

public class PlaceCameraIndicators : MonoBehaviour
{

    public List<string> files;
    public List<GameObject> indicators; 

    // Start is called before the first frame update
    void Start()
    {
        var indicatorEnum = indicators.GetEnumerator();
        if(!indicatorEnum.MoveNext())
        {
            Debug.LogError("No Camera indicators given"); 
        }
        GameObject indicator = indicatorEnum.Current;



        foreach (var dir in files)
        {
            string filename = dir + "camera_pose.txt"; 

            if(File.Exists(filename)) 
            {
                var lines = File.ReadAllLines(filename); 

                foreach(var line in lines)
                {
                    var coords = line.Split(' ');
                    Vector3 pos = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));

                    Instantiate(indicator, pos, Quaternion.identity); 

                }

            }
            else
            {
                Debug.LogError("Could not open:" + filename); 
            }


            if(indicatorEnum.MoveNext())
            {
                indicator = indicatorEnum.Current; 
            }
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
