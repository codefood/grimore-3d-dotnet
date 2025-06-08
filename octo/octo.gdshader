shader_type spatial;

void vertex() {
	// Called for every vertex the material is visible on.
}

vec3 float_square(vec2 uv) {
	float width = 0.8;
	float top = floor(uv.y + width);
	float bottom = floor(1.0 - uv.y + width);
	float left = floor(uv.x + width);
	float right = floor(1.0 - uv.x + width);
	float percent = top * left * right * bottom;
	return vec3(percent);
}
vec3 step_square(vec2 uv) {
	vec2 bottomleft = step(vec2(0.1), uv);
	vec2 topright = step(vec2(0.1), 1.0 - uv);
	float percent = bottomleft.x * bottomleft.y * topright.x * topright.y;
	return vec3(percent);
}
vec3 smooth_square(vec2 uv) {
	vec2 width = vec2(0.1);
	vec2 blurwidth = width + vec2(0.05);
	vec2 bottomleft = smoothstep(width, blurwidth, uv);
	vec2 topright = smoothstep(width, blurwidth, 1.0 - uv);
	float percent = bottomleft.x * bottomleft.y * topright.x * topright.y;
	return vec3(percent);
}

void fragment() {
	//vec3 color = float_square(UV);
	//vec3 color = step_square(UV);
	vec3 color = smooth_square(UV);
	ALBEDO = color;

	//ALBEDO = color;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
