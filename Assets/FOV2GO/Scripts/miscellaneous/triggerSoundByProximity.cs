using UnityEngine;
using System.Collections;

[System.Serializable]
/* Simple script to trigger sound by proximity */
[UnityEngine.RequireComponent(typeof(AudioSource))]
public partial class triggerSoundByProximity : MonoBehaviour
{
    public Transform player;
    public int distance;
    private bool playerCloseEnough;
    public virtual void Update()
    {
        if (Vector3.Distance(this.transform.position, this.player.position) < this.distance)
        {
            this.playerCloseEnough = true;
            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            this.playerCloseEnough = false;
            if (this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().Stop();
            }
        }
    }

    public triggerSoundByProximity()
    {
        this.distance = 5;
    }

}