using System;
using GLCamera.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace GLCamera
{
    // We now have a rotating rectangle but how can we make the view move based on the users input?
    // In this tutorial we will take a look at how you could implement a camera class
    // and start responding to user input.
    // You can move to the camera class to see a lot of the new code added.
    // Otherwise you can move to Load to see how the camera is initialized.

    // In reality, we can't move the camera but we actually move the rectangle.
    // This will explained more in depth in the web version, however it pretty much gives us the same result
    // as if the view itself was moved.
    public class Window : GameWindow
    {
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int indexCount = 36; // cube 索引数量

        private int _elementBufferObject;

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private int _vertexArrayObjectForCube;

        private Shader _shader;

        private Shader _shaderCube;

        private Texture _texture;

        private Texture _texture2;

        // The view and projection matrices have been removed as we don't need them here anymore.
        // They can now be found in the new camera class.

        // We need an instance of the new camera class so it can manage the view and projection matrix code.
        // We also need a boolean set to true to detect whether or not the mouse has been moved for the first time.
        // Finally, we add the last position of the mouse so we can calculate the mouse offset easily.
        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        private double _time;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _shader = new Shader("Shaders/vert.glsl", "Shaders/frag.glsl");
            _shader.Use();

            _texture = Texture.LoadFromFile("Resources/container.png");
            _texture.Use(TextureUnit.Texture0);

            _texture2 = Texture.LoadFromFile("Resources/awesomeface.png");
            _texture2.Use(TextureUnit.Texture1);

            _shader.SetInt("texture0", 0);
            _shader.SetInt("texture1", 1);

            _shaderCube = new Shader("Shaders/cube/vert.glsl", "Shaders/cube/frag.glsl");

            // We initialize the camera so that it is 3 units back from where the rectangle is.
            // We also give it the proper aspect ratio.
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            // We make the mouse cursor invisible and captured so we can have proper FPS-camera movement.
            CursorState = CursorState.Grabbed;
        }

        /// <summary>
        /// 方块mesh
        /// </summary>
        private void GenerateCube()
        {
            float len = 1.0f;

            // colors 
            var (c0r, c0b, c0g, c0a) = (0.1f, 0.5f, 0.1f, 1.0f);
            var (c1r, c1b, c1g, c1a) = (0.1f, 0.5f, 0.6f, 1.0f); 
            var (c2r, c2b, c2g, c2a) = (0.3f, 0.8f, 0.2f, 1.0f); 
            var (c3r, c3b, c3g, c3a) = (0.1f, 0.3f, 0.1f, 1.0f); 
            var (c4r, c4b, c4g, c4a) = (0.4f, 0.5f, 0.1f, 1.0f); 
            var (c5r, c5b, c5g, c5a) = (0.5f, 0.2f, 0.3f, 1.0f);
            var (c6r, c6b, c6g, c6a) = (0.1f, 0.2f, 0.4f, 1.0f); 
            var (c7r, c7b, c7g, c7a) = (0.7f, 0.5f, 0.5f, 1.0f); 

            float[] vertices = 
            {
                -len, -len, -len, c0r, c0b, c0g, c0a,  // 0
                len, -len, -len, c1r, c1b, c1g, c1a,  // 1
                len, -len,  len, c2r, c2b, c2g, c2a,  // 2
                -len, -len,  len, c3r, c3b, c3g, c3a,  // 3

                -len,  len, -len, c4r, c4b, c4g, c4a,   // 4
                len,  len, -len, c5r, c5b, c5g, c5a,   // 5
                len,  len,  len, c6r, c6b, c6g, c6a,   // 6
                -len,  len,  len, c7r, c7b, c7g, c7a    // 7
            };

            int[] indices = 
            {
                7, 6, 2,  7, 2, 3,      // front face
                4, 5, 1,  4, 1, 0,      // back face
                4, 5, 6,  4, 6, 7,      // top face
                0, 1, 2,  0, 2, 3,      // bottom face
                6, 5, 1,  6, 1, 2,      // right face
                7, 4, 0,  7, 0, 3       // left face
            };


            GenerateVAOforCube(len, vertices, indices);
        }

        private void GenerateVAOforCube(float len, float[] vertices, int[] indices)
        {
            var vao = GL.GenVertexArray();
            _vertexArrayObjectForCube = vao;
            GL.BindVertexArray(vao);
            // create vbo to store the data in opengl and copy data to vbo
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            // create ebo
            var ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);
            // tell opengl how to use the data via attributes
            // position attribute
            // int attrPosition_Position = GL.GetAttribLocation(shader.ShaderProgram, "aPos");
            int attrPosition_Position = 0;
            GL.EnableVertexAttribArray(attrPosition_Position);
            GL.VertexAttribPointer(attrPosition_Position, 3, VertexAttribPointerType.Float, false, sizeof(float) * 7, 0);
            int attrPosition_Color = 1;
            GL.EnableVertexAttribArray(attrPosition_Color);
            GL.VertexAttribPointer(attrPosition_Color, 4, VertexAttribPointerType.Float, false, sizeof(float) * 7, sizeof(float) * 3);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer(); // VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer(); // EBO，顶点索引可以存储在EBO中，索引缓冲对象
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);
            // 创建方块
            GenerateCube();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _time += 4.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // 步骤就是 1.绑定vao 2.使用shader实例
            GL.BindVertexArray(_vertexArrayObject);

            _texture.Use(TextureUnit.Texture0);
            _texture2.Use(TextureUnit.Texture1);
            _shader.Use();

            // Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time))
            // 这段代码加了旋转
            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            GL.BindVertexArray(_vertexArrayObjectForCube);
            _shaderCube.Use();

            _shaderCube.SetMatrix4("model", Matrix4.CreateTranslation(2, 0, 0));
            _shaderCube.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderCube.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        // In the mouse wheel function, we manage all the zooming of the camera.
        // This is simply done by changing the FOV of the camera.
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            // We need to update the aspect ratio once the window has been resized.
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
