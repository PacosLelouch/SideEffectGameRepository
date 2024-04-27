//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AddCollisionBox : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem.Controls;

public class DestroyPrompt : EditorWindow
{
    public static void ShowPrompt(Action inAction, string title = "清理Collider确认", string inPromptTips = "确定吗？")
    {
        DestroyPrompt destroyPrompt = EditorWindow.GetWindow<DestroyPrompt>(title);
        //设置窗口在屏幕中的位置
        destroyPrompt.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);
        destroyPrompt.action = inAction;
        destroyPrompt.promptTips = inPromptTips;
        //显示窗口
        destroyPrompt.Show();
    }

    Action action;
    string promptTips;
    private void OnGUI()
    {
        //一个输入框
        EditorGUILayout.LabelField(promptTips ?? "确定吗？");
        if (GUILayout.Button("确定") && action != null)
        {
            action();
            //关闭窗口
            Close();
        }
        if (GUILayout.Button("取消"))
        {
            //关闭窗口
            Close();
        }

    }
}

public class AddCollisionBox
{
    [MenuItem("GameObject/Custom Tools/添加碰撞体/一键添加所有碰撞盒", isValidateFunction: true)]
    private static bool Anchor_AddCollisionBoxForMeshes_Validate()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("GameObject/Custom Tools/添加碰撞体/一键添加所有碰撞盒")]
    private static void Anchor_AddCollisionBoxForMeshes()
    {
        if (Selection.activeObject)
        {
            AddColliderForMeshes((GameObject)Selection.activeObject);
        }

    }

    public static void AddColliderForMeshes(GameObject obj)
    {
        if (obj.transform.childCount > 0)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                if (child.GetComponent<MeshRenderer>())
                {
                    child.gameObject.AddComponent<MeshCollider>();
                }

                AddColliderForMeshes(child.gameObject);
            }
        }
    }
}

public class DestoryColliders
{
    [MenuItem("GameObject/Custom Tools/删除碰撞体/一键清理所有Collider", isValidateFunction: true)]
    [MenuItem("GameObject/Custom Tools/删除碰撞体/一键清理骨骼的所有Collider", isValidateFunction: true)]
    private static bool Anchor_ClearTreeCollider_Validate()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("GameObject/Custom Tools/删除碰撞体/一键清理所有Collider")]
    public static void Anchor_ClearTreeCollider()
    {
        if (Selection.activeObject)
        {
            DestroyPrompt.ShowPrompt(() =>
            {
                ClearMeshColliderByChild((GameObject)Selection.activeObject, false);
            });
        }
    }
    [MenuItem("GameObject/Custom Tools/删除碰撞体/一键清理骨骼的所有Collider")]
    public static void Anchor_ClearTreeSkeletonCollider()
    {
        if (Selection.activeObject)
        {
            DestroyPrompt.ShowPrompt(() =>
            {
                ClearMeshColliderByChild((GameObject)Selection.activeObject, true);
            });
        }
    }
    public static void ClearMeshColliderByChild(GameObject obj, bool destroyObject = false)
    {
        if (obj.transform.childCount > 0)
        {
            List<GameObject> colliderObjectsToDestroy = new List<GameObject>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                //MeshCollider meshCollider = child.GetComponent<MeshCollider>();
                //if (meshCollider != null)
                //{
                //    UnityEngine.Object.DestroyImmediate(meshCollider);
                //}
                //BoxCollider boxCollider = child.GetComponent<BoxCollider>();
                //if (boxCollider != null)
                //{
                //    Debug.Log(boxCollider.name);
                //    UnityEngine.Object.DestroyImmediate(boxCollider);
                //}
                ClearMeshColliderByChild(child, destroyObject);
                Collider[] colliders = child.GetComponents<Collider>();
                if (!destroyObject)
                {
                    foreach (Collider collider in colliders)
                    {
                        Debug.Log($"Clear {collider.name}");
                        UnityEngine.Object.DestroyImmediate(collider);
                    }
                }
                else if (colliders.Length > 0)
                {
                    colliderObjectsToDestroy.Add(child.gameObject);
                }
            }

            foreach (GameObject child in colliderObjectsToDestroy)
            {
                Debug.Log($"Clear {child.gameObject.name}");
                UnityEngine.Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}

public class AddCollisionCapsuleWindow : EditorWindow
{
    public string radiusString = "20";

    private void OnGUI()
    {
        //一个输入框
        radiusString = EditorGUILayout.TextField("半径", radiusString);
        if (GUILayout.Button("确定"))
        {
            if (float.TryParse(radiusString, out float radius))
            {
                AddCollisionCapsule.AddColliderForSkeleton((GameObject)Selection.activeObject, radius);
            }
            //关闭窗口
            Close();
        }
        if (GUILayout.Button("取消"))
        {
            //关闭窗口
            Close();
        }

    }
}

public class AddCollisionCapsule
{

    [MenuItem("GameObject/Custom Tools/添加碰撞体/一键给骨骼添加胶囊体", isValidateFunction:true)]
    private static bool Anchor_AddCollisionCapsuleForSkeleton_Validate()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("GameObject/Custom Tools/添加碰撞体/一键给骨骼添加胶囊体")]
    private static void Anchor_AddCollisionCapsuleForSkeleton()
    {
        if (Selection.activeObject)
        {
            AddCollisionCapsuleWindow window = EditorWindow.GetWindow<AddCollisionCapsuleWindow>("碰撞体设置");
            //设置窗口在屏幕中的位置
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);
            //显示窗口
            window.Show();
            // AddColliderForSkeleton((GameObject)Selection.activeObject, 20f);
        }
    }

    public static void AddColliderForSkeleton(GameObject obj, float radius)
    {
        Transform parent = obj.transform.parent;
        if (parent != null && parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform sibling = parent.GetChild(i);
                SkinnedMeshRenderer mesh = sibling.GetComponent<SkinnedMeshRenderer>();
                if (mesh != null)
                {
                    AddColliderForSkeletonInternal(obj, mesh, radius);
                }
            }
        }
    }

    protected static void AddColliderForSkeletonInternal(GameObject obj, SkinnedMeshRenderer mesh, float radius)
    {
        if (obj.transform.childCount > 0)
        {
            int previousChildCount = obj.transform.childCount;
            for (int i = 0; i < previousChildCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                GameObject colliderObject = new GameObject($"Collider:{child.name}");
                colliderObject.transform.parent = obj.transform;
                colliderObject.transform.localPosition = Vector3.zero;
                colliderObject.transform.localRotation = Quaternion.identity;
                colliderObject.transform.localScale = Vector3.one;

                CapsuleCollider collider = colliderObject.AddComponent<CapsuleCollider>();
                collider.center = (child.localPosition) * 0.5f;
                collider.height = (child.localPosition).magnitude;
                collider.radius = radius;
                collider.direction = 0;

                AddColliderForSkeletonInternal(child.gameObject, mesh, radius);
            }
        }
    }
}

