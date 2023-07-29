#version 330 core

in vec4 passColor;

out vec4 FragColor;

// uniform sampler2D textureSampler;

void main()
{
    FragColor = passColor;
}