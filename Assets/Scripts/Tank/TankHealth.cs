using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHealth : MonoBehaviour
{
    #region Variables

    [SerializeField] private Player player; //The player that controls this tank

    [SerializeField] private float startingHealth = 1f; //The amount of health the tank starts the game with
    private float currentHealth;                        //The amount of health the tank currently has
    private bool isDead;                                //Stores whether the tank is alive or dead

    [SerializeField] GameObject explosionPrefab;    //The prefab for death effects
    private AudioSource explosionAudio;             //The audio source for the death effect
    private ParticleSystem explosionParticles;      //The particle system for the death effect

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        //Create the explosion and get its components
        explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        explosionAudio = explosionParticles.GetComponent<AudioSource>();

        //Disable the explosion for now
        explosionParticles.gameObject.SetActive(false);
    }//end Awake

    // Start is called before the first frame update
    void Start()
    {
        
    }//end Start

    // OnEnable is called whenever the gameobject is enabled
    private void OnEnable()
    {
        currentHealth = startingHealth;
        isDead = false;
    }//end OnEnable

    // Update is called once per frame
    void Update()
    {
        
    }//end Update

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// Reduce the tank's health by the amount of damage passed and kill the tank if its health is now less than or equal to 0
    /// </summary>
    /// <param name="amount">The amount the tank's health will be decreased by</param>
    public void TakeDamage(float amount)
    {
        //Reduce the tank's health by the amount of damage it took
        currentHealth -= amount;

        //If the tank dies from this damage and is not already dead, kill the tank
        if(currentHealth <= 0f && !isDead)
        {
            OnDeath();
        }
    }//end TakeDamage

    /// <summary>
    /// Store that the tank has died, play the death effects, and disable the tank
    /// </summary>
    private void OnDeath()
    {
        //Store that the tank has died
        isDead = true;

        //Enable the effects and bring them to the right spot
        explosionParticles.transform.position = transform.position;
        explosionParticles.gameObject.SetActive(true);

        //Play the death effects
        explosionParticles.Play();
        explosionAudio.Play();

        //Disable the tank
        gameObject.SetActive(false);
    }//end OnDeath

    /// <summary>
    /// Tell the TankHealth who its player is
    /// </summary>
    /// <param name="_player">The player to store as this TankHealth's player</param>
    public void SetPlayer(Player _player)
    {
        player = _player;
    }//end SetPlayer

    /// <summary>
    /// Return whether or not this TankHealth's player is the same as the passed player
    /// </summary>
    /// <param name="otherPlayer">The player to compare this TankHealth's player against</param>
    /// <returns>Returns true if this TankHealth's player is the same as the passed player, and false otherwise</returns>
    public bool SamePlayer(Player otherPlayer)
    {
        return player.Equals(otherPlayer);
    }//end SamePlayer

    #endregion
}
