using UnityEngine.EventSystems;

namespace StealthGame.CamAndUi
{
    public class ExtendedInputModule : StandaloneInputModule
    {
        public PointerEventData GetPointerEventData(int pointerID = -1)
        {
            PointerEventData pEventData = null;
            GetPointerData(pointerID, out pEventData, true);
            return pEventData;
        }
    }
}