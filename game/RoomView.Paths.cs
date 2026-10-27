using System;
using System.Collections.Generic;
using Godot;

namespace OfficeMan.Game;

/// <summary>
/// Finding the way round furniture when the player clicks somewhere: a grid over the floor, the furniture's
/// footprints grown by half a body so Teo doesn't brush through them, A* across it, then the corners cut
/// wherever he can walk straight.
/// </summary>
public partial class RoomView
{
    private const int Cell = 4;
    /// <summary>How far Teo keeps from furniture: about half his width at the feet.</summary>
    private const float Clearance = 5;

    private readonly List<Vector2> _path = new();

    private bool BlockedWithRoom(Vector2 p) =>
        _blocks.Exists(r => p.X > r.Position.X - Clearance && p.X < r.End.X + Clearance
                         && p.Y > r.Position.Y - 2 && p.Y <= r.End.Y + 3);

    /// <summary>The way from Teo to <paramref name="goal"/>, as points to walk through; empty if there's none.</summary>
    private List<Vector2> FindPath(Vector2 from, Vector2 goal)
    {
        var walk = _room.Walk;
        int cols = (walk.MaxX - walk.MinX) / Cell + 1, rows = (walk.MaxY - walk.MinY) / Cell + 1;
        Vector2 At(int c, int r) => new(walk.MinX + c * Cell, walk.MinY + r * Cell);
        (int c, int r) CellOf(Vector2 p) =>
            (Math.Clamp((int)Math.Round((p.X - walk.MinX) / Cell), 0, cols - 1), Math.Clamp((int)Math.Round((p.Y - walk.MinY) / Cell), 0, rows - 1));

        var free = new bool[cols, rows];
        for (int c = 0; c < cols; c++)
            for (int r = 0; r < rows; r++) free[c, r] = !BlockedWithRoom(At(c, r));

        var start = CellOf(from);
        var end = CellOf(goal);
        if (!free[end.c, end.r]) end = NearestFree(free, end, cols, rows);
        free[start.c, start.r] = true;   // wherever he stands now, he can leave

        // A* with diagonal steps.
        var cost = new float[cols, rows];
        var came = new (int c, int r)?[cols, rows];
        for (int c = 0; c < cols; c++) for (int r = 0; r < rows; r++) cost[c, r] = float.MaxValue;
        var open = new PriorityQueue<(int c, int r), float>();
        cost[start.c, start.r] = 0;
        open.Enqueue(start, 0);
        while (open.TryDequeue(out var cur, out _))
        {
            if (cur == end) break;
            for (int dc = -1; dc <= 1; dc++)
                for (int dr = -1; dr <= 1; dr++)
                {
                    if (dc == 0 && dr == 0) continue;
                    int nc = cur.c + dc, nr = cur.r + dr;
                    if (nc < 0 || nr < 0 || nc >= cols || nr >= rows || !free[nc, nr]) continue;
                    if (dc != 0 && dr != 0 && (!free[cur.c + dc, cur.r] || !free[cur.c, cur.r + dr])) continue;
                    float step = dc != 0 && dr != 0 ? 1.414f : 1f;
                    float c2 = cost[cur.c, cur.r] + step;
                    if (c2 >= cost[nc, nr]) continue;
                    cost[nc, nr] = c2;
                    came[nc, nr] = cur;
                    open.Enqueue((nc, nr), c2 + Mathf.Sqrt((nc - end.c) * (nc - end.c) + (nr - end.r) * (nr - end.r)));
                }
        }
        if (cost[end.c, end.r] == float.MaxValue) return new();

        var cells = new List<Vector2>();
        for (var p = ((int c, int r)?)end; p is { } q && q != start; p = came[q.c, q.r]) cells.Add(At(q.c, q.r));
        cells.Reverse();
        if (cells.Count > 0 && free[CellOf(goal).c, CellOf(goal).r]) cells[^1] = goal;

        // Cut corners: keep only the points he can't walk straight past.
        var path = new List<Vector2>();
        var here = from;
        for (int i = 0; i < cells.Count; i++)
        {
            bool last = i == cells.Count - 1;
            if (last || !Clear(here, cells[i + 1])) { path.Add(cells[i]); here = cells[i]; }
        }
        return path;
    }

    private bool Clear(Vector2 a, Vector2 b)
    {
        int steps = (int)(a.DistanceTo(b) / 2) + 1;
        for (int i = 1; i <= steps; i++)
            if (BlockedWithRoom(a.Lerp(b, i / (float)steps))) return false;
        return true;
    }

    private static (int c, int r) NearestFree(bool[,] free, (int c, int r) at, int cols, int rows)
    {
        for (int d = 1; d < Math.Max(cols, rows); d++)
            for (int dc = -d; dc <= d; dc++)
                foreach (int dr in new[] { d, -d, 0 })   // in front of it first: the floor towards the camera
                {
                    int c = at.c + dc, r = at.r + (dr == 0 ? 0 : dr);
                    if (c >= 0 && r >= 0 && c < cols && r < rows && free[c, r]) return (c, r);
                }
        return at;
    }
}
