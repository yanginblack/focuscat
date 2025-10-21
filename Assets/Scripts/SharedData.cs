using UnityEngine;
using static UserDataManager;
using static ItemManager;

public class SharedData : MonoBehaviour
{
    public static SharedData I { get; set; }
    public ItemDatabase itemDatabase;
    public UserData userData;
    void Start()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        itemDatabase = ItemManager.Load();
        if (itemDatabase != null)
        {
            foreach (var item in itemDatabase.items)
                Debug.Log(item.ToString());
        }

        UserData userData = UserDataManager.Load();
        Debug.Log(userData.ToString());
    }
}
