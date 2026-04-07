using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Restart");
        Scene currentScene = gameObject.scene;
        SceneManager.LoadScene(currentScene.name);
    }
}
