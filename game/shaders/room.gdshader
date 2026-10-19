// The room's final image: albedo lit by the room's lights, plus emissive glow, bloom, drifting
// fog, a filmic tone curve, saturation, vignette and grain. Lighting is computed per art pixel
// so light stays crisp and pixelated; bloom and fog are smooth.
shader_type canvas_item;
render_mode unshaded;

const int MAX_LIGHTS = 16;

uniform vec2 canvas_size = vec2(480.0, 270.0);
uniform sampler2D emissive : filter_nearest;
uniform sampler2D glow : filter_linear;
uniform sampler2D base : filter_nearest;   // the room's albedo alone: where the frame differs, something stands in front
uniform vec3 ambient = vec3(0.4);
uniform float emissive_gain = 1.0;   // dusk: windows and bulbs come on
uniform int light_count = 0;
uniform vec4 light_pos[MAX_LIGHTS];   // x, y, radius, intensity
uniform vec4 light_col[MAX_LIGHTS];   // r, g, b, -
uniform vec4 light_dir[MAX_LIGHTS];   // dir.x, dir.y, cos_inner, cos_outer (cos_outer < -1.5 => omni)
uniform float bloom = 0.8;
uniform float fog = 0.12;
uniform vec3 fog_color = vec3(0.6, 0.55, 0.5);
uniform float exposure = 1.15;
uniform float saturation = 1.0;
uniform float vignette = 0.42;
uniform float grain = 0.035;

float hash(vec2 p) { return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453); }
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash(i), b = hash(i + vec2(1.0, 0.0)), c = hash(i + vec2(0.0, 1.0)), d = hash(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}
float fbm(vec2 p) {
    float s = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) { s += vnoise(p) * a; p *= 2.03; a *= 0.5; }
    return s;
}

vec3 light_at(vec2 p) {
    vec3 acc = ambient;
    for (int i = 0; i < MAX_LIGHTS; i++) {
        if (i >= light_count) break;
        vec4 lp = light_pos[i];
        vec2 d = p - lp.xy;
        float dist = length(d);
        if (dist > lp.z) continue;
        float k = 1.0 - (dist * dist) / (lp.z * lp.z);
        float f = k * k * lp.w;
        vec4 ld = light_dir[i];
        if (ld.w > -1.5) f *= smoothstep(ld.w, ld.z, dot(d / max(dist, 0.001), ld.xy));
        acc += light_col[i].rgb * f;
    }
    return acc;
}

vec3 tonemap(vec3 x) {
    x *= exposure;
    return clamp((x * (2.51 * x + 0.03)) / (x * (2.43 * x + 0.59) + 0.14), 0.0, 1.0);
}

void fragment() {
    vec2 px = floor(UV * canvas_size) + 0.5;
    vec2 suv = px / canvas_size;
    vec3 seen = texture(TEXTURE, suv).rgb;
    float clear = 1.0 - step(0.01, distance(seen, texture(base, suv).rgb));
    vec3 col = seen * light_at(px) + texture(emissive, suv).rgb * 1.35 * clear * emissive_gain;
    vec3 halo = texture(glow, UV).rgb * emissive_gain;
    col += halo * bloom;

    float n = fbm(px * 0.012 + vec2(TIME * 0.05, TIME * 0.015));
    col = mix(col, fog_color * 0.25 + halo * 1.6, fog * smoothstep(0.35, 0.85, n));

    col = tonemap(col);
    float luma = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(luma), col, saturation);

    vec2 c = UV - 0.5;
    col *= mix(1.0 - vignette, 1.0, smoothstep(0.85, 0.25, length(c * vec2(1.0, 1.15))));
    col += (hash(px + fract(TIME * 13.17) * 91.0) - 0.5) * grain;
    COLOR = vec4(col, 1.0);
}
