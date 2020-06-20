using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW_Lib.Utility;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GW_Lib
{
    [System.Serializable]
    public class FloatUnityEvent : UnityEvent<float> { }
    [System.Serializable]
    public class StringUnityEvent : UnityEvent<string> { }

    public enum MoveDir { X, Z, Both }
    public enum DecisionOperator { Or, And }
    public enum Comparision { Less, More, LessEq, MoreEq, Eq }
    public enum BoxOffset { None, ToBot, ToMid, ToTop }

    public class LazyField<T>
    {
        public T field
        {
            get
            {
                if (m_Field == null)
                {
                    m_Field = GetField();
                }
                return m_Field;
            }
        }

        T m_Field;
        Func<T> GetField;

        public static LazyField<T> Get(Func<T> getter)
        {
            return new LazyField<T>(getter);
        }
        public LazyField(Func<T> GetField)
        {
            this.GetField = GetField;
        }
        public static implicit operator T(LazyField<T> lazyField)
        {
            return lazyField.field;
        }
    }
    public class SerializableVector3
    {
        public float x, y, z;
        public SerializableVector3() { }
        public SerializableVector3(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }
        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public SerializableVector3(SerializableVector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }
        public static implicit operator SerializableVector3(Vector3 v3)
        {
            return new SerializableVector3(v3);
        }
        public static implicit operator Vector3(SerializableVector3 v3)
        {
            return new Vector3(v3.x, v3.y, v3.z);
        }
    }


    public static class Extentions
    {
        public static void SetKinematic(this MonoBehaviour caller,bool toState)
        {
            Rigidbody rb3d = caller.GetComponent<Rigidbody>();
            if (rb3d == null)
                return;
            if (rb3d.isKinematic == toState)
                return;

            rb3d.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb3d.isKinematic = toState;
            caller.InvokeDelayed(new WaitForEndOfFrame(), () => {
                if (toState)
                {
                    rb3d.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }
                else
                {
                    rb3d.collisionDetectionMode = CollisionDetectionMode.Continuous;
                }
            });
        }
        public static void KillCoro(this MonoBehaviour caller, ref Coroutine c)
        {
            if (caller == null || c == null)
            {
                return;
            }
            caller.StopCoroutine(c); 
            c = null;
        }
        public static void BeginCoro(this MonoBehaviour caller, ref Coroutine c, IEnumerator call, bool clearOld = true)
        {
            if (clearOld)
            {
                KillCoro(caller, ref c);
            }
            c = caller.StartCoroutine(call);
        }
        public static Vector3 GetRndPosOnCircle(Vector3 around, float rad, bool navigable)
        {
            float f = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector3 rndCircle = new Vector3(Mathf.Cos(f), 0, Mathf.Sin(f));
            Vector3 pos = around + rndCircle * rad;
            if (navigable)
                pos = GetNavigablePos(pos, pos, 1, -2, 2, 2, false);
            return pos;
        }

        public static void GetPlayerViewPos(out float x, out float y)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.GetViewPos(out x, out y);

        }
        public static void GetViewPos(this Transform transform, out float x, out float y)
        {
            if (transform == null)
            {
                x = 0.5f;
                y = 0.5f;
                return;
            }

            Vector2 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            x = viewPos.x;
            y = viewPos.y;
        }

        public static Vector3 GetNavigablePos(Vector3 startNavPos, Vector3 posToSample, float charaHeigth, int minHeigthStep, int maxHeigthStep, int maxSampleMulti, bool debug)
        {
            Vector3 testPos = posToSample;
            for (int i = minHeigthStep; i <= maxHeigthStep; i++)
            {
                NavMeshHit h = new NavMeshHit();
                bool didHit = false;

                for (int s = 1; s <= maxSampleMulti; s++)
                {
                    didHit = NavMesh.SamplePosition(testPos, out h, charaHeigth * s, NavMesh.AllAreas);
                    if (didHit)
                    {
                        break;
                    }
                }

                if (!didHit)
                {
                    UpdateTestPos(ref testPos, posToSample, i);
                    continue;
                }

                NavMeshPath navPath = new NavMeshPath();
                bool hasPath = NavMesh.CalculatePath(startNavPos, h.position, NavMesh.AllAreas, navPath);
                if (navPath.status != NavMeshPathStatus.PathComplete)
                {
                    UpdateTestPos(ref testPos, posToSample, i);
                    if (debug)
                        LogSphere(testPos, Color.yellow);
                    continue;
                }
                return navPath.corners[navPath.corners.Length - 1];
            }

            void UpdateTestPos(ref Vector3 p, Vector3 oriPos, int i)
            {
                //testPos.y = pos.y + UnityEngine.Random.Range(-player.colliderHeight * 4, player.colliderHeight * 4);
                p.y = oriPos.y + i * charaHeigth;
            }

            return posToSample;
        }

        public static GameObject LogSphere(Vector3 pos, Color c)
        {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.GetComponent<Collider>().enabled = false;
            s.GetComponent<Renderer>().material.color = c;
            s.transform.position = pos;
            return s;
        }

        public static Coroutine InvokeDelayed(this MonoBehaviour m, float y, Action a)
        {
            return m.InvokeDelayed(new WaitForSeconds(y), a);
        }
        public static Coroutine InvokeDelayed(this MonoBehaviour m, CustomYieldInstruction y, Action a)
        {
            return m.StartCoroutine(DelayedInvoker(y, a));
        }
        public static Coroutine InvokeDelayed(this MonoBehaviour m, YieldInstruction y, Action a)
        {
            return m.StartCoroutine(DelayedInvoker(y, a));
        }
        private static System.Collections.IEnumerator DelayedInvoker(CustomYieldInstruction y, Action a)
        {
            yield return y;
            a?.Invoke();
        }
        private static System.Collections.IEnumerator DelayedInvoker(YieldInstruction y, Action a)
        {
            yield return y;
            a?.Invoke();
        }
        public static Vector3 GetLaunchVelocity(Vector3 from, Vector3 to, float travelHeigth, float gravity)
        {
            Vector3 launchVel = Vector3.zero;
            float dy = to.y - from.y;
            Vector3 dXZ = to - from;
            dXZ.y = 0;

            float yVel = Mathf.Sqrt(-2 * gravity * travelHeigth);

            float xzBot1 = Mathf.Sqrt(-2 * travelHeigth / gravity);
            float xzBot2 = Mathf.Sqrt(2 * (dy - travelHeigth) / gravity);

            Vector3 xzVel = dXZ / (xzBot1 + xzBot2);
            Vector3 startVel = xzVel + Vector3.up * yVel * -Mathf.Sign(gravity);

            return startVel;
        }
        public static float GetJumpVelocity(float jumpHeigth, float gravity)
        {
            return Mathf.Sqrt(-2 * gravity * jumpHeigth);
        }
        public static float GetTravelTime(Vector3 xi, Vector3 xf, Vector3 vi, Vector3 vf)
        {
            //x=x0+0.5(vi+vf)t
            //delta*2 = (vi+vf)t
            Vector3 delta = xf - xi;
            Vector3 totalV = vi + vf;
            float time = 2 * delta.magnitude / totalV.magnitude;
            return time;
        }
        public static Quaternion GetTransformedFacing(float byAngle, Transform startFacing, Transform axisRef, bool is2D)
        {
            if (!startFacing || !axisRef)
            {
                return Quaternion.identity;
            }
            Vector3 normal = axisRef.up;
            Vector3 vStartFacing = startFacing.forward;

            if (is2D)
            {
                normal = axisRef.forward;
                vStartFacing = startFacing.up;
            }

            Quaternion transformer = Quaternion.AngleAxis(byAngle, normal);
            Vector3 desiredFacing = transformer * vStartFacing;
            Quaternion targetRot = Quaternion.identity;

            if (is2D)
            {
                targetRot = Quaternion.LookRotation(normal, desiredFacing);
            }
            else
            {
                targetRot = Quaternion.LookRotation(desiredFacing, normal);
            }

            return targetRot;
        }
        /// <summary>
        /// Find some projected angle measure off some forward around some axis.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="norm"></param>
        /// <returns>Angle in degrees</returns>
        public static float AngleOffAroundAxis(Vector3 to, Vector3 from, Vector3 norm, bool clockwise = false)
        {
            Vector3 right;
            if (clockwise)
            {
                right = Vector3.Cross(from, norm);
                from = Vector3.Cross(norm, right);
            }
            else
            {
                right = Vector3.Cross(norm, from);
                from = Vector3.Cross(right, norm);
            }
            return Mathf.Atan2(Vector3.Dot(to, right), Vector3.Dot(to, from)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Get Z angle from two transforms position.
        /// </summary>
        public static float GetZangleFromTwoPosition(Transform fromTrans, Transform toTrans)
        {
            if (fromTrans == null || toTrans == null)
            {
                return 0f;
            }
            float xDistance = toTrans.position.x - fromTrans.position.x;
            float yDistance = toTrans.position.y - fromTrans.position.y;
            float angle = (Mathf.Atan2(yDistance, xDistance) * Mathf.Rad2Deg) - 90f;
            angle = GetNormalizedAngle(angle);

            return angle;
        }

        public static float GetAngleFromTwoPositions(Transform transform, Transform targetTransform, Transform axisRef, bool is2D)
        {
            Vector3 localTarPos = axisRef.InverseTransformPoint(targetTransform.position);
            Vector3 localStartPos = axisRef.InverseTransformPoint(transform.position);
            Vector3 localDir = (localTarPos - localStartPos).normalized;
            float angle = 0;

            if (is2D)
            {
                angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg - 90;
            }
            else
            {
                angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
            }

            return angle;
        }

        /// <summary>
        /// Get Y angle from two transforms position.
        /// </summary>
        public static float GetYangleFromTwoPosition(Transform fromTrans, Transform toTrans)
        {
            if (fromTrans == null || toTrans == null)
            {
                return 0f;
            }
            float xDistance = toTrans.position.x - fromTrans.position.x;
            float zDistance = toTrans.position.z - fromTrans.position.z;
            float angle = (Mathf.Atan2(zDistance, xDistance) * Mathf.Rad2Deg) - 90f;
            angle = GetNormalizedAngle(angle);

            return angle;
        }
        /// <summary>
        /// Get shifted angle.
        /// </summary>
        public static float GetShiftedAngle(int wayIndex, float baseAngle, float betweenAngle)
        {
            float angle = wayIndex % 2 == 0 ?
                          baseAngle - (betweenAngle * (float)wayIndex / 2f) :
                          baseAngle + (betweenAngle * Mathf.Ceil((float)wayIndex / 2f));
            return angle;
        }

        /// <summary>
        /// Get 0 ~ 360 angle.
        /// </summary>
        public static float GetNormalizedAngle(float angle)
        {
            while (angle < 0f)
            {
                angle += 360f;
            }
            while (360f < angle)
            {
                angle -= 360f;
            }
            return angle;
        }

        public static Vector3 KeepHighest(this Vector3 v3)
        {
            int max = 0;
            float maxVal = float.MinValue;
            for (int i = 0; i < 3; i++)
            {
                if (v3[i] > maxVal)
                {
                    max = i;
                    maxVal = v3[i];
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (i != max)
                {
                    v3[i] = 0;
                }
            }
            v3.Normalize();
            return v3;
        }
        public static Vector3 RemoveLowest(this Vector3 v3)
        {
            int min = 0;
            float minVal = float.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                if (v3[i] < minVal)
                {
                    min = i;
                    minVal = v3[i];
                }
            }
            v3[min] = 0;
            v3.Normalize();
            return v3;
        }
        public static bool IsNotNone(this BoxOffset offset)
        {
            return offset != BoxOffset.None;
        }
        public static bool IsNotBot(this BoxOffset offset)
        {
            return offset != BoxOffset.ToBot;
        }
        public static Vector3 GetBoxOffset(this Collider col, BoxOffset bOffset)
        {
            Vector3 offset = Vector3.zero;

            if (col)
            {
                Bounds destiBounds = col.bounds;
                if (bOffset == BoxOffset.ToBot)
                {
                    offset = Vector3.up * destiBounds.extents.y * -1;
                }
                else if (bOffset == BoxOffset.ToMid)
                {
                    offset = Vector3.up * destiBounds.extents.y;
                }
                else if (bOffset == BoxOffset.ToTop)
                {
                    offset = Vector3.up * destiBounds.size.y;
                }
            }

            return offset;
        }

        public static Vector3 RandomRange(Vector3 from, Vector3 to)
        {
            float x = UnityEngine.Random.Range(from.x, to.x);
            float y = UnityEngine.Random.Range(from.y, to.y);
            float z = UnityEngine.Random.Range(from.z, to.z);
            return new Vector3(x, y, z);
        }
        public static bool AreArraysEqual<T>(T[] a1, T[] a2)
        {
            bool isSameLength = a1.Length == a2.Length;
            if (isSameLength)
            {
                return Enumerable.SequenceEqual(a1, a2);
            }
            return false;
        }

        public static void Print<T>(this List<T> b)
        {
            foreach (var v in b)
            {
                Debug.Log(v);
            }
        }
        public static List<T> GetAndRemoveType<T>(ref List<Collider> from) where T : UnityEngine.Object
        {
            List<T> typeList = new List<T>();

            foreach (Collider col in from)
            {
                T t = col.GetComponent<T>();
                if (t != null && typeList.Contains(t) == false)
                {
                    typeList.Add(t);
                }
            }
            from.RemoveAll((o) => o.GetComponent<T>() != null);

            return typeList;
        }

        public static T GetObjBasedOnNameKey<T>(string key, T[] objs) where T : UnityEngine.Component
        {
            foreach (T g in objs)
            {
                if (GetNameBasedKey(g) == key)
                {
                    return g;
                }
            }
            return null;
        }
        public static string GetNameBasedKey(Component c)
        {
            return (c.name + "::" + c.transform.GetSiblingIndex().ToString());
        }

        public static Ray GetConeRay(int raysCount, float angle, int rayOrder, Vector3 rotEU, Vector3 origin)
        {
            float startingAngle = rotEU.y - angle / 2;

            float angleBetweenRays = angle / raysCount;
            float currentAngleDist = angleBetweenRays * rayOrder;
            float currentAngle = startingAngle + currentAngleDist;

            Quaternion rayRot = Quaternion.Euler(rotEU.x, currentAngle, rotEU.z);
            Vector3 dir = rayRot * Vector3.forward;

            return new Ray(origin, dir);
        }
        public static bool ConeSphereCast(out RaycastHit hit, int rays, float angle, float maxDist,
                            Vector3 origin, Vector3 startRotEU, float radious, LayerMask layers, bool debug = false)
        {
            hit = new RaycastHit();
            for (int i = 0; i < rays; i++)
            {
                Ray ray = GetConeRay(rays, angle, i, startRotEU, origin);

                if (debug)
                {
                    Debug.DrawRay(ray.origin, ray.direction * maxDist, Color.yellow);
                }

                if (Physics.SphereCast(ray, radious, out hit, maxDist, layers))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool ConeCast(out RaycastHit hit, int rays, float angle, float maxDist,
                Vector3 origin, Vector3 startRotEU, LayerMask layers, bool debug = false)
        {
            hit = new RaycastHit();
            for (int i = 0; i < rays; i++)
            {
                Ray ray = GetConeRay(rays, angle, i, startRotEU, origin);

                if (debug)
                {
                    Debug.DrawRay(ray.origin, ray.direction * maxDist, Color.yellow);
                }

                if (Physics.Raycast(ray, out hit, maxDist, layers))
                {
                    return true;
                }
            }

            return false;
        }
        public static List<RaycastHit> ConeSphereCastAll(int rays, float angle, float maxDist, Vector3 origin,
                        Vector3 rotEU, float radious, LayerMask layers, bool debug = false)
        {
            List<RaycastHit> hits = new List<RaycastHit>();
            for (int i = 0; i < rays; i++)
            {
                Ray ray = GetConeRay(rays, angle, i, rotEU, origin);
                RaycastHit hit;
                if (Physics.SphereCast(ray, radious, out hit, maxDist, layers))
                {
                    hits.Add(hit);
                }
                if (debug)
                {
                    Debug.DrawRay(ray.origin, ray.direction * maxDist);
                }

            }
            return hits;
        }

        public static string GenerateUUID(Component comp)
        {
            string uuid;
            bool isPartOfScene = IsInAScene(comp);
            if (isPartOfScene)
            {
                uuid = Guid.NewGuid().ToString();
            }
            else
            {
                uuid = string.Empty;
            }
            return uuid;
        }
        public static bool IsInAScene(Component c)
        {
            return string.IsNullOrEmpty(c.gameObject.scene.path) == false;
        }

        public static void SetLayerTo(this MonoBehaviour mono, int layer, bool includeAllChildren)
        {
            Transform[] allChildren = mono.GetComponentsInChildren<Transform>();

            foreach (Transform t in allChildren)
            {
                t.gameObject.layer = layer;
            }
            mono.gameObject.layer = layer;
        }

        public static void SetLayerTo(this MonoBehaviour mono, SingleUnityLayer layer, bool includeAllChildren)
        {
            mono.SetLayerTo(layer.Layer, includeAllChildren);
        }

        public static Color DampColor(this Color color, float amountAsPercent, bool includeAlpha = false)
        {
            Color damped = color;
            damped.r = damped.r + amountAsPercent;
            damped.g = damped.g + amountAsPercent;
            damped.b = damped.b + amountAsPercent;

            if (includeAlpha)
            {
                damped.a = damped.a + amountAsPercent;
            }

            return damped;
        }
        public static List<T> GetAllObjsInGame<T>(GameObject persistantScene, bool includeInactive = true) where T : UnityEngine.Object
        {
            List<T> allObjs = new List<T>();
            int count = SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                List<T> objs = Extentions.GetAllObjectsInScene<T>(s, includeInactive);
                foreach (T obj in objs)
                {
                    if (allObjs.Contains(obj))
                    {
                        continue;
                    }
                    allObjs.Add(obj);
                }
            }
            T[] persistantObjs = persistantScene.GetComponentsInChildren<T>();
            foreach (T s in persistantObjs)
            {
                if (allObjs.Contains(s))
                {
                    continue;
                }
                allObjs.Add(s);
            }
            return allObjs;
        }
        public static List<T> GetAllObjectsInScene<T>(Scene fromScene, bool includeInactive = true) where T : UnityEngine.Object
        {
            List<T> allObjs = new List<T>();
            GameObject[] roots = fromScene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                T[] objsInRoot = root.GetComponentsInChildren<T>(includeInactive);
                foreach (T objInroot in objsInRoot)
                {
                    allObjs.Add(objInroot);
                }
            }
            return allObjs;
        }

        public static T SafeGetComponent<T>(this Component obj, T objToCheck) where T : Component
        {
            if (objToCheck == null)
            {
                objToCheck = obj.GetComponent<T>();
            }
            return objToCheck;
        }
        public static AnimationClip GetClip(this AnimationClip[] clips, int index = -1, bool keepEvents = true)
        {
            if (clips.Length == 0)
            {
                return null;
            }
            AnimationClip clip = clips.GetElementOfArray(index);
            if (keepEvents)
            {
                return clip;
            }
            return ClearEvents(clip);
        }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index">use -1 for random element</param>
        /// <returns></returns>
        public static T GetElementOfArray<T>(this T[] array, int index)
        {
            if (array.Length == 0)
            {
                return default(T);
            }
            if (index == -1 || index >= array.Length || index < 0)
            {
                int randIndex = UnityEngine.Random.Range(0, array.Length);
                return array[randIndex];
            }
            return array[index];
        }

        public static AnimationClip ClearEvents(this AnimationClip clip)
        {
            clip.events = new AnimationEvent[0];
            return clip;
        }
        public static UnityEngine.Object[] ToObjArray<T>(this T[] arrayToTurn) where T : UnityEngine.Object
        {
            UnityEngine.Object[] os = new UnityEngine.Object[arrayToTurn.Length];
            for (int i = 0; i < arrayToTurn.Length; i++)
            {
                os[i] = arrayToTurn[i];
            }
            return os;
        }
        public static T[] FromObjArray<T>(this UnityEngine.Object[] arrayToTurn) where T : UnityEngine.Object
        {
            T[] tes = new T[arrayToTurn.Length];
            for (int i = 0; i < tes.Length; i++)
            {
                tes[i] = arrayToTurn[i] as T;
            }
            return tes;
        }

        public static void Copy(this Transform toTake, Transform transToCopy, bool local = true, bool scale = false)
        {
            if (local)
            {
                toTake.localPosition = transToCopy.localPosition;
                toTake.localRotation = transToCopy.localRotation;
                if (scale)
                {
                    toTake.localScale = transToCopy.localScale;
                }
            }
            else
            {
                toTake.position = transToCopy.position;
                toTake.rotation = transToCopy.rotation;
                if (scale)
                {
                    toTake.localScale = transToCopy.localScale;
                }
            }
        }
        public static void ReSetTransform(this Transform t, bool local = true, bool scale = false)
        {
            if (local)
            {
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                if (scale)
                {
                    t.localScale = Vector3.one;
                }
            }
            else
            {
                t.position = Vector3.zero;
                t.rotation = Quaternion.identity;
                if (scale)
                {
                    t.localScale = Vector3.one;
                }
            }
        }

        public static void GenerateIdentifiers(out string description, out int identifer)
        {
            identifer = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            description = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
        }

        public static bool FCompare(Comparision comparisionType, float leftTerm, float rightTerm)
        {
            bool leftMakesRight = false;
            if (comparisionType == Comparision.Less)
            {
                leftMakesRight = leftTerm < rightTerm;
            }
            else if (comparisionType == Comparision.More)
            {
                leftMakesRight = leftTerm > rightTerm;
            }
            else if (comparisionType == Comparision.LessEq)
            {
                leftMakesRight = leftTerm <= rightTerm;
            }
            else if (comparisionType == Comparision.MoreEq)
            {
                leftMakesRight = leftTerm >= rightTerm;
            }
            else if (comparisionType == Comparision.Eq)
            {
                leftMakesRight = Mathf.Abs(leftTerm - rightTerm) < Mathf.Epsilon;
            }
            return leftMakesRight;
        }

        public static Quaternion GetFlatRot(this Vector3 from, Vector3 to)
        {
            Vector3 dir = to - from;
            if (dir == Vector3.zero)
            {
                return Quaternion.identity;
            }
            dir.Normalize();

            Quaternion q = Quaternion.LookRotation(dir);
            Vector3 eu = q.eulerAngles;
            eu.x = eu.z = 0;
            q = Quaternion.Euler(eu);

            return q;
        }

        public static GameObject[] GetObjsOfTagInChildren(this Transform t, string tag, bool includeInactive = true)
        {
            List<GameObject> g = new List<GameObject>();
            Transform[] children = t.GetComponentsInChildren<Transform>(true);
            foreach (Transform c in children)
            {
                if (c.CompareTag(tag) && g.Contains(c.gameObject) == false)
                {
                    g.Add(c.gameObject);
                }
            }
            return g.ToArray();
        }
        public static GameObject GetObjOfTagInChildren(this Transform t, string tag, bool includeInactive = true)
        {
            Transform[] children = t.GetComponentsInChildren<Transform>(true);
            foreach (Transform c in children)
            {
                if (c.CompareTag(tag))
                {
                    return c.gameObject;
                }
            }
            return null;
        }

        public static float GetRad(this Transform transform, float defVal)
        {
            SphereCollider sCol = transform.GetComponent<SphereCollider>();
            if (sCol)
                return sCol.radius;
            CapsuleCollider cCol = transform.GetComponent<CapsuleCollider>();
            if (cCol)
                return cCol.radius;
            NavMeshAgent agent = transform.GetComponent<NavMeshAgent>();
            if (agent)
                return agent.radius;
            BoxCollider bCol = transform.GetComponent<BoxCollider>();
            if (bCol)
                return bCol.size.x / 2.0f;
            return defVal;
        }
    }
    public static class MathExtensions
    {
        public static Vector3 GetAveragePosition<T>(this List<T> t) where T : MonoBehaviour
        {
            int sum = 0;
            List<Vector3> positions = new List<Vector3>();
            foreach (T i in t)
            {
                positions.Add(i.transform.position);
            }
            sum = positions.Count;
            if (sum == 0)
            {
                sum = 1;
            }
            Vector3 averagePosition = new Vector3();
            foreach (Vector3 v in positions)
            {
                averagePosition += v;
            }
            averagePosition = averagePosition / sum;
            return averagePosition;
        }
        public static float RotPerMinuteToRotPerFrame(float rotation, float deltaTime)
        {
            return FromPerMinuteToPerFrame(rotation, deltaTime) * 360;
        }
        public static float FromPerMinuteToPerFrame(float perMinute, float deltaTime)
        {
            return FromPerMinuteToPerSecond(perMinute) * deltaTime;
        }
        public static float FromPerMinuteToPerSecond(float perMinute)
        {
            return perMinute * (1.0f / 60.0f);
        }
        public static float FromPerSecToPerFrame(float perSec, float deltaTime)
        {
            return perSec * deltaTime;
        }

        public static float GetColRadiOf(Component callBackObj)
        {
            SphereCollider sc = callBackObj.GetComponent<SphereCollider>();
            if (sc)
            {
                return sc.radius;
            }
            CapsuleCollider cc = callBackObj.GetComponent<CapsuleCollider>();
            if (cc)
            {
                return cc.radius;
            }
            BoxCollider bc = callBackObj.GetComponent<BoxCollider>();
            if (bc)
            {
                return bc.bounds.extents.magnitude;
            }
            return 0;
        }

        public static int RoundOnEndToInt(float f)
        {
            int d = Mathf.CeilToInt(f);
            float difference = Mathf.Abs(d - f);
            if (difference <= 0.1f)
            {
                return d;
            }
            return Mathf.FloorToInt(f);
        }
        public static Vector3 ShortenVector(this Vector3 v, float by)
        {
            Vector3 shortenedVector = new Vector3();
            shortenedVector = v - v.normalized * by;
            return shortenedVector;
        }
    }
    public static class StringExtentions
    {
        public static string CutString(this string s, int maxCharCount)
        {
            string cut = "";
            if (s == null)
            {
                return "";
            }
            int charCount = s.Length;
            if (charCount > maxCharCount)
            {
                for (int i = 0; i < maxCharCount; i++)
                {
                    cut += s[i];
                }
            }
            else
            {
                cut = s;
            }
            return cut;
        }
        public static void GetBold(this string s, out string bold)
        {
            string start = "<b>";
            string middle = s;
            string end = "</b>";
            bold = start + middle + end;
        }
        public static void GetItalic(this string s, out string italic)
        {
            string start = "<i>";
            string middle = s;
            string end = "</i>";
            italic = start + middle + end;
        }
        public static void GetColored(this string s, Color color, out string colored)
        {
            //<color=yellow>RICH</color>
            string htmlFormColor = ColorUtility.ToHtmlStringRGBA(color);
            string start = "<color=" + "#" + htmlFormColor + ">";
            string middle = s;
            string end = "</color>";
            colored = start + middle + end;
        }
    }

    public static class ArrayExtentions
    {
        public static T[] RemoveElementFromArray<T>(this T[] array, int index)
        {
            T[] modifiedArray = (T[])array.Clone();

            if (index >= 0 && index < array.Length)
            {
                List<T> modifiedArrayList = modifiedArray.ToList();
                modifiedArrayList.Remove(modifiedArrayList[index]);
                modifiedArray = modifiedArrayList.ToArray();
            }

            return modifiedArray;
        }
        public static T[] SwapInArray<T>(this T[] array, int from, int to)
        {
            T[] modifiedArray = (T[])array.Clone();
            T original = modifiedArray[from];

            if (from > to && to >= 0)
            {
                modifiedArray[from] = modifiedArray[to];
                modifiedArray[to] = original;
            }
            if (from <= to && to < array.Length)
            {
                modifiedArray[from] = modifiedArray[to];
                modifiedArray[to] = original;
            }
            return modifiedArray;
        }
        public static T[] ReSizeArray<T>(this T[] array, int bySize)
        {
            T[] newArray = new T[bySize];

            if (array.Length < bySize)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    newArray[i] = array[i];
                }
            }
            else if (array.Length > bySize)
            {
                for (int i = 0; i < bySize; i++)
                {
                    newArray[i] = array[i];
                }
            }
            return newArray;
        }
    }
    public static class ListExtentions
    {
        public static List<RaycastHit> SortHitsByDist(this List<RaycastHit> hits)
        {
            List<RaycastHit> sortedHits = new List<RaycastHit>();
            foreach (RaycastHit hit in hits)
            {
                sortedHits.Add(hit);
            }
            sortedHits.Sort(HitsDistanceSorter);
            return sortedHits;
        }

        private static int HitsDistanceSorter(RaycastHit x, RaycastHit y)
        {
            return x.distance.CompareTo(y.distance);
        }
        public static bool CheckOperationOnList(this List<bool> list, DecisionOperator op, bool defaultsTo = true)
        {
            if (list.Count == 0)
            {
                return defaultsTo;
            }
            if (op == DecisionOperator.And)
            {
                foreach (bool b in list)
                {
                    if (b == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (op == DecisionOperator.Or)
            {
                foreach (bool b in list)
                {
                    if (b == true)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                throw new Exception("Un Defined Bool Operation");
            }
        }
    }


    public sealed class Mathfx
    {
        //Ease in out
        public static float Hermite(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
        }

        public static Vector2 Hermite(Vector2 start, Vector2 end, float value)
        {
            return new Vector2(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value));
        }

        public static Vector3 Hermite(Vector3 start, Vector3 end, float value)
        {
            return new Vector3(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value), Hermite(start.z, end.z, value));
        }

        //Ease out
        public static float Sinerp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
        }

        public static Vector2 Sinerp(Vector2 start, Vector2 end, float value)
        {
            return new Vector2(Mathf.Lerp(start.x, end.x, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.y, end.y, Mathf.Sin(value * Mathf.PI * 0.5f)));
        }

        public static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
        {
            return new Vector3(Mathf.Lerp(start.x, end.x, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.y, end.y, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.z, end.z, Mathf.Sin(value * Mathf.PI * 0.5f)));
        }
        //Ease in
        public static float Coserp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
        }

        public static Vector2 Coserp(Vector2 start, Vector2 end, float value)
        {
            return new Vector2(Coserp(start.x, end.x, value), Coserp(start.y, end.y, value));
        }

        public static Vector3 Coserp(Vector3 start, Vector3 end, float value)
        {
            return new Vector3(Coserp(start.x, end.x, value), Coserp(start.y, end.y, value), Coserp(start.z, end.z, value));
        }

        //Boing
        public static float Berp(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        public static Vector2 Berp(Vector2 start, Vector2 end, float value)
        {
            return new Vector2(Berp(start.x, end.x, value), Berp(start.y, end.y, value));
        }

        public static Vector3 Berp(Vector3 start, Vector3 end, float value)
        {
            return new Vector3(Berp(start.x, end.x, value), Berp(start.y, end.y, value), Berp(start.z, end.z, value));
        }

        //Like lerp with ease in ease out
        public static float SmoothStep(float x, float min, float max)
        {
            x = Mathf.Clamp(x, min, max);
            float v1 = (x - min) / (max - min);
            float v2 = (x - min) / (max - min);
            return -2 * v1 * v1 * v1 + 3 * v2 * v2;
        }

        public static Vector2 SmoothStep(Vector2 vec, float min, float max)
        {
            return new Vector2(SmoothStep(vec.x, min, max), SmoothStep(vec.y, min, max));
        }

        public static Vector3 SmoothStep(Vector3 vec, float min, float max)
        {
            return new Vector3(SmoothStep(vec.x, min, max), SmoothStep(vec.y, min, max), SmoothStep(vec.z, min, max));
        }

        public static float Lerp(float start, float end, float value)
        {
            return ((1.0f - value) * start) + (value * end);
        }

        public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
            float closestPoint = Vector3.Dot((point - lineStart), lineDirection);
            return lineStart + (closestPoint * lineDirection);
        }

        public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 fullDirection = lineEnd - lineStart;
            Vector3 lineDirection = Vector3.Normalize(fullDirection);
            float closestPoint = Vector3.Dot((point - lineStart), lineDirection);
            return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
        }

        //Bounce
        public static float Bounce(float x)
        {
            return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
        }

        public static Vector2 Bounce(Vector2 vec)
        {
            return new Vector2(Bounce(vec.x), Bounce(vec.y));
        }

        public static Vector3 Bounce(Vector3 vec)
        {
            return new Vector3(Bounce(vec.x), Bounce(vec.y), Bounce(vec.z));
        }

        // test for value that is near specified float (due to floating point inprecision)
        // all thanks to Opless for this!
        public static bool Approx(float val, float about, float range)
        {
            return ((Mathf.Abs(val - about) < range));
        }

        // test if a Vector3 is close to another Vector3 (due to floating point inprecision)
        // compares the square of the distance to the square of the range as this 
        // avoids calculating a square root which is much slower than squaring the range
        public static bool Approx(Vector3 val, Vector3 about, float range)
        {
            return ((val - about).sqrMagnitude < range * range);
        }

        /*
          * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
          * This is useful when interpolating eulerAngles and the object
          * crosses the 0/360 boundary.  The standard Lerp function causes the object
          * to rotate in the wrong direction and looks stupid. Clerp fixes that.
          */
        public static float Clerp(float start, float end, float value)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
            float retval = 0.0f;
            float diff = 0.0f;

            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else retval = start + (end - start) * value;

            // Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
            return retval;
        }

    }
}