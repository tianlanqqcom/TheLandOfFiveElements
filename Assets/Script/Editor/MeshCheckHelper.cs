using UnityEditor;
using UnityEngine;

namespace Script.Editor
{
    public class MeshCheckHelper : MonoBehaviour
    {
        [MenuItem("Window/检测mesh状态")]
        public static void CheckMesh()
        {
            Debug.Log("Begin Check Mesh.");
            GameObject obj = Selection.activeGameObject;
            if (obj == null) return;
            MeshFilter[] filter = obj.GetComponentsInChildren<MeshFilter>(true);
            foreach(var item in filter)
            {
                if(item.sharedMesh == null)
                {
                    Debug.LogError("该meshfilter的mesh为空  " + item);
                }
            }
        }

    }
}
