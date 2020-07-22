using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GW_Lib.Utility
{
    public class FindMissingScripts : EditorWindow
    {
        [MenuItem("Stealth Game/FindMissingScripts")]
        public static void CollectMissingScripts()
        {
            List<Transform> go = Extentions.GetAllObjectsInScene<Transform>(SceneManager.GetActiveScene(), true);
            int go_count = 0, components_count = 0, missing_count = 0;
            foreach (Transform g in go)
            {
                go_count++;
                Component[] components = g.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    components_count++;
                    if (components[i] == null)
                    {
                        missing_count++;
                        string s = g.name;
                        Transform t = g.transform;
                        while (t.parent != null)
                        {
                            s = t.parent.name + "/" + s;
                            t = t.parent;
                        }
                        Debug.Log(s + " has an empty script attached in position: " + i, g);
                    }
                }
            }

            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }
    }
}

