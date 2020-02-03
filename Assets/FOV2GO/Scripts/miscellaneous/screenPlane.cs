using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class screenPlane : MonoBehaviour
{
    //sPlane {near,screen,far}
    public sPlane screenSelect;
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public screenPlane()
    {
        this.screenSelect = sPlane.screen;
    }

}