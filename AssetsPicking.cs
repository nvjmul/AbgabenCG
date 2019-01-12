using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static System.Math;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Tutorial.Core
{
    public class AssetsPicking : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRenderer _sceneRenderer;
        private ScenePicker _scenePicker;
        private TransformComponent _baseTransform;
        private PickResult _currentPick;
        private float3 _oldColor;
        private TransformComponent _tank;
        private TransformComponent _turret;
        private TransformComponent _gun;
        private TransformComponent _wheel1Left;
        private TransformComponent _wheel2Left;
        private TransformComponent _wheel3Left;
        private TransformComponent _wheel1Right;
        private TransformComponent _wheel2Right;
        private TransformComponent _wheel3Right;
        private PickResult newPick;

        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intentsity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = AssetStorage.Get<SceneContainer>("Tank.fus");

            _tank = _scene.Children.FindNodes(node => node.Name == "Tank")?.FirstOrDefault()?.GetTransform();
            _turret = _scene.Children.FindNodes(node => node.Name == "Turret")?.FirstOrDefault()?.GetTransform();
            _gun = _scene.Children.FindNodes(node => node.Name == "Gun")?.FirstOrDefault()?.GetTransform();
            _wheel1Left = _scene.Children.FindNodes(node => node.Name == "Wheel1Left")?.FirstOrDefault()?.GetTransform();
            _wheel2Left = _scene.Children.FindNodes(node => node.Name == "Wheel2Left")?.FirstOrDefault()?.GetTransform();
            _wheel3Left = _scene.Children.FindNodes(node => node.Name == "Wheel3Left")?.FirstOrDefault()?.GetTransform();
            _wheel1Right = _scene.Children.FindNodes(node => node.Name == "Wheel1Right")?.FirstOrDefault()?.GetTransform();
            _wheel2Right = _scene.Children.FindNodes(node => node.Name == "Wheel2Right")?.FirstOrDefault()?.GetTransform();
            _wheel3Right = _scene.Children.FindNodes(node => node.Name == "Wheel3Right")?.FirstOrDefault()?.GetTransform();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRenderer(_scene);
            _scenePicker = new ScenePicker(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, 0, 10) * float4x4.CreateRotationX(-(float) Atan(15.0 / 40.0)) * float4x4.CreateRotationY((float) Atan(15.0 / 40.0));

            if (Mouse.LeftButton)
            {
                float2 pickPosClip = Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);
                _scenePicker.View = RC.View;
                _scenePicker.Projection = RC.Projection;
                List<PickResult> pickResults = _scenePicker.Pick(pickPosClip).ToList();
                newPick = null;
                if (pickResults.Count > 0)
                {
                    pickResults.Sort((a, b) => Sign(a.ClipPos.z - b.ClipPos.z));
                    newPick = pickResults[0];
                }
                if (newPick?.Node != _currentPick?.Node)
                {
                    if (_currentPick != null)
                    {
                        _currentPick.Node.GetMaterial().Diffuse.Color = _oldColor;
                    }
                    if (newPick != null)
                    {
                        var mat = newPick.Node.GetMaterial();
                        _oldColor = mat.Diffuse.Color;
                        mat.Diffuse.Color = new float3(0.4f, 0.4f, 1);
                    }
                    _currentPick = newPick;
                }
            }

            if (_currentPick != null) {
                if (_currentPick.Node.Name == "Tank") {
                    if (Keyboard.UpDownAxis != 0) {
                        _tank.Translation += new float3(0, 0, -0.025f * Keyboard.UpDownAxis);
                        _wheel1Left.Rotation += new float3(-0.025f * Keyboard.UpDownAxis, 0, 0);
                        _wheel2Left.Rotation += new float3(-0.025f * Keyboard.UpDownAxis, 0, 0);
                        _wheel3Left.Rotation += new float3(-0.025f * Keyboard.UpDownAxis, 0, 0);
                        _wheel1Right.Rotation += new float3(-0.025f * Keyboard.UpDownAxis, 0, 0);
                        _wheel2Right.Rotation += new float3(-0.025f * Keyboard.UpDownAxis, 0, 0);
                        _wheel3Right.Rotation += new float3(-0.025f * Keyboard.UpDownAxis, 0, 0);
                    }
                    if (Keyboard.LeftRightAxis != 0) {
                        _tank.Rotation += new float3(0, -0.025f * Keyboard.LeftRightAxis, 0);
                        _wheel1Left.Rotation += new float3(0.025f * Keyboard.LeftRightAxis, 0, 0);
                        _wheel2Left.Rotation += new float3(0.025f * Keyboard.LeftRightAxis, 0, 0);
                        _wheel3Left.Rotation += new float3(0.025f * Keyboard.LeftRightAxis, 0, 0);
                        _wheel1Right.Rotation += new float3(-0.025f * Keyboard.LeftRightAxis, 0, 0);
                        _wheel2Right.Rotation += new float3(-0.025f * Keyboard.LeftRightAxis, 0, 0);
                        _wheel3Right.Rotation += new float3(-0.025f * Keyboard.LeftRightAxis, 0, 0);
                    }
                }
                if (_currentPick.Node.Name == "Turret") {
                    _turret.Rotation += new float3(0.025f * Keyboard.UpDownAxis, 0.025f * Keyboard.LeftRightAxis, 0);
                }
                if (_currentPick.Node.Name == "Gun") {
                    _gun.Rotation += new float3(0.025f * Keyboard.UpDownAxis, 0, 0);
                }
            }

            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered farame) on the front buffer.
            Present();
        }


        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45� Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }
    }
}
