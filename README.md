# GLCamera
基于最新的OpenTK4，完成的相机类。  
code from [here](https://github.com/opentk/LearnOpenTK).  

## 截图
![image](https://github.com/pigLoveRabbit520/GLCamera/assets/16663435/e1b91b67-3c05-4546-bc8f-a6df4b047a7f)

## 参考
[一看就懂的 OpenGL 基础概念（3）：各种 O 之 VBO、EBO、VAO丨音视频基础](https://zhuanlan.zhihu.com/p/585126881)


## 函数说明
```
void gl.vertexAttribPointer(index, size, type, normalized, stride, offset);
```
参数
index
指定要修改的顶点属性的索引
* size
指定每个顶点属性的组成数量，必须是1，2，3或4。
* type
指定数组中每个元素的数据类型
* normalized
当转换为浮点数时是否应该将整数数值归一化到特定的范围
* stride 步长
一个GLsizei，以字节为单位指定连续顶点属性开始之间的偏移量(即数组中一行长度)。不能大于255。如果stride为0，则假定该属性是紧密打包的，即不交错属性，每个属性在一个单独的块中，下一个顶点的属性紧跟当前顶点之后。
* offset
GLintptr (en-US)指定顶点属性数组中第一部分的字节偏移量。必须是类型的字节长度的倍数
