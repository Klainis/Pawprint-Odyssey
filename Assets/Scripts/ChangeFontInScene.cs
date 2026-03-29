using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChangeFontInScene : MonoBehaviour
{
    [Header("Настройки")]
    public Font legacyFont;                 // Для обычного Unity UI Text
    public TMP_FontAsset tmpFontAsset;      // Для TextMeshPro
    public bool replaceLegacy = true;
    public bool replaceTMP = true;

    [ContextMenu("Execute: Заменить шрифты в сцене")]
    public void ReplaceFonts()
    {
        int countLegacy = 0;
        int countTMP = 0;

        // Заменяем Legacy Text
        if (replaceLegacy && legacyFont != null)
        {
            var texts = FindObjectsOfType<Text>(true);
            foreach (var t in texts)
            {
                t.font = legacyFont;
                countLegacy++;
            }
            Debug.Log($"[Legacy UI] Обновлено текстов: {countLegacy}");
        }

        // Заменяем TextMeshPro
        if (replaceTMP && tmpFontAsset != null)
        {
            var tmpTexts = FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (var t in tmpTexts)
            {
                t.font = tmpFontAsset;
                countTMP++;
            }
            Debug.Log($"[TextMeshPro] Обновлено текстов: {countTMP}");
        }

        if (countLegacy == 0 && countTMP == 0)
        {
            Debug.LogWarning("Ни одного текста не найдено или не выбран шрифт для замены!");
        }
    }
}