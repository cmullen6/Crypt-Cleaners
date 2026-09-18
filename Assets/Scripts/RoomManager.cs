using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Exit")]
    [SerializeField] private GameObject exitDoor;

    private bool roomComplete;

    private void Start()
    {
        if (exitDoor != null)
            exitDoor.SetActive(false);
    }

    private void Update()
    {
        if (roomComplete)
            return;

        if (CleaningManager.Instance == null)
            return;

        if (CleaningManager.Instance.RoomComplete)
        {
            CompleteRoom();
        }
    }

    private void CompleteRoom()
    {
        roomComplete = true;

        if (exitDoor != null)
            exitDoor.SetActive(true);

        Debug.Log("ROOM CLEANED!");
    }
}


