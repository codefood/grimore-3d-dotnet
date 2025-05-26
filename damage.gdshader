shader_type canvas_item;

uniform bool active = false;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR = vec4(vec3(1.0) - texture(TEXTURE, UV).rgb, 1.0);
	//if(active) {
		//vec4 colour = texture(TEXTURE, UV);
		//colour.r = 1.0;
		//colour.b = 0.0;
		//COLOR = colour;
	//}
	
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
