using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using System.Text;

public static class KeyboardHandler
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void PfnDelegateTest();
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void PfnKeyEvent(Int32 vk);

    [DllImport("keycapturer.dll", CallingConvention = CallingConvention.Winapi)]
    public static extern void SetProc(PfnKeyEvent proc);
    
    [DllImport("keycapturer.dll", CallingConvention = CallingConvention.Winapi)]
    public static extern void DeProc();

    public static event PfnKeyEvent GlobalKeyDown;
    private static void KeyProcessor(Int32 vk)
    {
        GlobalKeyDown?.Invoke(vk);
    }
    public static void StartCapture()
    {
        SetProc(KeyProcessor);
    }
    public static void StopCapture()
    {
        DeProc();
    }
}
