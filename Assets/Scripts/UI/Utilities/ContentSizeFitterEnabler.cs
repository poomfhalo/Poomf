using UnityEngine;
using UnityEngine.UI;

namespace Poomf.UI
{
    public class ContentSizeFitterEnabler : MonoBehaviour
    {
        private void Start()
        {
            ContentSizeFitter contentSizeFitter = GetComponent<ContentSizeFitter>();

            if (null != contentSizeFitter)
                contentSizeFitter.enabled = true;
        }
    }
}