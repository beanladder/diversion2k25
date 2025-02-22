using System;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Ashsvp.InputManager_SVP;

namespace Ashsvp
{
    public class InputManager_SVP : NetworkBehaviour
    {
        [Serializable]
        public class KeyboardInput
        {
            public KeyCode steerLeft = KeyCode.A;
            public KeyCode steerRight = KeyCode.D;
            public KeyCode accelerate = KeyCode.W;
            public KeyCode decelerate = KeyCode.S;
            public KeyCode handBrake = KeyCode.Space;
        }

        public KeyboardInput keyboardInput = new KeyboardInput();

        [Serializable]
        public class MobileInput
        {
            // Steering buttons (Removed because we use a steering wheel now)
            public UiButton_SVP accelerate;
            public UiButton_SVP decelerate;
            public UiButton_SVP handBrake;
        }

        public bool useMobileInput = false;
        public MobileInput mobileInput = new MobileInput();

        // Networked Inputs
        [Networked] public float NetworkSteerInput { get; private set; }
        [Networked] public float NetworkAccelerationInput { get; private set; }
        [Networked] public float NetworkHandbrakeInput { get; private set; }

        public float SteerInput => NetworkSteerInput;
        public float AccelerationInput => NetworkAccelerationInput;
        public float HandbrakeInput => NetworkHandbrakeInput;

        private float steerSmoothTime = 0.15f; // Adjust for more/less smoothness
        private float steerVelocity = 0f;

        public override void FixedUpdateNetwork()
        {
            float targetSteerInput = GetKeyboardSteerInput();
            float targetAccelerationInput = GetKeyboardAccelerationInput();
            float targetHandbrakeInput = GetKeyboardHandbrakeInput();

            if (useMobileInput)
            {
                targetSteerInput = GetMobileSteerInput();
                targetAccelerationInput = GetMobileAccelerationInput();
                targetHandbrakeInput = GetMobileHandbrakeInput();
            }

            // Apply smooth steering while maintaining networked synchronization
            NetworkSteerInput = Mathf.Lerp(NetworkSteerInput, targetSteerInput, (Mathf.Abs(targetSteerInput) > 0 ? 10f : 20f) * Runner.DeltaTime);
            
            // Maintain instant acceleration and braking response
            NetworkAccelerationInput = Mathf.Lerp(NetworkAccelerationInput, targetAccelerationInput, 15 * Runner.DeltaTime);
            NetworkHandbrakeInput = targetHandbrakeInput;
        }

        // **Keyboard Input Methods**
        private float GetKeyboardSteerInput()
        {
            float steerInput = 0f;
            if (Input.GetKey(keyboardInput.steerLeft)) steerInput -= 1f;
            if (Input.GetKey(keyboardInput.steerRight)) steerInput += 1f;
            return steerInput;
        }

        private float GetKeyboardAccelerationInput()
        {
            float accelInput = 0f;
            if (Input.GetKey(keyboardInput.accelerate)) accelInput += 1f;
            if (Input.GetKey(keyboardInput.decelerate)) accelInput -= 1f;
            return accelInput;
        }

        private float GetKeyboardHandbrakeInput()
        {
            return Input.GetKey(keyboardInput.handBrake) ? 1f : 0f;
        }

        // **Mobile Input Methods (Using Steering Wheel Instead of Buttons)**
        private float GetMobileSteerInput()
        {
            return steeringWheel != null ? steeringWheel.SteeringInput : 0f;
        }

        private float GetMobileAccelerationInput()
        {
            float accelInput = 0f;
            if (mobileInput.accelerate.isPressed) accelInput += 1f;
            if (mobileInput.decelerate.isPressed) accelInput -= 1f;
            return accelInput;
        }

        private float GetMobileHandbrakeInput()
        {
            return mobileInput.handBrake.isPressed ? 1f : 0f;
        }
    }
}
