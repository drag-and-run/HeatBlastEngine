#version 330 core
in vec3 VertexColor;
in float Time;
out vec4 finalColor;

void main()
{
    finalColor = vec4(vec3(cos(Time*8),0.0,0.0),1.0);
}