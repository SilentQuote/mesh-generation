using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splittable : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject planeObj;
    Mesh cutterMesh;
    Mesh mesh;

    void Start()
    {
        cutterMesh = planeObj.GetComponent<MeshFilter>().mesh;
        mesh = GetComponent<MeshFilter>().mesh;
        Split();
    }

    public void Split(){
        //Create first part
        GameObject part1 = new GameObject("CutPart1");
        part1.AddComponent<MeshRenderer>();
        part1.AddComponent<MeshFilter>();
        //part1.AddComponent<Rigidbody>();
        part1.AddComponent<MeshCollider>();

        //debug - disable rigid booty
        GetComponent<Rigidbody>().isKinematic = true;
        
        //Create the other
        GameObject part2 = new GameObject("CutPart2");
        part2.AddComponent<MeshRenderer>();
        part2.AddComponent<MeshFilter>();
        //part2.AddComponent<Rigidbody>();
        part2.AddComponent<MeshCollider>();
        
        List<Vector3> verts = new List<Vector3>();
        foreach(Vector3 v in mesh.vertices){
            verts.Add(v);
        }
        //Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;
        Plane plane = new Plane(
            transform.InverseTransformPoint(planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[0]])),
            transform.InverseTransformPoint(planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[1]])),
            transform.InverseTransformPoint(planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[2]]))
            );
        //StartCoroutine(printPlane(verts));
        List<int> fullPosTris = new List<int>();
        List<int> fullNegTris = new List<int>();
        List<int> splitTris = new List<int>();

        List<Vector3> posVerts = new List<Vector3>();
        List<Vector3> negVerts = new List<Vector3>();
        
        //filter out full triangles
        for(int i = 0; i < mesh.triangles.Length-2; i+=3){
            if((plane.SameSide(verts[tris[i]],verts[tris[i+1]])) && (plane.SameSide(verts[tris[i+1]],verts[tris[i+2]]))){
                //find what side the triangle is on, store it in the positive or negative array.
                if(plane.GetSide(verts[tris[i]])){
                    fullPosTris.Add(tris[i]);
                    fullPosTris.Add(tris[i+1]);
                    fullPosTris.Add(tris[i+2]);
                } else {
                    fullNegTris.Add(tris[i]);
                    fullNegTris.Add(tris[i+1]);
                    fullNegTris.Add(tris[i+2]);
                }
            } else { //if its split, set it aside.
                splitTris.Add(tris[i]);
                splitTris.Add(tris[i+1]);
                splitTris.Add(tris[i+2]);
            }
        }

        //update the new vert lists for both objects
        //first part - THIS WONT WORK! this will make duplicate verts!!!
        /*foreach(int i in fullPosTris){
            posVerts.Add(verts[i]);
        }*/
        //instead just add the new verts on top lol

        //create new mid points for split triangles
        //re-find points that are on opposite ends. TODO: room for optimization??? Maybe do at the same time as above method?
        if(splitTris.Count >= 3){ //only do this if there are triangle(s) in the list. 
            for(int i = 0; i < splitTris.Count-2; i+=3){
                //check the lines of the triangle
                /* NOTE
                There are 3 if statements, back to back. The idea is that
                since a slice can only pierce 2 lines of a single triangle,
                only 2 of these will run. Possible room for optimization since
                this guarantees an if statment is run for no reason. 
                */
                Vector3[] newPts = new Vector3[2];
                int[] newTPts = new int[2];
                int[] shrSide = new int[2];
                int lonePnt = i; //idk what to set it to, was going to make it null but it might crash. 
                int ind = 0;

                
                //first line
                if(!plane.SameSide(verts[splitTris[i]],verts[splitTris[i+1]])){
                    //create the new vert
                    newPts[ind] = MakePoint(verts, splitTris, i, plane);
                    ind++;
                } else {
                    //record which two triangle points are on the same side.
                    shrSide[0] = splitTris[i];
                    shrSide[1] = splitTris[i+1];
                    lonePnt = i+2;
                }
                //second line
                if(!plane.SameSide(verts[splitTris[i+1]],verts[splitTris[i+2]])){
                    newPts[ind] = MakePoint(verts, splitTris, i, plane);
                    ind++;
                } else {
                    shrSide[0] = splitTris[i+1];
                    shrSide[1] = splitTris[i+2];
                    lonePnt = i;
                }
                //third line
                if(!plane.SameSide(verts[splitTris[i+2]],verts[splitTris[i]])){
                    newPts[ind] = MakePoint(verts, splitTris, i, plane);
                    ind++;
                } else {
                    shrSide[0] = splitTris[i+2];
                    shrSide[1] = splitTris[i];
                    lonePnt = i+1;
                }
                //update new vertices
                verts.Add(newPts[0]);
                verts.Add(newPts[1]);
                
                //first deal with the two tris on the bigger side, then finish off the last. 
                //can possibly do at the same time as the part above??? right now this is only for organization?
                //check if the current triangle point is in the bigger side
                int[] newTris;
                newTris = new int[]{
                    shrSide[1],
                    shrSide[0],
                    verts.Count-2, //the index of the first found vertex | Count-1 is the final item, so the second vertex. confusing i know.
                    shrSide[1],
                    verts.Count-2,
                    verts.Count-1
                };
                //Pick which side to put the double tris on.
                if(plane.GetSide(verts[shrSide[0]])){
                    foreach(int pnt in newTris){
                        fullPosTris.Add(pnt);
                    }
                } else {
                    foreach(int pnt in newTris){
                        fullNegTris.Add(pnt);
                    }
                }
                //make the small triangle, unfortunately depending on side
                newTris = new int[]{
                    //use ternary operators to ensure clockwise order. remember, count-1 is the second vertex and count-2 is the first.
                    plane.GetSide(verts[lonePnt]) ? verts.Count-1 : verts.Count-2, 
                    plane.GetSide(verts[lonePnt]) ? verts.Count-2 : verts.Count-1,
                    lonePnt
                };
                //add them to the right list. 
                if(plane.GetSide(verts[lonePnt])){
                    foreach(int pnt in newTris){
                        fullPosTris.Add(pnt);
                    }
                } else {
                    foreach(int pnt in newTris){
                        fullNegTris.Add(pnt);
                    }
                }
            } // end of for loop, all slices should be created by now, and triangles set aside.
            /*now heres where the two parts are finally pieced together. The problem, however, is that
            currently i store the verticies in BOTH MODELS! THIS SUCKS!!! this means that data for essentially
            two models are stored on a single slice! try to find a way to get rid of unused vert data!!! PLEASE!!!!*/
            part1.GetComponent<MeshFilter>().mesh.vertices = verts.ToArray();
            part1.GetComponent<MeshFilter>().mesh.triangles = fullPosTris.ToArray();
            part1.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            //sloppy uvs PLEASE FIX
            //part1.GetComponent<MeshFilter>().mesh.uv = GetComponent<MeshFilter>().mesh.uv;
            part1.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            part2.GetComponent<MeshFilter>().mesh.vertices = verts.ToArray();
            part2.GetComponent<MeshFilter>().mesh.triangles = fullNegTris.ToArray();
            part2.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            //part2.GetComponent<MeshFilter>().mesh.uv = GetComponent<MeshFilter>().mesh.uv;
            part2.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        }
    }

    Vector3 MakePoint(List<Vector3> verts, List<int> tris, int i, Plane plane){
        Ray ray = new Ray(verts[tris[i]],verts[tris[i+1]] - verts[tris[i]]);
        float enter; 
        plane.Raycast(ray, out enter);
        return ray.GetPoint(enter);
    }


    //debug print routine
    IEnumerator printPlane(Vector3[] verts){
        while(true) {
            Debug.Log(
                gameObject.transform.InverseTransformPoint(planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[0]])) + " " + 
                gameObject.transform.InverseTransformPoint(planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[1]])) + " " + 
                gameObject.transform.InverseTransformPoint(planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[2]])) + "\nbefore conv:   " +
                planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[0]]) + " " + 
                planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[1]]) + " " + 
                planeObj.transform.TransformPoint(cutterMesh.vertices[cutterMesh.triangles[2]]) + " and we're at " +
                verts[0]
                /*cutterMesh.vertices[cutterMesh.triangles[0]] + " " + 
                cutterMesh.vertices[cutterMesh.triangles[1]] + " " + 
                cutterMesh.vertices[cutterMesh.triangles[2]] + " and we're at " +
                verts[0]*/
                );
            yield return new WaitForSeconds(1f);
        }
    }

}
