using UnityEngine;

public class ExampleSimulationSlideController : Slides.SimulationSlideController
{
    private ExampleSimulation sim;

    [SerializeField] private bool isRotating;
    [SerializeField] private bool isTranslating;

    public override void InitializeSlide()
    {
        sim = simulation as ExampleSimulation;

        sim.isRotating = isRotating;
        sim.isTranslating = isTranslating;
    }
}
