using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OfficeMan.Core.Content;

namespace OfficeMan.Game;

/// <summary>
/// A walkable room rendered at its native pixel size and scaled up crisply. Holds Teo and the people
/// and things present, moves Teo, and reports what he can reach.
/// </summary>
public partial class RoomView : SubViewportContainer
{
    private const float WalkSpeed = 74f;
    private const int Scale = 4;

    private SubViewport _viewport = null!;
    private Sprite2D _background = null!;
    private Node2D _actors = null!;
    private Actor _teo = null!;
    private readonly List<(RoomLayout.RoomThing thing, Actor? actor)> _present = new();
    private RoomLayout _room = null!;
    private ArtManifest _art = null!;
    private Func<string, Texture2D> _texture = null!;
    private double _clock;

    /// <summary>How drunk Teo is (0–4): he walks a little crooked.</summary>
    public int Drunk { get; set; }

    /// <summary>When false, Teo stands still (conversations, menus).</summary>
    public bool Controllable { get; set; }

    public Vector2 TeoPosition => _teo.Position;

    public void Init(ArtManifest art, Func<string, Texture2D> texture)
    {
        _art = art;
        _texture = texture;
        Stretch = true;
        StretchShrink = Scale;
        TextureFilter = TextureFilterEnum.Nearest;
        Size = new Vector2(1920, 1080);
        MouseFilter = MouseFilterEnum.Ignore;

        _viewport = new SubViewport { Size = new Vector2I(480, 270), CanvasItemDefaultTextureFilter = Viewport.DefaultCanvasItemTextureFilter.Nearest };
        AddChild(_viewport);
        _background = new Sprite2D { Centered = false };
        _viewport.AddChild(_background);
        _actors = new Node2D { YSortEnabled = true };
        _viewport.AddChild(_actors);
        _teo = new Actor(texture(art.Sprite("teo")));
        _actors.AddChild(_teo);
    }

    public void Load(RoomLayout room, Func<string, bool> present)
    {
        _room = room;
        _background.Texture = _texture(_art.Resolve(room.Image));
        _teo.Position = new Vector2(room.Spawn[0], room.Spawn[1]);
        _teo.FaceLeft = false;
        Refresh(present);
    }

    /// <summary>Re-evaluates who is in the room (time and events move people).</summary>
    public void Refresh(Func<string, bool> present)
    {
        foreach (var (_, actor) in _present) actor?.QueueFree();
        _present.Clear();
        foreach (var thing in _room.Things)
        {
            if (thing.When is not null && !present(thing.When)) continue;
            Actor? actor = null;
            if (thing.Sprite is not null)
            {
                actor = new Actor(_texture(_art.Sprite(thing.Sprite))) { Position = new Vector2(thing.X, thing.Y), FaceLeft = thing.FaceLeft };
                _actors.AddChild(actor);
            }
            _present.Add((thing, actor));
        }
    }

    /// <summary>The closest person or thing within reach, if any.</summary>
    public RoomLayout.RoomThing? Reachable()
    {
        var p = _teo.Position;
        RoomLayout.RoomThing? best = null;
        float bestDist = float.MaxValue;
        foreach (var (thing, _) in _present)
        {
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
        _teo.Position = new Vector2(Math.Clamp(t.X - 14, _room.Walk.MinX, _room.Walk.MaxX), y);
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
        _clock += delta;
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

    /// <summary>A person on the floor: a sprite sheet with idle (0–1) and walk (2–5) frames, anchored at the feet.</summary>
    private partial class Actor : Sprite2D
    {
        private double _t;
        private readonly double _offset = GD.Randf();

        public bool Walking { get; set; }

        public bool FaceLeft { get => FlipH; set => FlipH = value; }

        public Actor(Texture2D sheet)
        {
            Texture = sheet;
            Hframes = 6;
            Centered = true;
            Offset = new Vector2(0, -26);
            TextureFilter = TextureFilterEnum.Nearest;
        }

        public override void _Process(double delta)
        {
            _t += delta;
            Frame = Walking ? 2 + (int)(_t * 8) % 4 : ((int)((_t + _offset) / 0.7) % 2);
        }
    }
}
