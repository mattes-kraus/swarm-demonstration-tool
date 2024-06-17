using System;
using UnityEngine;

public class Lightbulb : MonoBehaviour{

    void Start(){
        // adjusting position to be always on the wall
        float diffX = 2 - Math.Abs(gameObject.transform.position.x);
        float diffZ = 2 - Math.Abs(gameObject.transform.position.z);;

        Vector3 pos = gameObject.transform.position;

        if(diffX < diffZ){
            if(gameObject.transform.position.x > 0){
                gameObject.transform.position= new Vector3(2, pos.y, pos.z);
            } else {
                gameObject.transform.position= new Vector3(-2, pos.y, pos.z);
            }
        } else {
            if(gameObject.transform.position.z > 0){
                gameObject.transform.position= new Vector3(pos.x, pos.y, 2);
            } else {
                gameObject.transform.position= new Vector3(pos.x, pos.y, -2);
            }
        }
    }
}