using OfficeMan.ArtGen.Portraits;
using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Drawn.Flat;

/// <summary>
/// The cast in the flat illustration style chosen from art/reference/drawn_flat.png (proposal, 2026-10-18).
/// Magda is rendered by the reference code itself (MagdaDrawing + DrawnStyles.Flat), only with other
/// expressions, so she stays exactly the reference. The others are built with <see cref="FlatFace"/>
/// to the same rules. Moods match the names used in content/art.json.
/// </summary>
public static class FlatCast
{
    /// <summary>Magda at 21: a round young face, no lines, lighter make-up, loose curls instead of the big perm.</summary>
    public static readonly FlatSpec Magda = new()
    {
        Name = "magda",
        Skin = "f2c6a8", SkinShade = "d8957e", SkinDeep = "bd7766", SkinLight = "fbdcc6", Blush = "ea8c86", BlushAmount = 0.45,
        Crown = 190, Temple = 116, Cheek = 128, JawY = 494, Jaw = 92, ChinY = 530, Chin = 36, Neck = 62, Shoulders = 292,
        Age = 0, Bags = false, EyeWidth = 28, EyeSpacing = 46, Hood = 0.2, Iris = "6b4a2e", EyeShadow = "9fb4e0",
        BrowColor = "5a2430", BrowWidth = 4.2,
        NoseLength = 0.78, NoseWidth = 0.9, MouthWidth = 30, LipFullness = 0.9, LipUpper = "c4505a", LipLower = "d86a6a",
        Hair = FlatHair.Curls, HairColor = "8a2c42", HairShade = "5c1a2f", HairLight = "b6536a",
        Clothes = FlatClothes.Sweetheart, Cloth = "d0303d", ClothShade = "97192c",
        Earrings = true, Necklace = true, DoubleChin = true,
    };

    public static readonly FlatSpec Viktor = new()
    {
        Name = "viktor",
        Skin = "d9a080", SkinShade = "b87a5e", SkinDeep = "9a5e48", SkinLight = "ecc0a0", Blush = "d07a64", BlushAmount = 0.2,
        Crown = 178, Temple = 120, Cheek = 124, JawY = 500, Jaw = 94, ChinY = 534, Chin = 36, Neck = 68, Shoulders = 292,
        Age = 0.45, EyeWidth = 24, Hood = 0.65, Iris = "4a3a2a",
        BrowColor = "2e2420", BrowWidth = 6.5, BrowTilt = 0.3,
        NoseLength = 1.1, NoseWidth = 1.1, MouthWidth = 30, LipFullness = 0.45, LipUpper = "a86050", LipLower = "bc705c",
        Hair = FlatHair.SweptBack, HairColor = "3c3432", HairShade = "241e1c", HairLight = "5e5450", Grey = "a7a3a0",
        Stubble = 0.4,
        Clothes = FlatClothes.SuitOpenCollar, Cloth = "2c3546", ClothShade = "1c2230", Shirt = "ece9e4", ShirtShade = "c4c0ba",
    };

    public static readonly FlatSpec Zdenka = new()
    {
        Name = "zdenka",
        Skin = "ecd3c4", SkinShade = "c9a596", SkinDeep = "a8867a", SkinLight = "f6e4d8", Blush = "d89a90", BlushAmount = 0.12,
        Crown = 184, Temple = 104, Cheek = 102, JawY = 500, Jaw = 62, ChinY = 536, Chin = 20, Neck = 44, Shoulders = 228,
        Age = 0.85, EyeWidth = 23, Hood = 0.8, Iris = "5a6a7a",
        BrowColor = "8a8486", BrowWidth = 3.4,
        NoseLength = 1.15, NoseWidth = 0.85, MouthWidth = 25, LipFullness = 0.32, LipUpper = "a8706a", LipLower = "b8807a",
        Hair = FlatHair.Bun, HairColor = "a6a2a8", HairShade = "77737a", HairLight = "cac6cc",
        Glasses = FlatGlasses.Half, GlassesColor = "c9a65a",
        Clothes = FlatClothes.SuitBlouse, Cloth = "6c6e72", ClothShade = "4a4c50", Shirt = "ece7dc", ShirtShade = "c8c2b4", Brooch = "4a7a5a",
    };

    public static readonly FlatSpec Kornel = new()
    {
        Name = "kornel",
        Skin = "f0c9b2", SkinShade = "d29b86", SkinDeep = "b47e6c", SkinLight = "fae0d0", BlushAmount = 0.35,
        Crown = 172, Temple = 112, Cheek = 116, JawY = 494, Jaw = 70, ChinY = 528, Chin = 22, Neck = 50, Shoulders = 236,
        Age = 0.3, EyeWidth = 22, Hood = 0.3, Iris = "5a7a8a",
        BrowColor = "5a4434", BrowWidth = 4, BrowTilt = -0.6,
        NoseLength = 1.0, NoseWidth = 0.95, MouthWidth = 26, LipFullness = 0.5,
        Hair = FlatHair.Combover, HairColor = "5a4434", HairShade = "3a2c22", HairLight = "7a6250",
        Glasses = FlatGlasses.Round, GlassesColor = "3a2e28",
        Clothes = FlatClothes.ShirtTie, Shirt = "b9cde0", ShirtShade = "8ea6be", Tie = "8a6a3a",
    };

    public static readonly FlatSpec Stojan = new()
    {
        Name = "stojan",
        Skin = "d9a07e", SkinShade = "b4785c", SkinDeep = "8a5440", SkinLight = "eec0a0", Blush = "e07a6a", BlushAmount = 0.45,
        Crown = 168, Temple = 128, Cheek = 136, JawY = 506, Jaw = 100, ChinY = 540, Chin = 40, Neck = 82, Shoulders = 300,
        Age = 0.6, EyeWidth = 21, Hood = 0.7, Iris = "4a3a2a",
        BrowColor = "6a625a", BrowWidth = 7,
        NoseLength = 1.05, NoseWidth = 1.35, MouthWidth = 32, LipFullness = 0.4, LipUpper = "9a5a48", LipLower = "b06a56",
        Hair = FlatHair.BaldFringe, HairColor = "8a847c", HairShade = "5e5a54", HairLight = "aaa49c",
        Moustache = "6a5e54", Stubble = 0.25,
        Clothes = FlatClothes.Vest, Cloth = "ece8dc", ClothShade = "b8b2a2", ChestHair = true,
        Cigarette = true,
    };

    /// <summary>Every portrait in the set: (character, mood) → image.</summary>
    public static IEnumerable<(string name, string mood, Canvas image)> All()
    {
        foreach (var (mood, e) in new[]
        {
            ("happy", new Expression { Name = "happy", Smile = 1, MouthOpen = 0.5, LidOpen = 0.75, BrowRaise = 1 }),
            ("irritated", new Expression { Name = "irritated", Smile = -0.35, BrowInner = -0.8, LidOpen = 0.6, Lopsided = 0.4, LookX = 0.6 }),
            ("sad", new Expression { Name = "sad", Smile = -0.7, BrowInner = 1.0, LidOpen = 0.55, LookY = 0.7, Tears = true }),
        })
            yield return ("magda", mood, DrawnStyles.Flat(new MagdaDrawing(e)));

        var looks = new (FlatSpec spec, string mood, Expression e)[]
        {
            (Viktor, "neutral", new Expression { Smile = 0.35, Lopsided = 0.6, LidOpen = 0.85 }),
            (Viktor, "displeased", new Expression { Smile = -0.45, BrowInner = -0.9, LidOpen = 0.75, LookX = -0.4 }),
            (Zdenka, "neutral", new Expression { LidOpen = 0.8 }),
            (Zdenka, "smile", new Expression { Smile = 0.45, Lopsided = 0.2, LidOpen = 0.85 }),
            (Kornel, "neutral", new Expression { Smile = 0.15, BrowRaise = 0.6, BrowInner = 0.3 }),
            (Kornel, "overjoyed", new Expression { Smile = 1, MouthOpen = 0.6, BrowRaise = 1.2, BrowInner = 0.4, LidOpen = 0.9, Blush = 0.8 }),
            (Stojan, "neutral", new Expression { Smile = 0.45, LidOpen = 0.85 }),
            (Stojan, "laughing", new Expression { Smile = 1, MouthOpen = 0.8, LidOpen = 0.05, BrowRaise = 1, Blush = 0.6 }),
        };
        yield return ("magda", "young", FlatFace.Draw(Magda, new Expression { Smile = 0.7, MouthOpen = 0.18, LookX = -0.2, BrowRaise = 0.4 }));
        foreach (var (spec, mood, e) in looks) yield return (spec.Name, mood, FlatFace.Draw(spec, e));
    }
}
