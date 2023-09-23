using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Add this component to a game object containing the children things you want to move apart
 * Click on one of the children, space is then created around it
 */
struct Layer
{
    public Transform trans;
    public Vector3 originalLocation;

    public Layer(Transform trans, Vector3 originalLocation)
    {
        this.trans = trans;
        this.originalLocation = originalLocation;
    }
}

public class LayerSpacer : MonoBehaviour {
    //private int numberOfLayers;
    public Camera layerCamera; //Camera that is clickable
    public float spaceBetween = 10f;
    public float movingSpeed = 1f;

    private List<Layer> listLayers;
    private Transform clickedLayer;
    private int count;

    // Use this for initialization
    void Start()
    {
        count = 0;
        listLayers = new List<Layer>();
        UpdateChildren();
        //PrintList();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("q")) //Reset
        {
            clickedLayer = listLayers[0].trans;
        }
        if (Input.GetMouseButtonDown(0)) //Get the transform of the clicked layer
        {
            count++;
            RaycastHit hit;
            Vector3 mousePos = Input.mousePosition;
            Ray ray = layerCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out hit))
            {
                clickedLayer = hit.transform;
                //Debug.Log(count + ":" + clickedLayer);
                //SetOriginalPosistion();
            }
        }
        if (clickedLayer)//If layer is chosen move all the layers above it
        {
            //Debug.Log(clickedLayer);
            MoveLayers(clickedLayer);
        }
        //else //Else reset the position of every layer
        //{
        //    SetOriginalPosistion();
        //}

    }

    void MoveLayers(Transform clickedLayer) //Clicked transform, int is 1 if you want it to move up or -1 if you want to move down
    {
        bool above = true;
        for (int i = 0; i < listLayers.Count; i++)
        {
            if (listLayers[i].trans != clickedLayer && above)
            {
                //Debug.Log("Name:" + listLayers[i].trans.name + ":" + listLayers[i].trans.localPosition + ":" + listLayers[i].originalLocation);
                Vector3 spaceBetweenVector = new Vector3(0.0f, spaceBetween, 0.0f);
                Vector3 newLocation = listLayers[i].originalLocation + spaceBetweenVector;
                //Debug.Log("Current:" + listLayers[i].trans.localPosition + ":Target:" + newLocation);
                listLayers[i].trans.localPosition = Vector3.MoveTowards(listLayers[i].trans.localPosition, newLocation, movingSpeed);
            }
            else if(listLayers[i].trans == clickedLayer)
            {
                above = false;
            }
            if (!above)
            {
                //Debug.Log("Name:" + listLayers[i].trans.name + ":" + listLayers[i].trans.localPosition + ":" + listLayers[i].originalLocation);
                Vector3 spaceBetweenVector = new Vector3(0.0f, spaceBetween, 0.0f);
                Vector3 newLocation = listLayers[i].originalLocation + spaceBetweenVector;
                //Debug.Log("Current:" + listLayers[i].trans.localPosition + ":Target:" + newLocation);
                listLayers[i].trans.localPosition = Vector3.MoveTowards(listLayers[i].trans.localPosition, listLayers[i].originalLocation, movingSpeed);
            }
        }
    }

    void SetOriginalPosistion()
    {
        //Debug.Log("Starting SetOriginalPosistion()");
        while (true)
        {
            for (int i = 0; i < listLayers.Count; i++)
            {
                //Debug.Log(listLayers[i].trans.name + ":Localpos:" + listLayers[i].trans.localPosition + ":orgPos:" + listLayers[i].originalLocation);
                listLayers[i].trans.localPosition = Vector3.MoveTowards(listLayers[i].trans.localPosition, listLayers[i].originalLocation, movingSpeed);
            }
            break;
        }
    }

    void UpdateChildren() //Set all children objects in a list
    {
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in ts)
        {
            //Debug.Log("Child:" + child + "GameObject:" + gameObject.transform);
            if (child.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                Layer temp = new Layer(child, child.localPosition);
                listLayers.Add(temp);
            }
            else
            {
                //Debug.Log("Parent obj");
            }
        }
        listLayers.Sort(SortByYvalue);
    }

    static int SortByYvalue(Layer a, Layer b) //Sorts the list by the y axis
    {
        return b.trans.position.y.CompareTo(a.trans.position.y);
    }

    void PrintList() //Print out the list
    {
        for (int i = 0; i < listLayers.Count; i++)
        {
            Debug.Log(listLayers[i].trans.name);
        }
    }
}
