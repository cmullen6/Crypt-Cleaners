using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceManager : MonoBehaviour
{



    public static ResourceManager instance;

    // Initializing resources
    public int gold;
    public int recA;
    public int recB;
    public int recC;
    public int recD;

    private void Awake()
    {

        if (instance != null)
        {

            Destroy(gameObject);
            return;

        }



        instance = this;

        DontDestroyOnLoad(gameObject);



    }


}