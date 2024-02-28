// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

public class Activity1Instructions : MonoBehaviour
{
    public InstructionStep[] steps;
    public int currentStepIndex = 0;

    private InstructionStep currentStep;

    public void Start()
    {
        foreach (var step in steps)
        {
            step.Hide();
        }

        LoadStep(currentStepIndex);
    }

    public void LoadStep(int stepIndex)
    {
        if (currentStep != null)
        {
            currentStep.Hide();
            currentStep = null;
        }

        if (stepIndex >= 0 && stepIndex < steps.Length)
        {
            // Debug.Log("Loading step " + (stepIndex + 1));
            currentStep = steps[stepIndex];
            currentStep.Show();
            currentStepIndex = stepIndex;
        }
    }

    public void LoadNextStep()
    {
        LoadStep(currentStepIndex + 1);
    }

    public void EndInstructions()
    {
        foreach (var step in steps)
        {
            step.Hide();
        }
    }
}
