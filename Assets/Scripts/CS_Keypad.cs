using UnityEngine;
using UnityEngine.Events;

public class CS_Keypad : MonoBehaviour
{
    private GameObject KeysHolder;

    public UnityEvent<int> OnKeyInput;
    public UnityEvent OnKeySubmission;
    public UnityEvent OnKeypadClear;
    

    public void HandleInput(int InKeyInput)
    {
        if (InKeyInput == -1)
        {
            OnKeypadClear.Invoke();
        }
        else if (InKeyInput == -2)
        {
            OnKeySubmission.Invoke();
        }
        else
        {
            OnKeyInput.Invoke(InKeyInput);
        }
    }
}
