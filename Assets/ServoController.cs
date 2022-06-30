using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoController : MonoBehaviour
{
    public Vector3 distancia;

    void Update()
    {
        distancia.x = distanciaEntreVectores(this.transform.right, Vector3.right);
        distancia.z = distanciaEntreVectores(this.transform.forward, Vector3.forward);

        /*forwarGLOBAL = Vector3.forward.z;
        forwardLOCAL = this.transform.forward.z;

        upGLOBAL = Vector3.up.y;
        upLOCAL = this.transform.up.y;

        rightGLOBAL = Vector3.right.x;
        rightLOCAL = this.transform.right.x;

        //Debug.Log((Vector3.forward - this.transform.forward).magnitude);//esto funciona??????????????????????? esta raro. tbm lo otro pero lo anterior tiene algo de sentido pero esto no se creo que no

        distancia.z = forwarGLOBAL - forwardLOCAL;
        distancia.x = rightGLOBAL - rightLOCAL;
        distancia.y = upGLOBAL - upLOCAL;*/
    }

    public float distanciaEntreVectores(Vector3 local, Vector3 global) 
    {
        Vector3 resul = local - global;
        if (resul.y < 0)
        {
            //Debug.Log("Distancia: " + -1 * (resul).magnitude);
            return -1 * (resul).magnitude;
        }
        else
        {
            //Debug.Log("Distancia: " + (resul).magnitude);
            return 1 * (resul).magnitude;
        }
        
    }
}
