using UnityEngine;
using TMPro;

namespace Jigupa.UI
{
    public static class FontManager
    {
        private static TMP_FontAsset _lexendBlack;
        private static TMP_FontAsset _lexendBold;
        private static TMP_FontAsset _lexendRegular;
        private static TMP_FontAsset _lexendThin;
        
        public static TMP_FontAsset LexendBlack
        {
            get
            {
                if (_lexendBlack == null)
                {
                    _lexendBlack = Resources.Load<TMP_FontAsset>("Fonts/LexendDeca-Black SDF");
                    if (_lexendBlack == null)
                    {
                        Debug.LogWarning("LexendDeca-Black SDF font not found in Resources/Fonts/. Using default font.");
                    }
                }
                return _lexendBlack;
            }
        }
        
        public static TMP_FontAsset LexendBold
        {
            get
            {
                if (_lexendBold == null)
                {
                    _lexendBold = Resources.Load<TMP_FontAsset>("Fonts/LexendDeca-Bold SDF");
                    if (_lexendBold == null)
                    {
                        Debug.LogWarning("LexendDeca-Bold SDF font not found in Resources/Fonts/. Using default font.");
                    }
                }
                return _lexendBold;
            }
        }
        
        public static TMP_FontAsset LexendRegular
        {
            get
            {
                if (_lexendRegular == null)
                {
                    _lexendRegular = Resources.Load<TMP_FontAsset>("Fonts/LexendDeca-Regular SDF");
                    if (_lexendRegular == null)
                    {
                        Debug.LogWarning("LexendDeca-Regular SDF font not found in Resources/Fonts/. Using default font.");
                    }
                }
                return _lexendRegular;
            }
        }
        
        public static TMP_FontAsset LexendThin
        {
            get
            {
                if (_lexendThin == null)
                {
                    _lexendThin = Resources.Load<TMP_FontAsset>("Fonts/LexendDeca-Thin SDF");
                    if (_lexendThin == null)
                    {
                        Debug.LogWarning("LexendDeca-Thin SDF font not found in Resources/Fonts/. Using default font.");
                    }
                }
                return _lexendThin;
            }
        }
        
        public static void ApplyLexendFont(TextMeshProUGUI text, FontWeight weight = FontWeight.Regular)
        {
            switch (weight)
            {
                case FontWeight.Black:
                case FontWeight.Heavy:
                    if (LexendBlack != null) text.font = LexendBlack;
                    break;
                    
                case FontWeight.Bold:
                case FontWeight.SemiBold:
                    if (LexendBold != null) text.font = LexendBold;
                    break;
                    
                case FontWeight.Thin:
                case FontWeight.ExtraLight:
                    if (LexendThin != null) text.font = LexendThin;
                    break;
                    
                default:
                    if (LexendRegular != null) text.font = LexendRegular;
                    break;
            }
        }
    }
}