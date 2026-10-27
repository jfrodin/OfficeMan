using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace OfficeMan.Game.Demo;

/// <summary>
/// Dev tool, not part of the game: the office built as a small 3D scene in code, rendered in one of several looks,
/// so the creator can choose the art direction by eye (design/art_direction.md).
///   godot --path game --rendering-method forward_plus res://demo/StyleDemo.tscn -- --style c1 --shot out.png
/// Styles: c1 flat colour and strong light; c2 two inks, dithered; c3 the same 3D rendered small, as pixels;
/// h as c1, but with the game's drawn people standing in the room instead of the simple figures; hp is h rendered
/// small like c3, so the room and the people share one pixel size; hf is h with the people redrawn at high resolution.
/// Everything is boxes, spheres and cylinders, placed by hand below; people are simple faceless figures.
/// Units are metres: x to the right, y up, z towards the camera; the back wall stands at z = 0.
/// </summary>
public partial class StyleDemo : Node
{
    private string _style = "c1";
    private string? _shot;
    private readonly Dictionary<Color, StandardMaterial3D> _materials = new();

    public override void _Ready()
    {
        var args = OS.GetCmdlineUserArgs();
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--style") _style = args[i + 1];
            if (args[i] == "--shot") _shot = args[i + 1];
        }
        int compare = Array.IndexOf(args, "--compare");
        if (compare >= 0)
        {
            Compare(args[(compare + 1)..].TakeWhile(a => !a.StartsWith("--")).ToArray());
            return;
        }

        var size = _style switch { "c3" or "hp" => new Vector2I(480, 270), "c2" => new Vector2I(640, 360), _ => new Vector2I(1920, 1080) };
        var vp = new SubViewport
        {
            Size = size, OwnWorld3D = true, RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
            Msaa3D = _style is "c1" or "h" or "hf" ? Viewport.Msaa.Msaa4X : Viewport.Msaa.Disabled,
        };
        AddChild(vp);
        var world = new Node3D();
        vp.AddChild(world);
        Build(world);

        var screen = new TextureRect
        {
            Texture = vp.GetTexture(), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.Scale, Size = new Vector2(1920, 1080),
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest,
        };
        if (_style is "c2" or "c3" or "hp") screen.Material = new ShaderMaterial { Shader = new Shader { Code = _style == "c2" ? DitherShader : PixelShader } };
        var layer = new CanvasLayer();
        layer.AddChild(screen);
        AddChild(layer);

        if (_shot is not null)
            GetTree().CreateTimer(2.0).Timeout += () =>
            {
                GetViewport().GetTexture().GetImage().SavePng(_shot);
                GetTree().Quit();
            };
    }

    /// <summary>
    /// Up to four finished pictures in a grid, numbered 1-4, for choosing between them:
    ///   ... res://demo/StyleDemo.tscn -- --compare a.png h.png hp.png hf.png --shot sheet.png
    /// </summary>
    private void Compare(string[] pictures)
    {
        var screen = GetViewport().GetVisibleRect().Size;
        const float gap = 12;
        float w = (screen.X - 3 * gap) / 2, h = (screen.Y - 3 * gap) / 2;
        var layer = new CanvasLayer();
        AddChild(layer);
        layer.AddChild(new ColorRect { Color = new Color("141416"), Size = screen });
        for (int i = 0; i < pictures.Length && i < 4; i++)
        {
            var at = new Vector2(gap + i % 2 * (w + gap), gap + i / 2 * (h + gap));
            layer.AddChild(new TextureRect
            {
                // Expand mode before size, or the size is clamped to the picture's own.
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.Scale,
                Texture = ImageTexture.CreateFromImage(Image.LoadFromFile(pictures[i])), Position = at, Size = new Vector2(w, h),
            });
            var badge = new PanelContainer { Position = at + new Vector2(14, 14) };
            badge.AddThemeStyleboxOverride("panel", new StyleBoxFlat
            {
                BgColor = new Color(0.05f, 0.05f, 0.06f, 0.85f), CornerRadiusTopLeft = 10, CornerRadiusTopRight = 10,
                CornerRadiusBottomLeft = 10, CornerRadiusBottomRight = 10, ContentMarginLeft = 22, ContentMarginRight = 22,
            });
            var number = new Label { Text = (i + 1).ToString() };
            number.AddThemeFontSizeOverride("font_size", 64);
            number.AddThemeColorOverride("font_color", new Color("f2e6cc"));
            badge.AddChild(number);
            layer.AddChild(badge);
        }
        if (_shot is not null)
            GetTree().CreateTimer(1.0).Timeout += () =>
            {
                GetViewport().GetTexture().GetImage().SavePng(_shot);
                GetTree().Quit();
            };
    }

    // ── The room ──────────────────────────────────────────

    private static readonly Color Plaster = new("c9b58c"), Panel = new("5a3e2a"), Lino = new("56675f"), Wood = new("9a7252"),
        DarkWood = new("4e392a"), Metal = new("8e949a"), DarkMetal = new("393d42"), Fabric = new("2c303a"), Paper = new("e8e2cc"),
        GreenMetal = new("5b6a5e"), Red = new("b8284a"), Blue = new("2e58b4"), Skin = new("d8b090"), Tan = new("c08a60");

    private void Build(Node3D w)
    {
        // Light: the morning sun through the windows behind, low, so the window frames and the blinds lay stripes
        // across the floor; fluorescent tubes as a cool fill; screens and a desk lamp as small practical lights.
        var env = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Color, BackgroundColor = new Color("201c1a"),
            AmbientLightSource = Godot.Environment.AmbientSource.Color, AmbientLightColor = new Color("8a90a0"), AmbientLightEnergy = 0.42f,
            TonemapMode = Godot.Environment.ToneMapper.Filmic, TonemapExposure = 1.15f,
            GlowEnabled = true, GlowIntensity = 0.5f, GlowBloom = 0.05f,
            FogEnabled = true, FogLightColor = new Color("c4ae8c"), FogDensity = 0.004f,
            SsaoEnabled = _style is "c1" or "h" or "hf", SsaoIntensity = 1.6f,
        };
        w.AddChild(new WorldEnvironment { Environment = env });

        var sun = new DirectionalLight3D { LightColor = new Color("ffd9a8"), LightEnergy = 3.2f, ShadowEnabled = true, ShadowBlur = 0.2f };
        w.AddChild(sun);
        sun.LookAtFromPosition(new Vector3(2, 5, -6), new Vector3(6.5f, 0, 6), Vector3.Up);
        foreach (float x in new[] { 3f, 7f, 11f })
            Omni(w, new Vector3(x, 3.2f, 3.5f), new Color("d8e6ff"), 0.25f, 7);

        var cam = new Camera3D { Fov = 32, Current = true };
        w.AddChild(cam);
        cam.Fov = 30;
        cam.LookAtFromPosition(Camera, new Vector3(6.7f, 0.8f, 2.2f), Vector3.Up);

        Shell(w);
        Outside(w);
        WallThings(w);
        ViktorsOffice(w);

        // Desks out on the floor: Magda's and Zdenka's at the back, Teo's and Kornel's pushed together in front.
        Desk(w, 3.0f, 1.9f, monitor: false);
        Desk(w, 5.25f, 4.3f, monitor: true);
        Desk(w, 6.9f, 4.3f, monitor: true);
        Desk(w, 8.7f, 1.9f, monitor: false);
        MagdasThings(w, 3.0f, 1.9f);
        ZdenkasLamp(w, 9.25f, 1.9f);
        Trolley(w, 9.6f, 4.8f);

        if (_style is "h" or "hp" or "hf")
        {
            // Hybrid: the game's own drawn people, standing in the 3D room and lit by it.
            Figure(w, "magda", new Vector3(2.8f, 0, 1.25f));
            Figure(w, _style == "hf" ? "zdenka" : "zdenka_sit", new Vector3(8.45f, 0, 1.45f));
            Figure(w, "kornel", new Vector3(7.3f, 0, 3.55f));
            Figure(w, "teo", new Vector3(5.9f, 0, 6.6f));
            Figure(w, "viktor", new Vector3(11.9f, 0, 1.6f));
        }
        else
        {
            Person(w, new Vector3(2.8f, 0, 1.25f), shirt: new Color("e070a0"), legs: new Color("2a2030"), hair: new Color("c83050"), bigHair: true, skirt: true);
            Person(w, new Vector3(8.95f, 0, 1.25f), shirt: new Color("8a8c88"), legs: new Color("6a6c68"), hair: new Color("c0c0bc"), bun: true, skirt: true);
            Person(w, new Vector3(7.3f, 0, 3.55f), shirt: new Color("cfcab4"), legs: new Color("a08a68"), hair: new Color("9a7a50"), glasses: true);
            Person(w, new Vector3(4.3f, 0, 5.6f), shirt: new Color("e4e2dc"), legs: new Color("2a3040"), hair: new Color("2e2018"));
            Person(w, new Vector3(12.6f, 0, 1.4f), shirt: new Color("2a3050"), legs: new Color("2a3050"), hair: new Color("3a2a1c"), skin: Tan);
        }

        Foreground(w);
    }

    private void Shell(Node3D w)
    {
        Box(w, new Vector3(7, -0.05f, 5), new Vector3(16, 0.1f, 12), Lino);
        // Faint tile seams, one material darker, every 60 cm.
        for (float z = 0.6f; z < 11; z += 0.6f) Box(w, new Vector3(7, 0.002f, z), new Vector3(16, 0.004f, 0.02f), Lino.Darkened(0.12f));
        for (float x = 0; x < 15; x += 0.6f) Box(w, new Vector3(x, 0.002f, 5), new Vector3(0.02f, 0.004f, 12), Lino.Darkened(0.12f));

        // The back wall, with three window openings the sun comes through.
        const float wallH = 3.4f, sill = 1.25f, head = 2.85f;
        float[][] windows = { new[] { 1.8f, 3.6f }, new[] { 4.4f, 6.2f }, new[] { 7.0f, 8.8f } };
        Box(w, new Vector3(5.25f, sill / 2, -0.1f), new Vector3(10.5f, sill, 0.2f), Plaster);
        Box(w, new Vector3(5.25f, (head + wallH) / 2, -0.1f), new Vector3(10.5f, wallH - head, 0.2f), Plaster);
        float left = 0;
        foreach (var win in windows)
        {
            Box(w, new Vector3((left + win[0]) / 2, (sill + head) / 2, -0.1f), new Vector3(win[0] - left, head - sill, 0.2f), Plaster);
            left = win[1];
            // Frame, a mullion, glass, and venetian blinds half down.
            float cx = (win[0] + win[1]) / 2, ww = win[1] - win[0];
            Box(w, new Vector3(cx, sill - 0.02f, 0.02f), new Vector3(ww + 0.1f, 0.05f, 0.18f), Metal);
            Box(w, new Vector3(cx, (sill + head) / 2, -0.1f), new Vector3(0.06f, head - sill, 0.1f), Metal);
            Glass(w, new Vector3(cx, (sill + head) / 2, -0.12f), new Vector3(ww, head - sill, 0.01f));
            for (float y = 2.05f; y < head; y += 0.09f)
                Box(w, new Vector3(cx, y, -0.02f), new Vector3(ww, 0.035f, 0.06f), new Color("d2cbb2"));
        }
        Box(w, new Vector3((left + 10.5f) / 2, (sill + head) / 2, -0.1f), new Vector3(10.5f - left, head - sill, 0.2f), Plaster);
        // Wood panelling along the bottom of the wall, and a rail.
        Box(w, new Vector3(5.25f, 0.5f, 0.01f), new Vector3(10.5f, 1.0f, 0.02f), Panel);
        Box(w, new Vector3(5.25f, 1.02f, 0.03f), new Vector3(10.5f, 0.05f, 0.06f), DarkWood);

        // The left wall and the ceiling with its tubes.
        Box(w, new Vector3(-0.1f, wallH / 2, 5), new Vector3(0.2f, wallH, 12), Plaster.Darkened(0.1f));
        Box(w, new Vector3(7, wallH + 0.05f, 5), new Vector3(16, 0.1f, 12), new Color("3a3632"));
        foreach (float x in new[] { 2.5f, 6.5f, 10.5f })
            for (float z = 1.5f; z < 9; z += 3)
                Box(w, new Vector3(x, wallH - 0.04f, z), new Vector3(1.4f, 0.06f, 0.2f), new Color("eef4ff"), emit: 0.9f);
    }

    private void Outside(Node3D w)
    {
        // The block opposite and a pale morning sky, seen through the windows.
        Box(w, new Vector3(6, 6, -14), new Vector3(40, 14, 0.1f), new Color("c8d4dc"), emit: 0.55f).CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        var rng = new Random(4);
        for (float x = -2; x < 14; x += 3.2f)
        {
            float h = 3 + (float)rng.NextDouble() * 4;
            Box(w, new Vector3(x, h / 2, -9), new Vector3(2.8f, h, 2), new Color("6a6870")).CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        }
    }

    private void WallThings(Node3D w)
    {
        // The door, its EXIT sign, the filing cabinet, the water cooler.
        Box(w, new Vector3(0.95f, 1.05f, 0.03f), new Vector3(0.9f, 2.1f, 0.06f), Wood);
        Box(w, new Vector3(0.95f, 2.3f, 0.05f), new Vector3(0.4f, 0.14f, 0.04f), new Color("40d070"), emit: 2);
        Box(w, new Vector3(1.55f, 0.65f, 0.25f), new Vector3(0.45f, 1.3f, 0.5f), GreenMetal);
        Box(w, new Vector3(1.55f, 1.33f, 0.25f), new Vector3(0.4f, 0.06f, 0.4f), Paper);
        Box(w, new Vector3(4.0f, 0.55f, 0.2f), new Vector3(0.3f, 1.1f, 0.3f), Paper);
        Sphere(w, new Vector3(4.0f, 1.25f, 0.2f), 0.14f, new Color("7aaad0"));

        // Low shelves of lever-arch files under the windows, every permit since 1994.
        Box(w, new Vector3(5.6f, 0.45f, 0.2f), new Vector3(4.2f, 0.9f, 0.4f), DarkWood);
        Color[] spines = { Blue, GreenMetal, Paper, Red };
        int k = 0;
        for (float x = 3.6f; x < 7.6f; x += 0.08f, k++)
            for (int row = 0; row < 2; row++)
                Box(w, new Vector3(x, 0.24f + row * 0.42f, 0.41f), new Vector3(0.065f, 0.32f, 0.02f), spines[(k * 7 + row * 3) % 4]);

        // Noticeboard, the EU poster, the clock.
        Box(w, new Vector3(9.4f, 1.9f, 0.02f), new Vector3(0.85f, 0.95f, 0.03f), new Color("96724e"));
        foreach (var (x, y) in new[] { (9.2f, 2.1f), (9.55f, 2.15f), (9.3f, 1.7f), (9.6f, 1.75f) })
            Box(w, new Vector3(x, y, 0.04f), new Vector3(0.22f, 0.26f, 0.01f), Paper);
        Box(w, new Vector3(10.15f, 1.95f, 0.02f), new Vector3(0.5f, 0.75f, 0.02f), new Color("5c727c"));
        Cylinder(w, new Vector3(9.9f, 2.75f, 0.03f), 0.16f, 0.03f, Paper, rotX: 90);
    }

    private void ViktorsOffice(Node3D w)
    {
        // Behind glass at the right: his own window, the good desk, the executive chair, the minister photo.
        Box(w, new Vector3(12.7f, 1.7f, -0.1f), new Vector3(4.6f, 3.4f, 0.2f), Plaster.Lightened(0.08f));
        Box(w, new Vector3(13, 1.9f, -0.12f), new Vector3(1.6f, 1.6f, 0.05f), new Color("dfe8ee"), emit: 0.8f);
        Box(w, new Vector3(11.6f, 1.9f, 0.02f), new Vector3(0.5f, 0.4f, 0.03f), DarkWood);
        Box(w, new Vector3(12.9f, 0.4f, 0.9f), new Vector3(1.8f, 0.8f, 0.8f), Wood.Darkened(0.2f));
        Box(w, new Vector3(13.3f, 0.75f, 0.35f), new Vector3(0.6f, 1.5f, 0.5f), Fabric);
        // The glass wall, with the door always open.
        Glass(w, new Vector3(10.5f, 1.7f, 1.6f), new Vector3(0.02f, 3.4f, 3.2f));
        Glass(w, new Vector3(12.9f, 1.7f, 3.2f), new Vector3(3.2f, 3.4f, 0.02f));
        foreach (var p in new[] { new Vector3(10.5f, 1.7f, 3.2f), new Vector3(11.3f, 1.7f, 3.2f), new Vector3(14.5f, 1.7f, 3.2f), new Vector3(10.5f, 1.7f, 0.05f) })
            Box(w, p, new Vector3(0.06f, 3.4f, 0.06f), Metal);
    }

    private void Desk(Node3D w, float x, float z, bool monitor)
    {
        Box(w, new Vector3(x, 0.74f, z), new Vector3(1.6f, 0.04f, 0.8f), Wood);
        Box(w, new Vector3(x, 0.42f, z + 0.36f), new Vector3(1.5f, 0.6f, 0.03f), Wood.Darkened(0.15f));
        foreach (float dx in new[] { -0.76f, 0.76f })
            Box(w, new Vector3(x + dx, 0.36f, z), new Vector3(0.04f, 0.72f, 0.76f), DarkMetal);
        // The chair, pushed in behind.
        Box(w, new Vector3(x + 0.45f, 0.48f, z - 0.62f), new Vector3(0.48f, 0.08f, 0.45f), Fabric);
        Box(w, new Vector3(x + 0.45f, 0.82f, z - 0.86f), new Vector3(0.44f, 0.6f, 0.06f), Fabric);
        Cylinder(w, new Vector3(x + 0.45f, 0.24f, z - 0.62f), 0.03f, 0.44f, DarkMetal);
        if (monitor)
        {
            // Its back to us; the screen lights whoever sits there.
            Box(w, new Vector3(x - 0.3f, 1.0f, z - 0.15f), new Vector3(0.55f, 0.34f, 0.04f), Fabric);
            Box(w, new Vector3(x - 0.3f, 1.0f, z - 0.175f), new Vector3(0.5f, 0.3f, 0.01f), new Color("9fd0ff"), emit: 2);
            Box(w, new Vector3(x - 0.3f, 0.8f, z - 0.15f), new Vector3(0.06f, 0.12f, 0.06f), Fabric);
            Omni(w, new Vector3(x - 0.3f, 1.0f, z - 0.4f), new Color("9fd0ff"), 0.6f, 1.2f);
            Box(w, new Vector3(x + 0.35f, 0.79f, z + 0.1f), new Vector3(0.3f, 0.06f, 0.22f), Paper);
        }
    }

    private void MagdasThings(Node3D w, float x, float z)
    {
        // Balloons tied to the GIFT tin, a photo frame, a mug.
        Box(w, new Vector3(x + 0.4f, 0.82f, z + 0.1f), new Vector3(0.2f, 0.12f, 0.14f), Metal);
        Box(w, new Vector3(x - 0.5f, 0.85f, z), new Vector3(0.16f, 0.2f, 0.03f), DarkWood);
        Cylinder(w, new Vector3(x - 0.2f, 0.8f, z + 0.15f), 0.05f, 0.1f, Paper);
        foreach (var (dx, y, dz, c) in new[] { (0.2f, 2.0f, 0f, Red), (0.45f, 2.25f, -0.1f, Blue), (0.65f, 2.05f, 0.1f, Red), (0.5f, 1.85f, 0.05f, Blue) })
        {
            Sphere(w, new Vector3(x + dx, y, z + dz), 0.15f, c);
            var a = new Vector3(x + 0.4f, 0.88f, z + 0.1f);
            var b = new Vector3(x + dx, y - 0.15f, z + dz);
            var mid = (a + b) / 2;
            var mi = Box(w, mid, new Vector3(0.008f, (b - a).Length(), 0.008f), Paper);
            mi.LookAtFromPosition(mid, b, Vector3.Forward);
            mi.RotateObjectLocal(Vector3.Right, Mathf.Pi / 2);
        }
    }

    private void ZdenkasLamp(Node3D w, float x, float z)
    {
        Box(w, new Vector3(x - 0.9f, 0.77f, z), new Vector3(0.32f, 0.02f, 0.24f), Paper);
        Cylinder(w, new Vector3(x, 0.98f, z - 0.1f), 0.015f, 0.42f, Metal);
        Cylinder(w, new Vector3(x - 0.06f, 1.2f, z - 0.1f), 0.12f, 0.12f, GreenMetal, top: 0.04f);
        Omni(w, new Vector3(x - 0.08f, 1.05f, z - 0.1f), new Color("ffc878"), 1.4f, 2.2f);
    }

    private void Trolley(Node3D w, float x, float z)
    {
        for (int i = 0; i < 3; i++)
        {
            Box(w, new Vector3(x, 0.25f + i * 0.3f, z), new Vector3(0.7f, 0.03f, 0.45f), Metal);
            Box(w, new Vector3(x, 0.35f + i * 0.3f, z), new Vector3(0.62f, 0.16f, 0.4f), Paper.Darkened(0.08f * i));
        }
        foreach (float dx in new[] { -0.33f, 0.33f })
            Box(w, new Vector3(x + dx, 0.5f, z + 0.2f), new Vector3(0.03f, 1.0f, 0.03f), Metal);
    }

    /// <summary>
    /// A drawn person from the game (art/game/sprites, first idle frame of the sheet), as a flat figure that always
    /// faces the camera, stands on the floor, catches the room's light and casts a shadow.
    /// </summary>
    private void Figure(Node3D w, string sprite, Vector3 at)
    {
        // hf: the people redrawn at high resolution (ArtGen "people", art/out/people/): one frame, 175 units a metre, feet 10 up.
        bool flat = _style == "hf";
        string path = flat
            ? System.IO.Path.Combine(ProjectSettings.GlobalizePath("res://"), "..", "art", "out", "people", sprite + ".png")
            : System.IO.Path.Combine(ProjectSettings.GlobalizePath("res://"), "..", "art", "game", "sprites", sprite + ".png");
        var sheet = ImageTexture.CreateFromImage(Image.LoadFromFile(path));
        int frames = flat ? 1 : 6;
        float pixel = flat ? 1.75f / 310 : 1.78f / 52, below = flat ? 10 * pixel : 0;
        var size = new Vector2(sheet.GetWidth() / frames, sheet.GetHeight()) * pixel;
        var m = new StandardMaterial3D
        {
            AlbedoTexture = sheet, Uv1Scale = new Vector3(1f / frames, 1, 1),
            Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor, AlphaScissorThreshold = 0.5f,
            TextureFilter = flat ? BaseMaterial3D.TextureFilterEnum.LinearWithMipmaps : BaseMaterial3D.TextureFilterEnum.Nearest, CullMode = BaseMaterial3D.CullModeEnum.Disabled,
            Roughness = 1, DiffuseMode = BaseMaterial3D.DiffuseModeEnum.Toon, SpecularMode = BaseMaterial3D.SpecularModeEnum.Disabled,
        };
        var s = new MeshInstance3D
        {
            Mesh = new QuadMesh { Size = size, CenterOffset = new Vector3(0, size.Y / 2 - below, 0) }, MaterialOverride = m, Position = at,
        };
        w.AddChild(s);
        // A soft shadow under the feet, as in the game today, so the figure stands on the floor.
        var blob = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.05f, 0.04f, 0.04f, 0.45f), Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        };
        w.AddChild(new MeshInstance3D
        {
            Mesh = new CylinderMesh { TopRadius = 0.32f, BottomRadius = 0.32f, Height = 0.005f }, MaterialOverride = blob,
            Position = at + new Vector3(0, 0.004f, 0), Scale = new Vector3(1, 1, 0.6f), CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        });
        // Turned to face the camera once, not billboarded: a billboard also turns to face the sun when shadows
        // are drawn, and then casts none.
        s.LookAt(new Vector3(Camera.X, at.Y, Camera.Z), Vector3.Up, useModelFront: true);
    }

    private static readonly Vector3 Camera = new(6.9f, 2.6f, 13.5f);

    /// <summary>A simple faceless figure, about 1.75 m: tapered legs and body, arms, head, hair.</summary>
    private void Person(Node3D w, Vector3 at, Color shirt, Color legs, Color hair, bool bigHair = false, bool bun = false,
        bool skirt = false, bool glasses = false, Color? skin = null)
    {
        var s = skin ?? Skin;
        if (skirt)
        {
            Cylinder(w, at + new Vector3(0, 0.74f, 0), 0.21f, 0.42f, legs, top: 0.15f);
            foreach (float dx in new[] { -0.07f, 0.07f }) Cylinder(w, at + new Vector3(dx, 0.27f, 0), 0.045f, 0.54f, s.Darkened(0.1f), top: 0.06f);
        }
        else
            foreach (float dx in new[] { -0.085f, 0.085f }) Cylinder(w, at + new Vector3(dx, 0.46f, 0), 0.06f, 0.92f, legs, top: 0.085f);
        Cylinder(w, at + new Vector3(0, 1.22f, 0), 0.17f, 0.62f, shirt, top: 0.22f);
        foreach (float dx in new[] { -0.25f, 0.25f }) Cylinder(w, at + new Vector3(dx, 1.2f, 0), 0.04f, 0.58f, shirt.Darkened(0.08f), top: 0.055f);
        Cylinder(w, at + new Vector3(0, 1.56f, 0), 0.05f, 0.1f, s);
        Sphere(w, at + new Vector3(0, 1.68f, 0), 0.115f, s);
        if (bigHair) Sphere(w, at + new Vector3(0, 1.73f, -0.03f), 0.17f, hair);
        else Sphere(w, at + new Vector3(0, 1.73f, -0.03f), 0.12f, hair, squash: 0.75f);
        if (bun) Sphere(w, at + new Vector3(0, 1.83f, -0.08f), 0.07f, hair);
        if (glasses) Box(w, at + new Vector3(0, 1.69f, 0.11f), new Vector3(0.19f, 0.03f, 0.01f), Fabric);
    }

    private void Foreground(Node3D w)
    {
        // A big office plant at the front left, something to look past.
        Cylinder(w, new Vector3(2.3f, 0.3f, 9.2f), 0.28f, 0.6f, new Color("6e422a"), top: 0.32f);
        var rng = new Random(7);
        for (int i = 0; i < 22; i++)
        {
            float a = (float)(rng.NextDouble() * Math.Tau), tilt = 25 + (float)rng.NextDouble() * 45;
            var leaf = Box(w, new Vector3(2.3f, 0.95f, 9.2f), new Vector3(0.14f, 0.9f, 0.02f), new Color("2e4824").Lightened((float)rng.NextDouble() * 0.15f));
            leaf.RotationDegrees = new Vector3(tilt * MathF.Cos(a), a * 57.3f, tilt * MathF.Sin(a));
            leaf.Position += leaf.Basis.Y * 0.4f;
        }
    }

    // ── Helpers ───────────────────────────────────────────

    private StandardMaterial3D Mat(Color c, float emit = 0)
    {
        if (emit == 0 && _materials.TryGetValue(c, out var m)) return m;
        m = new StandardMaterial3D
        {
            AlbedoColor = c, Roughness = 1,
            DiffuseMode = BaseMaterial3D.DiffuseModeEnum.Toon, SpecularMode = BaseMaterial3D.SpecularModeEnum.Disabled,
        };
        if (emit > 0) { m.EmissionEnabled = true; m.Emission = c; m.EmissionEnergyMultiplier = emit; }
        else _materials[c] = m;
        return m;
    }

    private MeshInstance3D Box(Node3D w, Vector3 at, Vector3 size, Color c, float emit = 0)
    {
        var mi = new MeshInstance3D { Mesh = new BoxMesh { Size = size }, MaterialOverride = Mat(c, emit), Position = at };
        w.AddChild(mi);
        return mi;
    }

    private void Sphere(Node3D w, Vector3 at, float r, Color c, float squash = 1)
    {
        w.AddChild(new MeshInstance3D { Mesh = new SphereMesh { Radius = r, Height = r * 2 * squash }, MaterialOverride = Mat(c), Position = at });
    }

    private void Cylinder(Node3D w, Vector3 at, float r, float h, Color c, float? top = null, float rotX = 0)
    {
        w.AddChild(new MeshInstance3D
        {
            Mesh = new CylinderMesh { BottomRadius = r, TopRadius = top ?? r, Height = h }, MaterialOverride = Mat(c),
            Position = at, RotationDegrees = new Vector3(rotX, 0, 0),
        });
    }

    private static void Glass(Node3D w, Vector3 at, Vector3 size)
    {
        var m = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.75f, 0.85f, 0.9f, 0.1f), Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            Roughness = 0.2f, ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        };
        w.AddChild(new MeshInstance3D { Mesh = new BoxMesh { Size = size }, MaterialOverride = m, Position = at, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off });
    }

    private static void Omni(Node3D w, Vector3 at, Color c, float energy, float range)
    {
        w.AddChild(new OmniLight3D { Position = at, LightColor = c, LightEnergy = energy, OmniRange = range });
    }

    // ── Looks ─────────────────────────────────────────────

    // c2: two inks, an ordered dither between them, like a printed page.
    private const string DitherShader = @"shader_type canvas_item;
uniform vec3 ink = vec3(0.12, 0.10, 0.14);
uniform vec3 paper = vec3(0.91, 0.86, 0.74);
float bayer(vec2 p) {
    int x = int(mod(p.x, 4.0)), y = int(mod(p.y, 4.0));
    int m[16] = int[16](0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5);
    return (float(m[y * 4 + x]) + 0.5) / 16.0;
}
void fragment() {
    vec3 c = texture(TEXTURE, UV).rgb;
    float l = dot(c, vec3(0.299, 0.587, 0.114));
    l = smoothstep(0.12, 0.62, l);
    vec2 px = floor(UV / TEXTURE_PIXEL_SIZE);
    COLOR = vec4(l > bayer(px) ? paper : ink, 1.0);
}";

    // c3: the 3D rendered small and shown as big pixels, each colour channel on a short ramp.
    private const string PixelShader = @"shader_type canvas_item;
float bayer(vec2 p) {
    int x = int(mod(p.x, 4.0)), y = int(mod(p.y, 4.0));
    int m[16] = int[16](0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5);
    return (float(m[y * 4 + x]) + 0.5) / 16.0 - 0.5;
}
void fragment() {
    vec3 c = texture(TEXTURE, UV).rgb;
    vec2 px = floor(UV / TEXTURE_PIXEL_SIZE);
    c = floor(c * 14.0 + 0.5 + bayer(px) * 0.6) / 14.0;
    COLOR = vec4(c, 1.0);
}";
}
