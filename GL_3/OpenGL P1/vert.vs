#version 400 core
layout (location = 0) in vec3 pos;
// Properties
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = projection * view * model * vec4(pos, 1.0);
}

// Instead of passing model, view, and projection each after one, 
// just pass all or multiply in one combined matrix.
// Result = less uniforms
// Example: Make a Transform variable, which has everything in it