using UnityEngine;

public class CS_MultiStageDoor : MonoBehaviour
{
    [System.Serializable]
    private struct DoorState
    {
        [SerializeField] private int DoorIndex;
        
        [SerializeField] private Transform ClosedPos;
        
        [SerializeField] private Transform OpenPos;
        
        [SerializeField] private float Speed;
    
        [SerializeField] private bool IsOpen;
        
        private bool IsComplete;
    
    
    
        public void Init()
        {
            IsComplete = true;
        }
    
        public void Toggle()
        {
            IsOpen = !IsOpen;
            IsComplete = false;
        }
    
        public void Update(GameObject InDoor)
        {
            if (IsComplete)
            {
                return;
            }
    
            if (IsOpen)
            {
                bool bComplete = true;
                //if ( .DistSquared(InDoor.Transform.position, OpenPos.position) > 1.0f)
                {
                    //InDoor.Transform.position = OpenPos.position;
                }
                //else
                {
                    //InDoor.Transform.position = Mathf.Interp(InDoor.Transform.position, OpenPos.position, Speed * Time.deltaTime);
                    bComplete = false;
                }
            }
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
