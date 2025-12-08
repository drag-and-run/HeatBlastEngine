#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aVertexColor;
layout (location = 3) in vec3 aNormal;

out vec3 VertexColor;
out vec2 TexCoord;
out vec3 Normal;
out vec3 FragPos;

uniform mat4 uTransform;
uniform mat4 uView;
uniform mat4 uModel;
uniform mat4 uProjection;


void main()
{

    gl_Position = uProjection * uView * uModel * vec4(aPos, 1.0);
    FragPos = vec3(uModel * vec4(aPos, 1.0));
    VertexColor = aVertexColor;
    TexCoord = aTexCoord;
    Normal = aNormal;
}