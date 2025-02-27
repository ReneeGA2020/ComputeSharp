// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from winrt/roapi.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.WinRT;

internal static unsafe partial class WinRT
{
    [DllImport("combase", ExactSpelling = true)]
    public static extern HRESULT RoGetActivationFactory(HSTRING activatableClassId, [NativeTypeName("const IID &")] Guid* iid, void** factory);
}