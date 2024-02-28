// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

public class TheoryBasisVectors : MonoBehaviour
{
    public GameObject labBasisVectors;
    public GameObject gunBasisVectors;
    public SimulationState simState;

    private bool dynamicsTabIsActive;

    private void Awake()
    {
        HideAll();
    }

    public void SetCoordinatesTabVisibility(bool tabIsActive)
    {
        if (tabIsActive)
        {
            ShowAll();
        }
        else
        {
            HideAll();
        }
    }

    public void SetDynamicsTabVisibility(bool tabIsActive)
    {
        HideAll();

        if (simState && tabIsActive)
        {
            switch (simState.referenceFrame)
            {
                case SimulationState.ReferenceFrame.Lab:
                    SetLabBasisVisibility(true);
                    SetGunBasisVisibility(false);
                    break;
                case SimulationState.ReferenceFrame.Gun:
                    SetLabBasisVisibility(false);
                    SetGunBasisVisibility(true);
                    break;
                default:
                    break;
            }
        }

        dynamicsTabIsActive = tabIsActive;
    }

    public void UpdateReferenceFrame(bool frameIsLab)
    {
        if (simState && dynamicsTabIsActive)
        {
            switch (simState.referenceFrame)
            {
                case SimulationState.ReferenceFrame.Lab:
                    SetLabBasisVisibility(true);
                    SetGunBasisVisibility(false);
                    break;
                case SimulationState.ReferenceFrame.Gun:
                    SetLabBasisVisibility(false);
                    SetGunBasisVisibility(true);
                    break;
                default:
                    break;
            }
        }
    }

    public void SetLabBasisVisibility(bool isVisible)
    {
        if (labBasisVectors) labBasisVectors.SetActive(isVisible);
    }

    public void SetGunBasisVisibility(bool isVisible)
    {
        if (gunBasisVectors) gunBasisVectors.SetActive(isVisible);
    }

    private void HideAll()
    {
        SetLabBasisVisibility(false);
        SetGunBasisVisibility(false);
    }

    private void ShowAll()
    {
        SetLabBasisVisibility(true);
        SetGunBasisVisibility(true);
    }
}
