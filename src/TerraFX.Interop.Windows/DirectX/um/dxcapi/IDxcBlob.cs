// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/dxcapi.h in the Windows SDK for Windows 10.0.20348.0
// Original source is Copyright © Microsoft. All rights reserved. Licensed under the University of Illinois Open Source License.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[Guid("8BA5FB08-5195-40E2-AC58-0D989C3A0102")]
[NativeTypeName("struct IDxcBlob : IUnknown")]
[NativeInheritance("IUnknown")]
internal unsafe partial struct IDxcBlob
{
    public void** lpVtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged[Stdcall]<IDxcBlob*, Guid*, void**, int>)(lpVtbl[0]))((IDxcBlob*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged[Stdcall]<IDxcBlob*, uint>)(lpVtbl[1]))((IDxcBlob*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged[Stdcall]<IDxcBlob*, uint>)(lpVtbl[2]))((IDxcBlob*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(3)]
    [return: NativeTypeName("LPVOID")]
    public void* GetBufferPointer()
    {
        return ((delegate* unmanaged[Stdcall]<IDxcBlob*, void*>)(lpVtbl[3]))((IDxcBlob*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(4)]
    [return: NativeTypeName("SIZE_T")]
    public nuint GetBufferSize()
    {
        return ((delegate* unmanaged[Stdcall]<IDxcBlob*, nuint>)(lpVtbl[4]))((IDxcBlob*)Unsafe.AsPointer(ref this));
    }
}