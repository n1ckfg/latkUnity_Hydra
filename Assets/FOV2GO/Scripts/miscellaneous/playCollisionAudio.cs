using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(AudioSource))]
public partial class playCollisionAudio : MonoBehaviour
{
    public AudioClip impactSound;
    public float minimumMagnitude;
    public float pitchMin;
    public float pitchMax;
    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > this.minimumMagnitude)
        {
            this.GetComponent<AudioSource>().volume = collision.relativeVelocity.magnitude / 30;
            this.GetComponent<AudioSource>().pitch = Random.Range(this.pitchMin, this.pitchMax);
            this.GetComponent<AudioSource>().PlayOneShot(this.impactSound);
        }
    }

    public playCollisionAudio()
    {
        this.minimumMagnitude = 2;
        this.pitchMin = 0.25f;
        this.pitchMax = 2f;
    }

}