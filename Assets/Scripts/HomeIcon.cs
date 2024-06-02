using UnityEngine;

public class HomeIcon : MonoBehaviour
{
    public void GoToHome()
    {
        GameManager.Instance.LoadHomeScene();
    }
}
