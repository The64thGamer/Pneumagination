shader_type spatial;
render_mode specular_disabled,ambient_light_disabled;
uniform vec4 colorA : source_color = vec4(1,0.4,0,1);
uniform vec4 colorB : source_color = vec4(0.4,1,0,1);
uniform vec4 colorC : source_color = vec4(0.5,0.5,1,1);
uniform vec4 colorD : source_color = vec4(0.5,0.5,0.5,1);
uniform vec4 colorA_shaded: source_color = vec4(0.5,0,0,1);
uniform vec4 colorB_shaded: source_color = vec4(0,0.5,0,1);
uniform vec4 colorC_shaded: source_color = vec4(0,0,0.5,1);
uniform vec4 colorD_shaded : source_color = vec4(0.25,0.25,0.25,1);
global uniform vec4 ambient : source_color;

uniform sampler2D palette : source_color, hint_default_white;
	
const vec3 white = vec3(1,1,1);
const vec3 black = vec3(0,0,0);

varying vec3 paint;
varying vec3 paint_shaded;
varying vec3 paint_black;




void fragment() {
	ALBEDO = white;
	
	paint = vec3(0);
	paint_shaded = vec3(0);

	paint = mix(paint,colorA.rgb,texture(palette, UV).r);
	paint = mix(paint,colorB.rgb,texture(palette, UV).g);
	paint = mix(paint,colorC.rgb,texture(palette, UV).b);
	paint = mix(paint,colorD.rgb,texture(palette, UV).a);
	
	paint_shaded = mix(paint_shaded,colorA_shaded.rgb,texture(palette, UV).r);
	paint_shaded = mix(paint_shaded,colorB_shaded.rgb,texture(palette, UV).g);
	paint_shaded = mix(paint_shaded,colorC_shaded.rgb,texture(palette, UV).b);
	paint_shaded = mix(paint_shaded,colorD_shaded.rgb,texture(palette, UV).a);
}

void light() { 
	SPECULAR_LIGHT += ATTENUATION * LIGHT_COLOR.rgb * max(dot(LIGHT,NORMAL), 0.0) + ambient.rgb;
	
	
	if (SPECULAR_LIGHT.r < 0.5f && 
		SPECULAR_LIGHT.g < 0.5f && 
		SPECULAR_LIGHT.b < 0.5f)
	{
		DIFFUSE_LIGHT = mix(black, paint_shaded.rgb, SPECULAR_LIGHT*2.0f) - SPECULAR_LIGHT;
	}
	else
	{
		DIFFUSE_LIGHT = mix(paint_shaded.rgb, paint.rgb, (clamp(SPECULAR_LIGHT,0,1)-0.5f)*2.0f) - SPECULAR_LIGHT;
	}

}