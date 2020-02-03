using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
[UnityEngine.RequireComponent(typeof(AudioSource))]
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class fireGun : MonoBehaviour
{
    private GameObject lightGameObject;
    public virtual IEnumerator Go()
    {
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.1f);
        this.transform.Translate(Vector3.forward * -0.1f);
        this.lightGameObject = new GameObject("gunshot");
        this.lightGameObject.transform.localPosition = this.transform.position + this.transform.forward;
        this.lightGameObject.AddComponent(typeof(Light));
        this.lightGameObject.GetComponent<Light>().intensity = 50;
        this.lightGameObject.GetComponent<Light>().range = 50;
        this.lightGameObject.GetComponent<Light>().type = LightType.Point;
        yield return new WaitForSeconds(0.1f);
        this.lightGameObject.transform.localPosition = this.transform.position + (this.transform.forward * 5);
        this.lightGameObject.GetComponent<Light>().intensity = 25;
        this.lightGameObject.GetComponent<Light>().range = 25;
        yield return new WaitForSeconds(0.1f);
        UnityEngine.Object.Destroy(this.lightGameObject);
        this.transform.Translate(Vector3.forward * 0.1f);
    }

}