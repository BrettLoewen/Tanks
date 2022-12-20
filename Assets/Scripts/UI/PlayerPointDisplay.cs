using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPointDisplay: MonoBehaviour
{
    #region Variables

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Slider pointDisplaySlider;
    [SerializeField] private Image pointDisplayFill;

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
        
    }//end Update

    #endregion //end Unity Control Methods

    #region

    //
    public void Setup(Player player, int maxPoints)
    {
        playerNameText.text = player.coloredPlayerText;
        pointDisplaySlider.maxValue = maxPoints;
        pointDisplaySlider.value = player.wins;
        pointDisplayFill.color = player.playerColor;
    }

    //
    public IEnumerator IncreasePoints(AnimationCurve curve)
    {
        float pointIncrease = 0f;
        float prevPoints = pointDisplaySlider.value - 1f;
        float stepThroughCurve = 0f;

        while(stepThroughCurve <= 1f)
        {
            pointIncrease = curve.Evaluate(stepThroughCurve);

            pointDisplaySlider.value = prevPoints + pointIncrease;

            yield return new WaitForSeconds(0.025f);

            stepThroughCurve += 0.1f;
        }
    }

    #endregion
}