#version 330 core
out vec4 finalColor;

in vec3 TexCoord;

uniform samplerCube uCubemap;

void main()
{
    finalColor = texture(uCubemap, TexCoord);

}