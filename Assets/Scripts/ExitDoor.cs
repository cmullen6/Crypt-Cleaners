using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (CleaningManager.Instance == null)
            return;

        if (!CleaningManager.Instance.RoomComplete)
        {
            Debug.Log("The room isn't clean enough!");
            return;
        }

        Debug.Log("Room complete!");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}
