// People and furniture lit up under the mouse, the same way as things in the picture (room.gdshader):
// brighter, with a light rim along the edge of the shape.
shader_type canvas_item;

uniform bool lit = false;

void fragment() {
    vec4 c = COLOR;   // the texture, already: sampling it again would darken it
    if (lit && c.a > 0.5) {
        vec2 s = TEXTURE_PIXEL_SIZE;
        float edge = texture(TEXTURE, UV + vec2(s.x, 0.0)).a * texture(TEXTURE, UV - vec2(s.x, 0.0)).a
                   * texture(TEXTURE, UV + vec2(0.0, s.y)).a * texture(TEXTURE, UV - vec2(0.0, s.y)).a;
        c.rgb = c.rgb * 1.45 + (edge < 0.5 ? vec3(0.32, 0.29, 0.22) : vec3(0.05));
    }
    COLOR = c;
}
