using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class slowRotate : MonoBehaviour
{
    public float rotateSpeed;
    public bool randomize;
    public Vector3 rotateAxis;
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
        this.transform.Rotate(new Vector3((Time.deltaTime * this.rotateSpeed) * this.rotateAxis.x, (Time.deltaTime * this.rotateSpeed) * this.rotateAxis.y, (Time.deltaTime * this.rotateSpeed) * this.rotateAxis.z));
        if (this.randomize)
        {
            this.rotateAxis = this.rotateAxis + new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
        }
    }

    public slowRotate()
    {
        this.rotateSpeed = 1f;
        this.rotateAxis = new Vector3(0.5f, 0.5f, 0.5f);
    }

}