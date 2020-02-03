using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class spinObject : MonoBehaviour
{
    public float spinSpeed;
    public bool counterClockwise;
    public virtual void Update()
    {
        if (!this.counterClockwise)
        {
            this.transform.Rotate(new Vector3(0, Time.deltaTime * this.spinSpeed, 0));
        }
        else
        {
            this.transform.Rotate(new Vector3(0, Time.deltaTime * -this.spinSpeed, 0));
        }
    }

    public spinObject()
    {
        this.spinSpeed = 10f;
    }

}