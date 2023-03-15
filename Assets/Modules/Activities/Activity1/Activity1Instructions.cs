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
            step.HideAllPanels();
        }

        LoadStep(currentStepIndex);
    }

    public void LoadStep(int stepIndex)
    {
        if (currentStep != null)
        {
            currentStep.HideAllPanels();
            currentStep = null;
        }

        if (stepIndex >= 0 && stepIndex < steps.Length)
        {
            Debug.Log("Loading step index " + stepIndex);
            currentStep = steps[stepIndex];
            currentStep.ShowAllPanels();
            currentStepIndex = stepIndex;
        }
    }

    public void LoadNextStep()
    {
        LoadStep(currentStepIndex + 1);
    }
}
