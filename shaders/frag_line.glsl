#version 330 core
in vec3 VertexColor;
in vec2 TexCoord;
in vec3 Normal;
in vec3 FragPos;

out vec4 finalColor;

uniform vec3 uViewPos;


void main()
{
    finalColor = vec4(VertexColor,1.0);
}