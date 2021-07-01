using UnityEngine;
using UnityEngine.UI;


public class MoveBackButton : MonoBehaviour
{
    private void Awake()
    {
        Button button = transform.AddOrGetComponent<Button>();
        button.AddListener(this, UIStackManager.Instance.MoveBack);
    }
}