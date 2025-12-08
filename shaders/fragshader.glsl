#version 330 core
in vec3 VertexColor;
in vec2 TexCoord;

out vec4 finalColor;

uniform sampler2D uTexture;

float near = 0.1; 
float far  = 100.0; 
  



void main()
{
    finalColor = texture(uTexture, TexCoord) * vec4(VertexColor, 1.0);
}