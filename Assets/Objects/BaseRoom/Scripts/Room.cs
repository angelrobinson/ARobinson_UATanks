using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour {

    [Header("Outside Environment")]
    public GameObject doorNorthEast;
    public GameObject doorNorthWest;
    public GameObject doorSouthEast;
    public GameObject doorSouthWest;
    public GameObject doorEast;
	public GameObject doorWest;

    [Header("Patrol")]
    public GameObject[] patrol;


    private Transform trans;

    private void Awake()
    {
        trans = gameObject.transform;
    }

    private void Start()
    {
        if (doorEast == null)
        {
            doorEast =trans.Find("OutsideEnvironment/East_Door").gameObject; ;
        }

        if (doorNorthEast == null)
        {
            doorNorthEast = trans.Find("OutsideEnvironment/NorthEast_Door").gameObject; ;
        }

        if (doorNorthWest == null)
        {
            doorNorthWest = trans.Find("OutsideEnvironment/NorthWest_Door").gameObject; ;
        }

        if (doorSouthEast == null)
        {
            doorSouthEast = trans.Find("OutsideEnvironment/SouthEast_Door").gameObject; ;
        }

        if (doorSouthWest == null)
        {
            doorSouthWest = trans.Find("OutsideEnvironment/SouthWest_Door").gameObject; ;
        }

        if (doorWest == null)
        {
            doorWest = trans.Find("OutsideEnvironment/West_Door").gameObject; ;
        }

        if (patrol.Length == 0)
        {
            patrol = new GameObject[trans.Find("Patrol").transform.childCount];

            for (int i = 0; i < patrol.Length; i++)
            {
                patrol[i] = trans.Find("Patrol").transform.GetChild(i).gameObject;
            }
        }
    }





}
