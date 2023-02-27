using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheoryBasisVectors : MonoBehaviour
{
    public GameObject labBasisVectors;
    public GameObject gunBasisVectors;
    public SimulationState simState;

    public void SetQuantitiesTabVisibility(bool tabIsActive)
    {
        if (!simState) return;

        if (tabIsActive)
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
}
