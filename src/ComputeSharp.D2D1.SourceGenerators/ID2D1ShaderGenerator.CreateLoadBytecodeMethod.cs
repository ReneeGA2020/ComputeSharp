using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using ComputeSharp.D2D1.Exceptions;
using ComputeSharp.D2D1.Shaders.Translation;
using ComputeSharp.D2D1.SourceGenerators.Models;
using ComputeSharp.SourceGeneration.Extensions;
using ComputeSharp.SourceGeneration.Helpers;
using ComputeSharp.SourceGeneration.Models;
using Microsoft.CodeAnalysis;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static ComputeSharp.SourceGeneration.Diagnostics.DiagnosticDescriptors;

namespace ComputeSharp.D2D1.SourceGenerators;

/// <inheritdoc/>
partial class ID2D1ShaderGenerator
{
    /// <summary>
    /// A helper with all logic to generate the <c>LoadBytecode</c> method.
    /// </summary>
    internal static partial class LoadBytecode
    {
        /// <summary>
        /// Extracts the shader profile for the current shader.
        /// </summary>
        /// <param name="structDeclarationSymbol">The input <see cref="INamedTypeSymbol"/> instance to process.</param>
        /// <returns>The shader profile to use to compile the shader, if present.</returns>
        public static D2D1ShaderProfile? GetShaderProfile(INamedTypeSymbol structDeclarationSymbol)
        {
            if (structDeclarationSymbol.TryGetAttributeWithFullyQualifiedMetadataName("ComputeSharp.D2D1.D2DShaderProfileAttribute", out AttributeData? attributeData))
            {
                return (D2D1ShaderProfile)attributeData.ConstructorArguments[0].Value!;
            }

            if (structDeclarationSymbol.ContainingAssembly.TryGetAttributeWithFullyQualifiedMetadataName("ComputeSharp.D2D1.D2DShaderProfileAttribute", out attributeData))
            {
                return (D2D1ShaderProfile)attributeData.ConstructorArguments[0].Value!;
            }

            return null;
        }

        /// <summary>
        /// Extracts the compile options for the current shader.
        /// </summary>
        /// <param name="diagnostics">The collection of produced <see cref="DiagnosticInfo"/> instances.</param>
        /// <param name="structDeclarationSymbol">The input <see cref="INamedTypeSymbol"/> instance to process.</param>
        /// <returns>The compile options to use to compile the shader, if present.</returns>
        public static D2D1CompileOptions? GetCompileOptions(ImmutableArrayBuilder<DiagnosticInfo> diagnostics, INamedTypeSymbol structDeclarationSymbol)
        {
            if (structDeclarationSymbol.TryGetAttributeWithFullyQualifiedMetadataName("ComputeSharp.D2D1.D2DCompileOptionsAttribute", out AttributeData? attributeData))
            {
                D2D1CompileOptions options = (D2D1CompileOptions)attributeData.ConstructorArguments[0].Value!;

                if ((options & D2D1CompileOptions.PackMatrixColumnMajor) != 0)
                {
                    diagnostics.Add(
                        InvalidPackMatrixColumnMajorOption,
                        structDeclarationSymbol,
                        structDeclarationSymbol);
                }

                // PackMatrixRowMajor is always automatically enabled
                return options | D2D1CompileOptions.PackMatrixRowMajor;
            }

            if (structDeclarationSymbol.ContainingAssembly.TryGetAttributeWithFullyQualifiedMetadataName("ComputeSharp.D2D1.D2DCompileOptionsAttribute", out attributeData))
            {
                // No need to validate against PackMatrixColumnMajor as that's checked separately
                return (D2D1CompileOptions)attributeData.ConstructorArguments[0].Value! | D2D1CompileOptions.PackMatrixRowMajor;
            }

            return null;
        }

        /// <summary>
        /// Extracts the metadata definition for the current shader.
        /// </summary>
        /// <param name="structDeclarationSymbol">The input <see cref="INamedTypeSymbol"/> instance to process.</param>
        /// <param name="inputCount">The number of inputs for the shader.</param>
        /// <returns>Whether the shader only has simple inputs.</returns>
        public static bool IsSimpleInputShader(INamedTypeSymbol structDeclarationSymbol, int inputCount)
        {
            // If there are no inputs, the shader is as if only had simple inputs
            if (inputCount == 0)
            {
                return true;
            }

            // Build a map of all simple inputs (unmarked inputs default to being complex)
            bool[] simpleInputsMap = new bool[inputCount];

            foreach (AttributeData attributeData in structDeclarationSymbol.GetAttributes())
            {
                switch (attributeData.AttributeClass?.GetFullyQualifiedMetadataName())
                {
                    // Only retrieve indices of simple inputs that are in range. If an input is out of
                    // range, the diagnostic for it will already be emitted by a previous generator step.
                    case "ComputeSharp.D2D1.D2DInputSimpleAttribute"
                    when attributeData.ConstructorArguments[0].Value is int index && index < inputCount:
                        simpleInputsMap[index] = true;
                        break;
                }
            }

            return simpleInputsMap.All(static x => x);
        }

        /// <summary>
        /// Gets an <see cref="ImmutableArray{T}"/> instance with the compiled bytecode for the current shader.
        /// </summary>
        /// <param name="sourceInfo">The source info for the shader to compile.</param>
        /// <param name="token">The <see cref="CancellationToken"/> used to cancel the operation, if needed.</param>
        /// <param name="shaderProfile">The effective shader profile used to create the shader bytecode, or the default profile to use.</param>
        /// <param name="compileOptions">The effective compile options used to create the shader bytecode, or the default options to use.</param>
        /// <param name="diagnostic">The resulting diagnostic from the processing operation, if any.</param>
        /// <returns>The <see cref="ImmutableArray{T}"/> instance with the compiled shader bytecode.</returns>
        public static unsafe ImmutableArray<byte> GetBytecode(
            HlslShaderSourceInfo sourceInfo,
            CancellationToken token,
            out D2D1ShaderProfile shaderProfile,
            out D2D1CompileOptions compileOptions,
            out DeferredDiagnosticInfo? diagnostic)
        {
            // Set the shader profile to either be the requested one, or the default value (which maps to PS5.0)
            shaderProfile = sourceInfo.ShaderProfile ?? D2D1ShaderProfile.PixelShader50;

            // If an explicit set of compile options is provided, use that directly. Otherwise, use the default
            // options plus the option to enable linking only if the shader can potentially support it.
            compileOptions = sourceInfo.CompileOptions ?? (D2D1CompileOptions.Default | (sourceInfo.IsLinkingSupported ? D2D1CompileOptions.EnableLinking : 0));

            ImmutableArray<byte> bytecode = ImmutableArray<byte>.Empty;

            // No embedded shader was requested, or there were some errors earlier in the pipeline.
            // In this case, skip the compilation, as diagnostic will be emitted for those anyway.
            // Compiling would just add overhead and result in more errors, as the HLSL would be invalid.
            if (sourceInfo is { HasErrors: true } or { ShaderProfile: null })
            {
                diagnostic = null;

                goto End;
            }

            try
            {
                token.ThrowIfCancellationRequested();

                // Compile the shader bytecode using the requested parameters
                using ComPtr<ID3DBlob> dxcBlobBytecode = D3DCompiler.Compile(
                    sourceInfo.HlslSource.AsSpan(),
                    sourceInfo.ShaderProfile.Value,
                    compileOptions);

                token.ThrowIfCancellationRequested();

                byte* buffer = (byte*)dxcBlobBytecode.Get()->GetBufferPointer();
                int length = checked((int)dxcBlobBytecode.Get()->GetBufferSize());

                byte[] array = new ReadOnlySpan<byte>(buffer, length).ToArray();

                bytecode = Unsafe.As<byte[], ImmutableArray<byte>>(ref array);
                diagnostic = null;
            }
            catch (Win32Exception e)
            {
                compileOptions = default;
                diagnostic = DeferredDiagnosticInfo.Create(EmbeddedBytecodeFailedWithWin32Exception, e.HResult, FixupExceptionMessage(e.Message));
            }
            catch (FxcCompilationException e)
            {
                compileOptions = default;
                diagnostic = DeferredDiagnosticInfo.Create(EmbeddedBytecodeFailedWithFxcCompilationException, FixupExceptionMessage(e.Message));
            }

            End:
            return bytecode;
        }

        /// <summary>
        /// Fixes up an exception message to improve the way it's displayed in VS.
        /// </summary>
        /// <param name="message">The input exception message.</param>
        /// <returns>The updated exception message.</returns>
        internal static string FixupExceptionMessage(string message)
        {
            // Add square brackets around error headers
            message = Regex.Replace(message, @"((?:error|warning) \w+):", static m => $"[{m.Groups[1].Value}]:");

            return message.NormalizeToSingleLine();
        }
    }
}