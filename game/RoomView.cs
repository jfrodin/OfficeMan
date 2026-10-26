using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OfficeMan.Core.Content;

namespace OfficeMan.Game;

/// <summary>
/// A walkable room. The room and its people are drawn unlit into a small viewport at the room's
/// native pixel size; the room shader then lights it with the room's lights, adds glow, bloom, fog,
/// a tone curve, vignette and grain, and shows it scaled up crisply.
/// </summary>
public partial class RoomView : Control
{
    private const float WalkSpeed = 74f;

    private SubViewport _viewport = null!;
    private TextureRect _screen = null!;
    private ShaderMaterial _material = null!;
    private Sprite2D _background = null!;
    private Sprite2D _front = null!;
    private Node2D _actors = null!;
    private Actor _teo = null!;
    private readonly List<(RoomLayout.RoomThing thing, Actor? actor)> _present = new();
    private RoomLayout _room = null!;
    private ArtManifest _art = null!;
    private Func<string, Texture2D> _texture = null!;
    private Texture2D _shadow = null!;
    private double _clock;
    private Smoke _smoke = null!;
    private ClockHands _hands = null!;
    private List<RoomLayout.RoomLight> _lights = new();
    private Vector4[] _lightPos = new Vector4[16];

    /// <summary>How drunk Teo is (0-4): he walks a little crooked.</summary>
    public int Drunk { get; set; }

    /// <summary>The time of day in minutes, for wall clocks.</summary>
    public int Minute
    {
        get => _hands.Minute;
        set { _hands.Minute = value; _hands.QueueRedraw(); }
    }

    /// <summary>When false, Teo stands still (conversations, menus).</summary>
    public bool Controllable { get; set; }

    public Vector2 TeoPosition => _teo.Position;

    public bool TeoVisible => _teo.Visible;

    private float _lift, _pan;

    /// <summary>How far the view is raised (screen pixels), to keep people clear of the dialogue bar.</summary>
    public float Lift
    {
        get => _lift;
        set { _lift = value; Place(); }
    }

    /// <summary>How far the view is slid left (screen pixels), to keep people clear of the text column.</summary>
    public float Pan
    {
        get => _pan;
        set { _pan = value; Place(); }
    }

    private void Place()
    {
        if (_screen is not null) _screen.Position = new Vector2(-Mathf.Round(_pan), -Mathf.Round(_lift));
    }

    /// <summary>Teo's horizontal position on screen (unpanned).</summary>
    public float TeoScreenX => _room is null ? 960 : _teo.Position.X * ScreenScale;

    private float ScreenScale => _room is null ? 4 : 1920f / _room.Width;

    /// <summary>Where someone's feet are on screen (unlifted): Teo when <paramref name="sprite"/> is null.</summary>
    public float? ScreenFeet(string? sprite)
    {
        if (_room is null) return null;
        if (sprite is null) return _teo.Position.Y * ScreenScale;
        foreach (var (thing, actor) in _present)
            if (actor is not null && thing.Sprite == sprite) return actor.Position.Y * ScreenScale;
        return null;
    }

    /// <summary>A thing's horizontal position on screen, by id or anchor.</summary>
    public float? ScreenX(string anchor)
    {
        foreach (var (thing, _) in _present)
            if (thing.Id == anchor || thing.AnchorId == anchor) return thing.X * ScreenScale;
        return null;
    }

    public void Init(ArtManifest art, Func<string, Texture2D> texture)
    {
        _art = art;
        _texture = texture;
        Size = new Vector2(1920, 1080);
        MouseFilter = MouseFilterEnum.Ignore;

        _viewport = new SubViewport
        {
            Size = new Vector2I(480, 270),
            TransparentBg = false,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
            CanvasItemDefaultTextureFilter = Viewport.DefaultCanvasItemTextureFilter.Nearest,
        };
        AddChild(_viewport);
        _background = new Sprite2D { Centered = false };
        _viewport.AddChild(_background);
        _actors = new Node2D { YSortEnabled = true };
        _viewport.AddChild(_actors);
        _front = new Sprite2D { Centered = false, ZIndex = 100 };
        _viewport.AddChild(_front);
        _smoke = new Smoke { ZIndex = 50 };
        _viewport.AddChild(_smoke);
        _hands = new ClockHands { ZIndex = 1 };
        _viewport.AddChild(_hands);

        _shadow = MakeShadow();
        _teo = new Actor(texture(art.Sprite("teo")), _shadow);
        _actors.AddChild(_teo);

        _material = new ShaderMaterial { Shader = GD.Load<Shader>("res://shaders/room.gdshader") };
        _screen = new TextureRect
        {
            Texture = _viewport.GetTexture(),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.Scale,
            TextureFilter = TextureFilterEnum.Nearest,
            Size = new Vector2(1920, 1080),
            Material = _material,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        AddChild(_screen);
    }

    public void Load(RoomLayout room, Func<string, bool> present)
    {
        _room = room;
        _viewport.Size = new Vector2I(room.Width, room.Height);
        _background.Texture = _texture(_art.Resolve(room.Image));
        _material.SetShaderParameter("base", _background.Texture);
        _front.Texture = room.Front is null ? null : _texture(_art.Resolve(room.Front));
        _teo.Position = new Vector2(room.Spawn[0], room.Spawn[1]);
        _teo.FaceLeft = false;
        _teo.Visible = room.ShowTeo;
        _hands.Visible = room.Clock is { Length: 2 };
        if (room.Clock is { Length: 2 } c) _hands.Position = new Vector2(c[0], c[1]);

        var black = BlackTexture();
        _material.SetShaderParameter("canvas_size", new Vector2(room.Width, room.Height));
        _material.SetShaderParameter("emissive", room.Emissive is null ? black : _texture(_art.Resolve(room.Emissive)));
        _material.SetShaderParameter("glow", room.Glow is null ? black : _texture(_art.Resolve(room.Glow)));
        _material.SetShaderParameter("ambient", new Vector3(room.Ambient[0], room.Ambient[1], room.Ambient[2]));
        var look = room.Look;
        _material.SetShaderParameter("bloom", look.Bloom);
        _material.SetShaderParameter("fog", look.Fog);
        var fog = Color.FromHtml(look.FogColor);
        _material.SetShaderParameter("fog_color", new Vector3(fog.R, fog.G, fog.B));
        _material.SetShaderParameter("exposure", look.Exposure);
        _material.SetShaderParameter("saturation", look.Saturation);
        _material.SetShaderParameter("vignette", look.Vignette);
        _material.SetShaderParameter("grain", look.Grain);
        Refresh(present);
    }

    /// <summary>Re-evaluates who is in the room and which lights are on (time and events change both).</summary>
    public void Refresh(Func<string, bool> present)
    {
        foreach (var (_, actor) in _present) actor?.QueueFree();
        _present.Clear();
        _smoke.Sources.Clear();
        foreach (var thing in _room.Things)
        {
            if (thing.When is not null && !present(thing.When)) continue;
            Actor? actor = null;
            if (thing.Sprite is not null)
            {
                actor = new Actor(_texture(_art.Sprite(thing.Sprite)), _shadow) { Position = new Vector2(thing.X, thing.Y), FaceLeft = thing.FaceLeft };
                _actors.AddChild(actor);
                if (thing.Smoke is { Length: 2 } sm)
                    _smoke.Sources.Add(new Vector2(thing.X + (thing.FaceLeft ? -sm[0] : sm[0]), thing.Y + sm[1]));
            }
            _present.Add((thing, actor));
        }

        // The time of day: ambient light and how strongly things glow.
        var ambient = _room.Ambient;
        float gain = 1;
        foreach (var step in _room.Timeline)
            if (present(step.When))
            {
                if (step.Ambient is { Length: 3 } a) ambient = a;
                gain = step.Emissive;
            }
        _material.SetShaderParameter("ambient", new Vector3(ambient[0], ambient[1], ambient[2]));
        _material.SetShaderParameter("emissive_gain", gain);

        var lights = _room.Lights.Where(l => l.When is null || present(l.When)).Take(16).ToList();
        _lights = lights;
        var pos = new Vector4[16];
        var col = new Vector4[16];
        var dir = new Vector4[16];
        for (int i = 0; i < lights.Count; i++)
        {
            var l = lights[i];
            var c = Color.FromHtml(l.Color);
            pos[i] = new Vector4(l.X, l.Y, l.Radius, l.Intensity);
            col[i] = new Vector4(c.R, c.G, c.B, 0);
            if (l.Dir is { Length: 2 } d)
            {
                var v = new Vector2(d[0], d[1]).Normalized();
                dir[i] = new Vector4(v.X, v.Y, Mathf.Cos(Mathf.DegToRad(l.Inner)), Mathf.Cos(Mathf.DegToRad(l.Outer)));
            }
            else dir[i] = new Vector4(0, 0, 0, -2);
        }
        _lightPos = pos;
        _material.SetShaderParameter("light_count", lights.Count);
        _material.SetShaderParameter("light_pos", pos);
        _material.SetShaderParameter("light_col", col);
        _material.SetShaderParameter("light_dir", dir);
    }

    /// <summary>The closest person or thing within reach, if any.</summary>
    public RoomLayout.RoomThing? Reachable()
    {
        var p = _teo.Position;
        RoomLayout.RoomThing? best = null;
        float bestDist = float.MaxValue;
        foreach (var (thing, _) in _present)
        {
            if (thing.Prop) continue;
            float dx = Math.Abs(p.X - thing.X), dy = Math.Abs(p.Y - (thing.Y + 10));
            bool inReach = thing.Wall ? dx < 16 && p.Y < _room.Walk.MinY + 26 : dx < 22 && dy < 18;
            if (!inReach) continue;
            float dist = dx + dy;
            if (dist < bestDist) { best = thing; bestDist = dist; }
        }
        return best;
    }

    /// <summary>Puts Teo next to a thing (used by automated play-throughs).</summary>
    public void StandNear(string anchor)
    {
        var t = _present.Select(x => x.thing).FirstOrDefault(x => x.AnchorId == anchor);
        if (t is null) return;
        float y = t.Wall ? _room.Walk.MinY + 4 : Math.Clamp(t.Y + 10, _room.Walk.MinY, _room.Walk.MaxY);
        _teo.Position = new Vector2(Math.Clamp(t.X - 18, _room.Walk.MinX, _room.Walk.MaxX), y);
        _teo.FaceLeft = false;
    }

    /// <summary>People turn to face Teo while he talks to them.</summary>
    public void FaceTeo(string anchor)
    {
        foreach (var (thing, actor) in _present)
            if (thing.AnchorId == anchor && actor is not null) actor.FaceLeft = actor.Position.X > _teo.Position.X;
    }

    public override void _Process(double delta)
    {
        if (_room is null) return;
        _clock += delta;
        Flicker();
        var dir = Vector2.Zero;
        if (Controllable)
        {
            dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            if (dir.LengthSquared() > 0.04f && Drunk > 0)
                dir += new Vector2((float)Math.Sin(_clock * 2.3) * 0.25f * Drunk, (float)Math.Cos(_clock * 1.7) * 0.12f * Drunk);
        }
        if (dir.LengthSquared() > 0.04f)
        {
            var next = _teo.Position + dir.LimitLength(1.4f) * WalkSpeed * (float)delta;
            next.X = Math.Clamp(next.X, _room.Walk.MinX, _room.Walk.MaxX);
            next.Y = Math.Clamp(next.Y, _room.Walk.MinY, _room.Walk.MaxY);
            if (Math.Abs(dir.X) > 0.1f) _teo.FaceLeft = dir.X < 0;
            _teo.Position = next;
            _teo.Walking = true;
        }
        else _teo.Walking = false;
    }

    /// <summary>Failing bulbs: mostly steady, sometimes a dip, now and then a stutter.</summary>
    private void Flicker()
    {
        bool any = false;
        var pos = (Vector4[])_lightPos.Clone();
        for (int i = 0; i < _lights.Count; i++)
        {
            if (!_lights[i].Flicker) continue;
            any = true;
            double t = _clock;
            double stutter = Math.Sin(t * 0.7) > 0.93 ? (Math.Sin(t * 47) > 0 ? 0.25 : 1) : 1;
            double hum = 0.94 + Math.Sin(t * 13.1) * 0.03 + Math.Sin(t * 7.3) * 0.03;
            pos[i].W = (float)(_lightPos[i].W * hum * stutter);
        }
        if (any) _material.SetShaderParameter("light_pos", pos);
    }

    private static Texture2D MakeShadow()
    {
        var img = Image.CreateEmpty(18, 5, false, Image.Format.Rgba8);
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 18; x++)
            {
                double dx = (x - 8.5) / 9.0, dy = (y - 2) / 2.5;
                double d = dx * dx + dy * dy;
                if (d < 1) img.SetPixel(x, y, new Color(0, 0, 0, (float)(0.45 * (1 - d * 0.6))));
            }
        return ImageTexture.CreateFromImage(img);
    }

    private static Texture2D BlackTexture()
    {
        var img = Image.CreateEmpty(4, 4, false, Image.Format.Rgb8);
        img.Fill(Colors.Black);
        return ImageTexture.CreateFromImage(img);
    }

    /// <summary>The hands of a wall clock, one pixel wide, showing the game's time.</summary>
    private partial class ClockHands : Node2D
    {
        public int Minute { get; set; }

        public override void _Draw()
        {
            var ink = new Color(0.1f, 0.1f, 0.12f);
            double m = Minute % 60, h = (Minute / 60.0) % 12;
            Hand(m / 60.0 * Math.Tau, 5.5f, ink);
            Hand(h / 12.0 * Math.Tau, 3.5f, ink);
            DrawRect(new Rect2(0, 0, 1, 1), ink);
        }

        private void Hand(double angle, float length, Color c)
        {
            // Step along the hand a pixel at a time, so it stays crisp at the room's native size.
            for (float r = 0; r <= length; r += 0.5f)
            {
                float x = Mathf.Floor((float)(Math.Sin(angle) * r) + 0.5f);
                float y = Mathf.Floor((float)(-Math.Cos(angle) * r) + 0.5f);
                DrawRect(new Rect2(x, y, 1, 1), c);
            }
        }
    }

    /// <summary>Cigarette smoke: single pixels rising, drifting and thinning out.</summary>
    private partial class Smoke : Node2D
    {
        private readonly List<(Vector2 pos, double age, double seed)> _puffs = new();
        private double _next;
        public List<Vector2> Sources { get; } = new();

        public override void _Process(double delta)
        {
            _next -= delta;
            if (_next <= 0 && Sources.Count > 0)
            {
                foreach (var s in Sources) _puffs.Add((s, 0, GD.Randf() * 10));
                _next = 0.18;
            }
            for (int i = _puffs.Count - 1; i >= 0; i--)
            {
                var (p, age, seed) = _puffs[i];
                age += delta;
                if (age > 3.2) { _puffs.RemoveAt(i); continue; }
                p += new Vector2((float)(Math.Sin(age * 1.7 + seed) * 4 * delta), (float)(-9 * delta));
                _puffs[i] = (p, age, seed);
            }
            QueueRedraw();
        }

        public override void _Draw()
        {
            foreach (var (p, age, _) in _puffs)
            {
                float a = (float)Math.Clamp(0.55 - age * 0.17, 0, 0.55);
                var c = new Color(0.86f, 0.85f, 0.82f, a);
                DrawRect(new Rect2(Mathf.Floor(p.X), Mathf.Floor(p.Y), 1, 1), c);
                if (age > 1.2) DrawRect(new Rect2(Mathf.Floor(p.X) + 1, Mathf.Floor(p.Y), 1, 1), c * new Color(1, 1, 1, 0.6f));
            }
        }
    }

    /// <summary>A person on the floor: a sprite sheet with idle (0-1) and walk (2-5) frames, anchored at the feet, with a soft shadow.</summary>
    private partial class Actor : Sprite2D
    {
        private double _t;
        private readonly double _offset = GD.Randf();

        public bool Walking { get; set; }

        public bool FaceLeft { get => FlipH; set => FlipH = value; }

        public Actor(Texture2D sheet, Texture2D shadow)
        {
            Texture = sheet;
            Hframes = 6;
            Centered = true;
            // Anchored at the feet whatever the frame size. Someone lying down (a frame wider than tall) casts no foot shadow.
            int frameW = sheet.GetWidth() / 6, frameH = sheet.GetHeight();
            Offset = new Vector2(0, -frameH / 2);
            TextureFilter = TextureFilterEnum.Nearest;
            if (frameH > frameW)
                AddChild(new Sprite2D { Texture = shadow, Position = new Vector2(0, 0), ShowBehindParent = true, TextureFilter = TextureFilterEnum.Nearest });
        }

        public override void _Process(double delta)
        {
            _t += delta;
            Frame = Walking ? 2 + (int)(_t * 8) % 4 : ((int)((_t + _offset) / 0.7) % 2);
        }
    }
}
