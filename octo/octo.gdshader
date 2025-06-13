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


vec3 step_square_space(vec2 uv) {
	vec2 width = vec2(0.2);
	vec2 x = vec2(0.1);
	//vec2 bottomleft = step(width, uv);
	float foo = uv.x > 0.2 && uv.x < 0.3 ? 1.0 : 0.0;
	 // * topright.x * topright.y;
	return vec3(foo);
}

vec3 circleAt(vec2 uv, vec2 pt, float size) {
	float pct = distance(uv, pt);
	return vec3(step(pct, size));	
}

vec3 circle(vec2 uv) {
	return circleAt(uv, vec2(0.2, 0.8), 0.2)
		+ circleAt(uv, vec2(0.4, 0.4), TIME)
		+ circleAt(uv, vec2(0.8, 0.8), 0.1);
}

vec3 circle_hollow(vec2 uv) {
	float pct = distance(uv, vec2(0.5));
	float next = step(0.1, pct);
	pct = next - step(0.2, pct);
	return vec3(pct);
}

vec3 circle_sqrt(vec2 uv) {
	float pct = distance(uv, vec2(0.5));
	vec2 toCenter = vec2(0.2) - uv;
	pct = sqrt(toCenter.x * toCenter.x + toCenter.y * toCenter.y);
	return vec3(pct);
}

vec3 circle_pos_length(vec2 uv) {
	float pct = distance(uv, vec2(0.5));
	vec2 toCenter = vec2(0.9) - uv;
	pct = length(toCenter);
	return vec3(pct);
}

vec3 circle_pos(vec2 uv) {
	float pct = distance(uv, vec2(0.3));
	pct = step(0.1, pct);
	return vec3(pct);
}

void fragment() {
	//vec3 color = float_square(UV);
	//vec3 color = step_square(UV);
	//vec3 color = step_square_space(UV);
	//vec3 color = smooth_square(UV);
	//vec3 color = circle(UV);
	vec2 st = UV;
	vec3 color = vec3(0.0);
	float d = 0.0;
	st = st * 2.-22.;
	d = length( abs(st)-.82 ) * TIME;
	
	ALBEDO = vec3(fract(d*10.0));

	//ALBEDO = color;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
