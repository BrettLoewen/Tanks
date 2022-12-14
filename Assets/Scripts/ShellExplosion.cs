using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    #region Variables

    [SerializeField] private LayerMask tankMask;                //The layer on which tanks can be found
    [SerializeField] private ParticleSystem explosionParticles; //The particle system used to play the explosion effect
    [SerializeField] private AudioSource explosionAudio;        //The audio source used to play the explosion sound effect
    [SerializeField] private float damage = 100f;               //The damage the explosion will do to tanks
    [SerializeField] private float explosionForce = 1000f;      //The knockback force that will be applied to damaged tanks
    [SerializeField] private float maxLifeTime = 2f;            //After this time, the shell will be destroyed
    [SerializeField] private float explosionRadius = 5f;        //The radius of the damaging explosion

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        
    }//end Awake

    // Start is called before the first frame update
    void Start()
    {
        //Ensure the shell is destroyed if it does not hit anything
        Destroy(gameObject, maxLifeTime);
    }//end Start

    // Update is called once per frame
    void Update()
    {
        
    }//end Update

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// The collider entered something, so it hit an object and needs to explode. Find all nearby tanks that could be damaged and damage them
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //Get all the possible targets in the explosion area
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, tankMask);

        //Loop through all of the possible targets
        for (int i = 0; i < colliders.Length; i++)
        {
            //Get the target's rigidbody
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            //If the target does not have a rigidbody, skip it
            if(targetRigidbody == null)
            {
                continue;
            }

            //Add an explosion (knockback) force to the target
            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            //Get the target's health script
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            //If the target does not have a TankHealth component, skip it
            if(targetHealth == null)
            {
                continue;
            }

            //Inflict damage on the target
            targetHealth.TakeDamage(damage);
        }

        //Separate the effects from this object because this object will be destroyed
        explosionParticles.transform.parent = null;

        //Play the explosion effect
        explosionParticles.Play();
        explosionAudio.Play();

        //Destroy the effect when it is done playing and destroy the shell now
        Destroy(explosionParticles.gameObject, explosionParticles.main.duration);
        Destroy(gameObject);
    }//end OnTriggerEnter

    #endregion
}
