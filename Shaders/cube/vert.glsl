#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec4 aColor;

uniform mat4 model;
uniform mat4 projection;
uniform mat4 view;

out vec4 passColor;

void main()
{
    gl_Position =  vec4(aPos, 1.0) * model * view * projection;
    passColor = aColor;
}