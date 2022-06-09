using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRay : MonoBehaviour
{
    [SerializeField] GameObject target;

    void Start(){
        Ray ray = new Ray(transform.position, target.transform.position);
        Gizmos.color = Color.red;
        Vector3[] tes = new Vector3[2];
        Debug.Log(tes[0]);
        
    }

    private void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, target.transform.position - transform.position);
        //Debug.Log(target.transform.position - transform.position);
    }
}
