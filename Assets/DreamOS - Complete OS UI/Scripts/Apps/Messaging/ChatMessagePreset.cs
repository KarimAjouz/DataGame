using UnityEngine;
using TMPro;

namespace Michsky.DreamOS
{
    public class ChatMessagePreset : MonoBehaviour
    {
        public TextMeshProUGUI contentText;
        public TextMeshProUGUI timeText;

        /// </ #BeginKazChange (26.11.2024): Updated chat message system to allow for an FChatMessage to specify the individual sending a message in a group chat>
        public TextMeshProUGUI authorText;
        /// </ #EndKazChange>
    }
}