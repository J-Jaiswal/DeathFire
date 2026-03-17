using UnityEngine;


public class SceneReadySignal : MonoBehaviour
{
    void Start()
    {
        SceneLoader.Instance?.OnSceneReady();
    }
}