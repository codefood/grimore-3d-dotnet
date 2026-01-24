shader_type spatial;

float sdVerticalCapsule( vec3 p, float h, float r )
{
  p.y -= clamp( p.y, 0.0, h );
  return length( p ) - r;
}

float sdRoundBox( vec3 p, vec3 b, float r )
{
  vec3 q = abs(p) - b + r;
  return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
}

float sdCappedTorus(in vec3 p, in vec2 sc, in float ra, in float rb)
{
    p.x = abs(p.x);
    float k = (sc.y*p.x>sc.x*p.y) ? dot(p.xy,sc) : length(p.xy);
    return sqrt( dot(p,p) + ra*ra - 2.0*ra*k ) - rb;
}

float map( in vec3 pos )
{
    vec2 c = vec2(sin(PI),cos(PI));
	float r = 0.05;
	float line = sdVerticalCapsule(pos, 0.25, r);
	float ring = sdCappedTorus(pos + vec3(0.0, 0.15, 0.0), c, 0.1, r );
	float teeth = sdRoundBox(pos + vec3(0.09, -0.2, 0.0), vec3(0.06), r);
	return min(min(line, ring), teeth);
}

vec3 calcNormal( in vec3 pos )
{
    vec2 e = vec2(1.0,-1.0)*0.5773;
    const float eps = 0.0005;
    return normalize( e.xyy*map( pos + e.xyy*eps ) + 
					  e.yyx*map( pos + e.yyx*eps ) + 
					  e.yxy*map( pos + e.yxy*eps ) + 
					  e.xxx*map( pos + e.xxx*eps ) );
}

void fragment() {
	// camera movement	
	float an = 0.7 * TIME;
	vec3 ro = vec3( 1.0*cos(an), 0.2, 1.0*sin(an) );
    vec3 ta = vec3(0.0); //orbital center of gravity
    // camera matrix
    vec3 ww = normalize( ta - ro );
    vec3 uu = normalize( cross(ww,vec3(0.0,1.0,0.0) ) );
    vec3 vv =          ( cross(uu,ww));
    
    // render
    vec3 tot = vec3(0.0);
    
    for( int m=0; m<3; m++ )
	{
	    for( int n=0; n<3; n++ )
	    {
	        // pixel coordinates
	        vec2 o = vec2(float(m),float(n)) / float(3) - 0.5;
	
			vec2 res = VIEWPORT_SIZE;
			vec2 p = (-res.xy + (2.0 * (FRAGCOORD.xy + o).xy)) / res.y;

		    // create view ray
	        vec3 rd = normalize( p.x*uu + p.y*vv + 1.5*ww );

	        // raymarch
	        const float tmax = 5.0;
	        float t = 0.0;
	        for( int i=0; i<256; i++ )
	        {
	            vec3 pos = ro + t*rd;
	            float h = map(pos);
	            if( h<0.0001 || t>tmax ) break;
	            t += h;
	        }
	        
	    
	        // shading/lighting	
	        vec3 col = vec3(0.0);
	        if( t<tmax )
	        {
	            vec3 pos = ro + t*rd;
	            vec3 nor = calcNormal(pos);
	            float dif = clamp( dot(nor,vec3(0.57703)), 0.0, 1.0 );
	            float amb = 0.5 + 0.5*dot(nor,vec3(0.0,1.0,0.0));
				//vec3 colDefault = vec3(0.7, 0.7, 0.0); //yellowish
				vec3 colDefault = vec3(0.8, 0.0, 0.0);
	            col = colDefault * amb + colDefault * dif;
	        }

	        // gamma        
	        col = sqrt( col );
		    tot += col;
	    }
	}
    tot /= float(3*3);
	
	ALBEDO = tot;
	
	if(tot == vec3(0)) {
		ALPHA = 0.0;
	} else {
		
	}
}
