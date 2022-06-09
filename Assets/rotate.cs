using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(rotat());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator rotat(){
        while(true){
            gameObject.transform.Rotate(0,5f,0,Space.World);
            yield return new WaitForSeconds(.1f);
        }
    }
}
