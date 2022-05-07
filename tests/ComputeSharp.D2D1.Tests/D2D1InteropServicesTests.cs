﻿using ComputeSharp.D2D1.Interop;
using ComputeSharp.D2D1.Tests.Effects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComputeSharp.D2D1.Tests;

[TestClass]
[TestCategory("D2D1InteropServices")]
public partial class D2D1InteropServicesTests
{
    [TestMethod]
    public unsafe void GetInputCount()
    {
        Assert.AreEqual(D2D1PixelShader.GetInputCount<InvertEffect>(), 1);
        Assert.AreEqual(D2D1PixelShader.GetInputCount<PixelateEffect.Shader>(), 1);
        Assert.AreEqual(D2D1PixelShader.GetInputCount<ZonePlateEffect>(), 0);
        Assert.AreEqual(D2D1PixelShader.GetInputCount<ShaderWithMultipleInputs>(), 7);
    }

    [TestMethod]
    public unsafe void GetInputType()
    {
        Assert.AreEqual(D2D1PixelShader.GetInputType<ShaderWithMultipleInputs>(0), D2D1PixelShaderInputType.Simple);
        Assert.AreEqual(D2D1PixelShader.GetInputType<ShaderWithMultipleInputs>(1), D2D1PixelShaderInputType.Complex);
        Assert.AreEqual(D2D1PixelShader.GetInputType<ShaderWithMultipleInputs>(2), D2D1PixelShaderInputType.Simple);
        Assert.AreEqual(D2D1PixelShader.GetInputType<ShaderWithMultipleInputs>(3), D2D1PixelShaderInputType.Complex);
        Assert.AreEqual(D2D1PixelShader.GetInputType<ShaderWithMultipleInputs>(4), D2D1PixelShaderInputType.Complex);
        Assert.AreEqual(D2D1PixelShader.GetInputType<ShaderWithMultipleInputs>(5), D2D1PixelShaderInputType.Complex);
    }

    [D2DInputCount(7)]
    [D2DInputSimple(0)]
    [D2DInputSimple(2)]
    [D2DInputComplex(1)]
    [D2DInputComplex(3)]
    [D2DInputComplex(5)]
    [D2DInputSimple(6)]
    partial struct ShaderWithMultipleInputs : ID2D1PixelShader
    {
        public Float4 Execute()
        {
            return 0;
        }
    }

    [TestMethod]
    public unsafe void GetOutputBufferPrecision()
    {
        Assert.AreEqual(D2D1PixelShader.GetOutputBufferPrecision<ShaderWithMultipleInputs>(), D2D1BufferPrecision.Unknown);
        Assert.AreEqual(D2D1PixelShader.GetOutputBufferPrecision<OnlyBufferPrecisionShader>(), D2D1BufferPrecision.Int16Normalized);
        Assert.AreEqual(D2D1PixelShader.GetOutputBufferPrecision<OnlyChannelDepthShader>(), D2D1BufferPrecision.Unknown);
        Assert.AreEqual(D2D1PixelShader.GetOutputBufferPrecision<CustomBufferOutputShader>(), D2D1BufferPrecision.Int8NormalizedSRGB);
    }

    [TestMethod]
    public unsafe void GetOutputBufferChannelDepth()
    {
        Assert.AreEqual(D2D1PixelShader.GetOutputBufferChannelDepth<ShaderWithMultipleInputs>(), D2D1ChannelDepth.Default);
        Assert.AreEqual(D2D1PixelShader.GetOutputBufferChannelDepth<OnlyBufferPrecisionShader>(), D2D1ChannelDepth.Default);
        Assert.AreEqual(D2D1PixelShader.GetOutputBufferChannelDepth<OnlyChannelDepthShader>(), D2D1ChannelDepth.Four);
        Assert.AreEqual(D2D1PixelShader.GetOutputBufferChannelDepth<CustomBufferOutputShader>(), D2D1ChannelDepth.One);
    }

    [D2DInputCount(0)]
    partial struct EmptyShader : ID2D1PixelShader
    {
        public Float4 Execute()
        {
            return 0;
        }
    }

    [D2DInputCount(0)]
    [D2DOutputBuffer(D2D1BufferPrecision.Int16Normalized)]
    partial struct OnlyBufferPrecisionShader : ID2D1PixelShader
    {
        public Float4 Execute()
        {
            return 0;
        }
    }

    [D2DInputCount(0)]
    [D2DOutputBuffer(D2D1ChannelDepth.Four)]
    partial struct OnlyChannelDepthShader : ID2D1PixelShader
    {
        public Float4 Execute()
        {
            return 0;
        }
    }

    [D2DInputCount(0)]
    [D2DOutputBuffer(D2D1BufferPrecision.Int8NormalizedSRGB, D2D1ChannelDepth.One)]
    partial struct CustomBufferOutputShader : ID2D1PixelShader
    {
        public Float4 Execute()
        {
            return 0;
        }
    }
}