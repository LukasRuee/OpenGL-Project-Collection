#version 400 core

layout (location = 0) in vec3 in_position;
layout (location = 1) in vec3 in_color;
layout (location = 2) in vec2 in_texcoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 vertexColor;
out vec2 vertexUV0;

void main()
{
    gl_Position = projection * view * model * vec4(in_position, 1.0);
    vertexColor = in_color;
    vertexUV0 = in_texcoords;
}
