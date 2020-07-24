using UnityEngine;

public class MovingObjShooter : MonoBehaviour
{
    [SerializeField] MovingObj objPrefab = null;

    public void Shoot()
    {
        MovingObj obj = Instantiate(objPrefab);
        obj.transform.rotation = transform.rotation;
        obj.transform.position = transform.position;
    }
}