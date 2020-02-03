using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class attractAvoid : MonoBehaviour
{
    public float randomAvoid;
    public float randomAttract;
    public float moveSpeed;
    private float currentMoveSpeed;
    public float nearHeight;
    public float farHeight;
    public bool attractState; // true means attract, false means avoid
    public float nearThreshold;
    public float farThreshold;
    private float attractAmount;
    private Vector3 nextPoint;
    private float attractOffset;
    private GameObject mainCam;
    public virtual void Start()
    {
        this.mainCam = GameObject.FindWithTag("MainCamera");
        this.nextPoint = this.mainCam.transform.position;
        this.currentMoveSpeed = this.moveSpeed + Random.Range(this.moveSpeed, this.moveSpeed * 2);
    }

    public virtual void Update()
    {
        int drctn = 0;
        // update position
        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, this.nextPoint, this.attractAmount);
        // attract
        if (this.attractState)
        {
            this.nextPoint = this.mainCam.transform.position + new Vector3(0, Random.Range(this.nearHeight, this.nearHeight * 2), 0);
            this.attractAmount = this.attractAmount + (this.currentMoveSpeed / 20000);
            // switch to avoid
            if (((Vector3.Distance(this.gameObject.transform.position, this.nextPoint) < 0.1f) || (Vector3.Distance(this.gameObject.transform.position, this.mainCam.transform.position) < this.nearThreshold)) || (this.attractAmount > 1f))
            {
                this.attractState = false;
                this.attractAmount = 0;
                //currentMoveSpeed = moveSpeed+Random.Range(moveSpeed,moveSpeed*2);
                int dice = Random.Range(-1, 1);
                if (dice < 0)
                {
                    drctn = -1;
                }
                else
                {
                    drctn = 1;
                }
                Ray ray = new Ray(this.mainCam.transform.position, this.transform.position);
                this.nextPoint = ray.GetPoint((this.farThreshold * 3) * drctn);
                this.nextPoint = new Vector3(this.nextPoint.x + Random.Range(-this.randomAvoid, this.randomAvoid), Random.Range(-this.farHeight, this.farHeight), this.nextPoint.z + Random.Range(-this.randomAvoid, this.randomAvoid));
            }
        }
        else
        {
            // avoid
            this.attractAmount = this.attractAmount + (this.currentMoveSpeed / 10000);
            // switch to attract
            if ((Vector3.Distance(this.gameObject.transform.position, this.mainCam.transform.position) > this.farThreshold) || (this.attractAmount > 1f))
            {
                this.attractState = true;
                this.attractAmount = 0;
                this.currentMoveSpeed = this.moveSpeed + Random.Range(this.moveSpeed, this.moveSpeed * 2);
            }
        }
    }

    public attractAvoid()
    {
        this.randomAvoid = 10f;
        this.randomAttract = 5f;
        this.moveSpeed = 1f;
        this.nearHeight = 2;
        this.farHeight = 1;
        this.attractState = true;
        this.nearThreshold = 10f;
        this.farThreshold = 20f;
    }

}