using System.Collections.Generic;
using System.Text;
using GlmSharp;
using Veldrid;
using Veldrid.SPIRV;

namespace MazeGame
{
    public class Renderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly ResourceFactory _resourceFactory;
        private readonly Pipeline _pipeline;
        private readonly CommandList _commandList;
        private readonly Shader[] _shaders;
        private readonly ResourceSet _resourceSet;
        private readonly DeviceBuffer _uniformBuffer;

        private Veldrid.Rectangle? _windowBounds;
        
        private readonly RgbaFloat _backgroundColor;
        private mat4 _vp;
        private mat4 _projection;

        private readonly Dictionary<Renderable, RenderData> _renderables;

        private const string VertexCode = @"
#version 450

layout(set = 0, binding = 0) uniform UniformBuffer {
    mat4 mvp;
    vec4 color;
};

layout(location = 0) in vec2 Position;

layout(location = 0) out vec4 fsin_Color;

void main()
{
    gl_Position = mvp * vec4(Position, 0, 1);
    fsin_Color = color;
}";

        private const string FragmentCode = @"
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = fsin_Color;
}";

        public Renderer(GraphicsDevice graphicsDevice, Veldrid.Rectangle bounds, RgbaFloat backgroundColor)
        {
            _renderables = new Dictionary<Renderable, RenderData>();
            
            _graphicsDevice = graphicsDevice;
            _backgroundColor = backgroundColor;
            _resourceFactory = _graphicsDevice.ResourceFactory;

            var vertexBufferLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float2)
            );

            var vertexShaderDescription = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(VertexCode),
                "main"
            );
            var fragmentShadeDescription = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(FragmentCode),
                "main"
            );
            _shaders = _resourceFactory.CreateFromSpirv(vertexShaderDescription, fragmentShadeDescription);

            var resourceLayoutDescription = new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("UniformBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
            );
            var resourceLayout = _resourceFactory.CreateResourceLayout(resourceLayoutDescription);
            _uniformBuffer = _resourceFactory.CreateBuffer(new BufferDescription(80,
                BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            var resourceSetDescription = new ResourceSetDescription(resourceLayout, _uniformBuffer);
            _resourceSet = _resourceFactory.CreateResourceSet(resourceSetDescription);

            var pipelineDescription = new GraphicsPipelineDescription()
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = new DepthStencilStateDescription(true, true, ComparisonKind.LessEqual),
                RasterizerState = new RasterizerStateDescription(
                    FaceCullMode.Back,
                    PolygonFillMode.Solid,
                    FrontFace.Clockwise,
                    true,
                    false),
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceLayouts = new[] {resourceLayout},
                ShaderSet = new ShaderSetDescription(new[] {vertexBufferLayout}, _shaders),
                Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription
            };
            _pipeline = _resourceFactory.CreateGraphicsPipeline(pipelineDescription);

            _commandList = _resourceFactory.CreateCommandList();
            
            WindowResized(bounds);
        }

        public void RegisterRenderable(Renderable renderable)
        {
            var renderData = new RenderData();
            if (_renderables.TryAdd(renderable, renderData))
            {
                var vertexBufferDescription = new BufferDescription(
                    (uint) renderable.VertexArray.Length * Point.SizeInBytes, BufferUsage.VertexBuffer);
                var vertexBuffer = _resourceFactory.CreateBuffer(vertexBufferDescription);
                _graphicsDevice.UpdateBuffer(vertexBuffer, 0, renderable.VertexArray);
                renderData.VertexBuffer = vertexBuffer;
                
                var indexBufferDescription = new BufferDescription(
                    (uint) renderable.IndexArray.Length * sizeof(uint), BufferUsage.IndexBuffer);
                var indexBuffer = _resourceFactory.CreateBuffer(indexBufferDescription);
                _graphicsDevice.UpdateBuffer(indexBuffer, 0, renderable.IndexArray);
                renderData.IndexBuffer = indexBuffer;
            }
        }

        public void BeginRender(mat4 view)
        {
            _commandList.Begin();
            _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
            if (_windowBounds != null)
            {
                var width = _windowBounds.Value.Width;
                var height = _windowBounds.Value.Height;
                _projection = mat4.Ortho(0, width, 0, height, 0.1f, 100);
                _graphicsDevice.ResizeMainWindow((uint) width, (uint) height);
                _commandList.SetFullViewports();
                _windowBounds = null;
            }
            _vp = _projection * view;
            _commandList.ClearColorTarget(0, _backgroundColor);
            _commandList.SetPipeline(_pipeline);
            _commandList.SetGraphicsResourceSet(0, _resourceSet);
        }

        public void Render(Renderable renderable)
        {
            var renderData = _renderables[renderable];
            _commandList.SetVertexBuffer(0, renderData.VertexBuffer);
            _commandList.SetIndexBuffer(renderData.IndexBuffer, IndexFormat.UInt32);
            var mvp = _vp * renderable.Model;
            _commandList.UpdateBuffer(_uniformBuffer, 0, mvp);
            _commandList.UpdateBuffer(_uniformBuffer, 64, renderable.Color);
            _commandList.DrawIndexed((uint) renderable.IndexArray.Length, 1, 0, 0, 0);
        }

        public void EndRender()
        {
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);
            _graphicsDevice.SwapBuffers();
        }

        public void WindowResized(Veldrid.Rectangle bounds)
        {
            _windowBounds = bounds;
        }

        public void Dispose()
        {
            _pipeline.Dispose();
            foreach (var shader in _shaders)
            {
                shader.Dispose();
            }
            foreach (var renderData in _renderables.Values)
            {
                renderData.Dispose();
            }
            _uniformBuffer.Dispose();
            _resourceSet.Dispose();
            _pipeline.Dispose();
            _commandList.Dispose();
        }
        
        private class RenderData
        {
            public DeviceBuffer VertexBuffer;
            public DeviceBuffer IndexBuffer;

            public void Dispose()
            {
                VertexBuffer.Dispose();
                IndexBuffer.Dispose();
            }
        }
    }
}