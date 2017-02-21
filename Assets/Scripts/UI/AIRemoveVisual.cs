using UnityEngine;
using System.Collections;

public class AIRemoveVisual : MonoBehaviour {


    public GameObject AI;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(AI.GetComponent<Standard_Enemy>()._state == Base_Enemy.State.Dead)
        {
            Destroy(gameObject);
        }
	}
}
