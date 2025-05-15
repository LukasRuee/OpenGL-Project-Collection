#version 400 core

uniform vec3 color;
uniform sampler2D baseColorMap;

in vec3 vertexColor;
in vec2 vertexUV0;

out vec4 fragColor;

void main()
{
    vec3 baseColor = texture(baseColorMap, vertexUV0).rgb;
    fragColor = vec4(vertexColor * color, 1.0f);
}
