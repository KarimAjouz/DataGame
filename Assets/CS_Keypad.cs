using UnityEngine;
using UnityEngine.Events;

public class CS_Keypad : MonoBehaviour
{
    private GameObject KeysHolder;

    public UnityEvent<int> OnKeyInput;
    public UnityEvent OnKeypadClear;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleInput(int InKeyInput)
    {
        if (InKeyInput == -1)
        {
            OnKeypadClear.Invoke();
        }
        else
        {
            OnKeyInput.Invoke(InKeyInput);
        }
    }
}
