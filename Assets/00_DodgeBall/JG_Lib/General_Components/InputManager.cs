using UnityEngine;
using System;

namespace GW_Lib
{
    public class InputManager : MonoBehaviour
    {
        public event Action<float> WheelScroll;
        public event Action OnPausedGame;
        public event Action OnZoomRestUp;
        public event Action OnSoundSwitchClicked;

        public bool LeftClickDown { get; private set; }
        public bool RightClickDown { get; private set; }

        public bool LeftClickUp { get; private set; }
        public bool RightClickUp { get; private set; }

        public bool RightClickHeld { get; private set; }
        public bool LeftClickHeld { get; private set; }
        public bool SwitchedCamera { get; private set; }

        public bool CancelSceneKeyUp { get; private set; }

        public bool PauseGameKeyUp { get; private set; }


        [SerializeField] KeyCode cameraSwitchKey = KeyCode.F;
        [SerializeField] KeyCode cancelCutSceneKey = KeyCode.Escape;
        [SerializeField] KeyCode pauseGameKey = KeyCode.F10;
        [SerializeField] KeyCode restZoomKey = KeyCode.R;
        [SerializeField] KeyCode soundSwitch = KeyCode.M;

        float scrollValue;

        private void Update()
        {
            LeftClickDown = Input.GetMouseButtonDown(0);
            RightClickDown = Input.GetMouseButtonDown(1);

            LeftClickUp = Input.GetMouseButtonUp(0);
            RightClickUp = Input.GetMouseButtonUp(1);

            LeftClickHeld = Input.GetMouseButton(0);
            RightClickHeld = Input.GetMouseButton(1);

            SwitchedCamera = Input.GetKeyUp(cameraSwitchKey);

            CancelSceneKeyUp = Input.GetKeyUp(cancelCutSceneKey);

            if (Input.GetKeyUp(pauseGameKey) && OnPausedGame!=null)
            {
                OnPausedGame();
            }

            if (WheelScroll!=null)
            {
                scrollValue = Input.GetAxisRaw("Mouse ScrollWheel");
                WheelScroll(-scrollValue);
            }

            if (Input.GetKeyUp(restZoomKey))
            {
                if (OnZoomRestUp != null)
                {
                    OnZoomRestUp();
                }
            }

            if (Input.GetKeyUp(soundSwitch))
            {
                OnSoundSwitchClicked?.Invoke();
            }
        }
    }
}