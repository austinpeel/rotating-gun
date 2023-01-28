using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingGunSlideController : Slides.SimulationSlideController
{
    public float angularFrequency;
    public RotatingGunSimulation.ReferenceFrame referenceFrame;

    private RotatingGunSimulation sim;
    private bool hasInitialized;

    private Vector3 cameraPosition;
    private Quaternion cameraRotation;

    public override void InitializeSlide()
    {
        sim = simulation as RotatingGunSimulation;

        sim.angularFrequency = angularFrequency;
        sim.referenceFrame = referenceFrame;
        sim.Pause();

        hasInitialized = true;
    }

    private void OnEnable()
    {
        Slides.CameraController.OnCameraFinishTransition += HandleCameraTransitionComplete;
    }

    private void OnDisable()
    {
        Slides.CameraController.OnCameraFinishTransition -= HandleCameraTransitionComplete;

        // Reset the positions and orientations of the camera and simulation
        if (hasInitialized) sim.Reset(cameraPosition, cameraRotation);
    }

    public void HandleCameraTransitionComplete(Vector3 cameraPosition, Quaternion cameraRotation)
    {
        if (enabled)
        {
            this.cameraPosition = cameraPosition;
            this.cameraRotation = cameraRotation;
            sim.Resume();
        }
    }
}
