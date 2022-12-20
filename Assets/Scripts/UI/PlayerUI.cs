using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    #region Variables

    public Player player;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Renderer[] tankPreviewRenderers;

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        
    }//end Awake

    // Start is called before the first frame update
    void Start()
    {
        
    }//end Start

    // Update is called once per frame
    void Update()
    {
        playerNameText.text = player.coloredPlayerText;
        foreach(Renderer renderer in tankPreviewRenderers)
        {
            renderer.material.color = player.playerColor;
        }
    }//end Update

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// Setup the PlayerUI so it references the correct player
    /// </summary>
    /// <param name="_player">The player that this PlayerUI is displaying</param>
    public void SetPlayer(Player _player)
    {
        player = _player;
    }//end SetPlayer

    #endregion
}
