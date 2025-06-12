using UnityEngine;

public class randomPosTv : MonoBehaviour
{
    [SerializeField] private float posZ;
    [SerializeField] private float posX;
    [SerializeField] private float posY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int LeftOrRight = Random.Range(0, 1);
        if (LeftOrRight == 0) posX = -7.32f;
        else if (LeftOrRight == 1) posX = 3.35f;


       float currentPosZ = transform.position.z; 
             posZ = currentPosZ - Random.Range(250, 320);
                        
        posY = transform.position.y;

        transform.position = new Vector3(posX,posY,posZ);
    }

}
