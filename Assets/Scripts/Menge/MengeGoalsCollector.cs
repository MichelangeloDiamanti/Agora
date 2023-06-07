using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MengeGoalsCollector : MonoBehaviour
{
    public string goalsName = "Goals";
    public List<GameObject> goals;

    // Start is called before the first frame update
    void Start()
    {
        FindGoals();
    }

    void FindGoals()
    {
        goals = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        int id = 1;
        string goalsString = "";

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == goalsName)
            {
                goals.Add(obj);
                Vector3 position = obj.transform.position;
                string goalString = $"\t\t<Goal type=\"point\" id=\"{id}\" x=\"{position.x}\" y=\"{position.z}\" capacity=\"1000\"/>";
                goalsString += goalString + "\n";
                id++;
            }
        }

        Debug.Log(goalsString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
