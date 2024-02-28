// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

public class AnimatedTheta : MonoBehaviour
{
    public SimulationState simState;
    public Image circleImage;
    public Image thetaLabel;
    public float thetaRadius = 30;

    private SimulationState.ReferenceFrame currentReferenceFrame;

    private void LateUpdate()
    {
        if (!simState) return;

        if (circleImage)
        {
            float fillAmount = simState.theta / 360f;
            if (simState.referenceFrame == SimulationState.ReferenceFrame.Gun)
            {
                fillAmount = 1 - fillAmount;
            }
            circleImage.fillAmount = fillAmount;
        }

        if (thetaLabel)
        {
            float angle = simState.theta * Mathf.Deg2Rad;
            if (simState.referenceFrame == SimulationState.ReferenceFrame.Gun)
            {
                angle = 2 * Mathf.PI - angle;
            }
            angle *= 0.5f;
            Vector2 direction = Mathf.Cos(angle) * Vector2.right + Mathf.Sin(angle) * Vector2.up;
            thetaLabel.rectTransform.anchoredPosition = thetaRadius * direction;
        }
    }
}
