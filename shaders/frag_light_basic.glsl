#version 330 core
in vec3 VertexColor;
in vec2 TexCoord;
in vec3 Normal;
in vec3 FragPos;

out vec4 finalColor;

uniform sampler2D uTexture;
uniform sampler2D uTextureNormal;
uniform vec3 ulightPos;
uniform vec3 uViewPos;


void main()
{
    float ambientStrength = 0.8;
    vec3 ambientLight = ambientStrength * vec3(0.2,0.2,0.2);

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(ulightPos - FragPos);  
    
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * vec3(0.5,0.4,0.2);

    float specularStrength = 0.5;
    vec3 viewDir = normalize(uViewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);  

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 90);
    vec3 specular = specularStrength * spec * vec3(1.0,1.0,1.0);  
    
    vec3 result = (vec3(0.2,0.2,0.2) + diffuse + specular) * VertexColor;
    finalColor = vec4(result, 1.0) * texture(uTexture, TexCoord);

    //vec3 nColor = normalize(Normal) * 0.5 + 0.5;
    //finalColor = vec4(nColor, 1.0);
}