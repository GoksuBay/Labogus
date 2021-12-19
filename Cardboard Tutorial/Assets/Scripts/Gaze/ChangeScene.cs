using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public GameObject player;
    public string scene;

    public void SceneChange()
    {
        SceneManager.LoadScene(scene);
    }
}
