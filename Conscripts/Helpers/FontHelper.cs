using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SharpDX.DirectWrite;

namespace Conscripts.Helpers
{
    /// <summary>
    /// 参考自 https://edi.wang/post/2017/1/22/windows-10-uwp-get-fonts
    /// </summary>
    public static class FontHelper
    {
        private static List<Character> _allSegoeFluentIconsChar = new List<Character>();

        public static List<Character> GetAllSegoeFluentIcons()
        {
            try
            {
                if (_allSegoeFluentIconsChar.Count <= 0)
                {
                    var allFonts = InstalledFont.GetAllFonts();
                    var segoeFluentIconsFont = allFonts.First(t => t.Name == "Segoe Fluent Icons");
                    var allIcons = segoeFluentIconsFont.GetCharacters();
                    foreach (var character in allIcons)
                    {
                        if (!string.IsNullOrWhiteSpace(character.Char) && character.UnicodeIndex > 0)
                        {
                            // _allSegoeFluentIconsFont.Add(character.UnicodeIndex.ToString("X"));
                            _allSegoeFluentIconsChar.Add(character);
                        }
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }

            return _allSegoeFluentIconsChar;
        }
    }

    public class InstalledFont
    {
        public string Name { get; set; }

        public int FamilyIndex { get; set; }

        public int Index { get; set; }

        public static List<InstalledFont> GetAllFonts()
        {
            var fontList = new List<InstalledFont>();

            var factory = new Factory();
            var fontCollection = factory.GetSystemFontCollection(false);
            var familyCount = fontCollection.FontFamilyCount;

            for (int i = 0; i < familyCount; i++)
            {
                var fontFamily = fontCollection.GetFontFamily(i);
                var familyNames = fontFamily.FamilyNames;
                int index;

                if (!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out index))
                {
                    if (!familyNames.FindLocaleName("en-us", out index))
                    {
                        index = 0;
                    }
                }

                string name = familyNames.GetString(index);
                fontList.Add(new InstalledFont()
                {
                    Name = name,
                    FamilyIndex = i,
                    Index = index
                });
            }

            return fontList;
        }

        public List<Character> GetCharacters()
        {
            var factory = new Factory();
            var fontCollection = factory.GetSystemFontCollection(false);
            var fontFamily = fontCollection.GetFontFamily(FamilyIndex);

            var font = fontFamily.GetFont(Index);

            var characters = new List<Character>();
            var count = 65535;
            for (var i = 0; i < count; i++)
            {
                if (font.HasCharacter(i))
                {
                    characters.Add(new Character()
                    {
                        Char = char.ConvertFromUtf32(i),
                        UnicodeIndex = i
                    });
                }
            }

            return characters;
        }
    }

    public class Character
    {
        public string Char { get; set; }

        public int UnicodeIndex { get; set; }
    }
}
