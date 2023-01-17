using UnityEngine;

namespace Slides
{
    [CreateAssetMenu(fileName = "New Language State", menuName = "Language State", order = 48)]
    public class LanguageState : ScriptableObject
    {
        public LanguageToggle.Language language = default;
    }
}
