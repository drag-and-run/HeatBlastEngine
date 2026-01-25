#version 330 core
in vec3 VertexColor;
in vec2 TexCoord;

out vec4 finalColor;

uniform sampler2D uTexture;




void main()
{
    finalColor = texture(uTexture, TexCoord) * vec4(VertexColor, 1.0);
}