using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrbanTerritoriality.MeshToolkit;

namespace UrbanTerritoriality.Experimental
{
    public class HashSetTest : MonoBehaviour
    {
        protected virtual void Start()
        {
            HashSet<Vector3Info> vInfo = new HashSet<Vector3Info>();
            vInfo.Add(new Vector3Info(0, new Vector3(1, 1, 1)));
            vInfo.Add(new Vector3Info(0, new Vector3(1, 1, 1)));
            vInfo.Add(new Vector3Info(1, new Vector3(1, 1, 1)));
            vInfo.Add(new Vector3Info(1, new Vector3(0, 1, 1)));
            vInfo.Add(new Vector3Info(0, new Vector3(0, 1, 1)));
            vInfo.Add(new Vector3Info(3, new Vector3(0, 1, 2)));
            vInfo.Add(new Vector3Info(3, new Vector3(0, 1, 2)));
            vInfo.Add(new Vector3Info(3, new Vector3(0, 1, 5)));
            Debug.Log("vInfo count: " + vInfo.Count);

            Vector2 v1 = new Vector2(1, 2.1f);
            Vector2 v2 = new Vector2(1, 2.1f);
            Vector2 v3 = new Vector2(2, 2.1f);

            if (v1 == v2) Debug.Log("v1 == v2");
            if (v2 == v3) Debug.Log("v2 == v3");

            Vector3 a = new Vector3(1, 2, 3);
            Vector3 b = new Vector3(4, 5, 6);
            a = b;
            a.x = 10;
            b.x = 100;
            Debug.Log("a : " + a);
            Debug.Log("b : " + b);

            EdgeInfo ei1 = new EdgeInfo(0, 2, new Vector3(1, 2, 3), new Vector3(5, 6, 7), 1);
            EdgeInfo ei2 = new EdgeInfo(3, 5, new Vector3(3, 4, 5), new Vector3(10, 2, 2), 2);
            ei1 = ei2;
            ei1.vertice1.x = 20;
            ei2.vertice1.x = 40;
            Debug.Log("ei1.vertice1 : " + ei1.vertice1);
            Debug.Log("ei2.vertice1 : " + ei2.vertice1);
        }
    }
}
