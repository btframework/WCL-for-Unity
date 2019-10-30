using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

// This class contains all Bluetooth Framework error codes (copied from .NET Edition).
// You can find more information about error codes (full description)
// on our site: https://www.btframework.com/errors.htm
public class BluetoothErrors
{
    // Use just few error defines here. Refer to full errors list for other error codes.
    public const Int32 WCL_E_SUCCESS = 0x00000000;

    public const Int32 WCL_E_BASE = 0x00010000;
    public const Int32 WCL_E_INVALID_ARGUMENT = WCL_E_BASE + 0x0000;
    public const Int32 WCL_E_OUT_OF_MEMORY = WCL_E_BASE + 0x0001;

    public const Int32 WCL_E_CONNECTION_BASE = 0x00030000;
    public const Int32 WCL_E_CONNECTION_ACTIVE = WCL_E_CONNECTION_BASE + 0x0000;
    public const Int32 WCL_E_CONNECTION_NOT_ACTIVE = WCL_E_CONNECTION_BASE + 0x0001;
    public const Int32 WCL_E_CONNECTION_UNABLE_CREATE_TERMINATE_EVENT = WCL_E_CONNECTION_BASE + 0x0002;
    public const Int32 WCL_E_CONNECTION_UNABLE_START_COMMUNICATION = WCL_E_CONNECTION_BASE + 0x0004;
    public const Int32 WCL_E_CONNECTION_UNABLE_CREATE_INIT_EVENT = WCL_E_CONNECTION_BASE + 0x0007;
};

// The base class for Bluetooth Framework C++ Edition wrappers.
// It must be disposable.
public abstract class BluetoothImports : IDisposable
{
    private Boolean FDisposed;

    private void Dispose(Boolean Disposing)
    {
        if (FDisposed)
            return;

        Free();

        FDisposed = true;
    }

    // This can be overridden. Called when object is disposing.
    protected virtual void Free()
    {

    }

    protected Boolean Disposed { get { return FDisposed; } }

    protected internal const String WclGattClientDllName = "..\\WclGattClientDll\\build\\WclGattClientDll.dll";

    public BluetoothImports()
    {
        FDisposed = false;
    }

    ~BluetoothImports()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
};

public sealed class Helpers : BluetoothImports
{
    #region Helper function imports
    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static extern UInt32 AlertableWait(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Handle,
        [param: MarshalAs(UnmanagedType.Bool), In] Boolean Infinite);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    public static extern void ProcessApc();

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    public static extern void SetApcSync();
    #endregion

    #region Win32 constants
    public const UInt32 WAIT_OBJECT_0 = 0;
    public const UInt32 WAIT_IO_COMPLETION = 0x000000C0;
    public const UInt32 WAIT_TIMEOUT = 0x00000102;
    public const UInt32 WAIT_FAILED = 0xFFFFFFFF;
    #endregion
};

#region Bluetooth Manager delegates
public delegate void DeviceFoundEvent(System.Object sender, IntPtr Radio, Int64 Address);
public delegate void DiscoveringStartedEvent(System.Object sender, IntPtr Radio);
public delegate void DiscoveringCompletedEvent(System.Object sender, IntPtr Radio, Int32 Error);
#endregion

public sealed class BluetoothManager : BluetoothImports
{
    #region Bluetooth Manager callback types
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void NOTIFY_EVENT(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr sender);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void DEVICE_FOUND_EVENT(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr sender,
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio,
        [param: MarshalAs(UnmanagedType.I8), In] Int64 Address);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void DISCOVERING_STARTED_EVENT(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr sender,
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void DISCOVERING_COMPLETED_EVENT(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr sender,
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio,
        [param: MarshalAs(UnmanagedType.I4), In] Int32 Error);
    #endregion

    #region Bluetooth Manager imports
    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static extern IntPtr ManagerCreate();

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void ManagerDestroy(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 ManagerOpen(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 ManagerClose(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 ManagerGetRadioCount(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static extern IntPtr ManagerGetRadio(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager,
        [param: MarshalAs(UnmanagedType.I4)] Int32 Index);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void ManagerSetAfterOpen(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] NOTIFY_EVENT Event);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void ManagerSetBeforeClose(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] NOTIFY_EVENT Event);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void ManagerSetOnDeviceFound(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] DEVICE_FOUND_EVENT Event);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void ManagerSetOnDiscoveringStarted(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] DISCOVERING_STARTED_EVENT Event);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void ManagerSetOnDiscoveringCompleted(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Manager,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] DISCOVERING_COMPLETED_EVENT Event);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 RadioDiscover(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio,
        [param: MarshalAs(UnmanagedType.U4), In] UInt32 Timeout);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 RadioTerminate(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern Boolean RadioIsAvailable(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 RadioGetDeviceName(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio,
        [param: MarshalAs(UnmanagedType.I8), In] Int64 Address,
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Name,
        [param: MarshalAs(UnmanagedType.I4), In] Int32 Len);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 RadioRemoteUnpair(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio,
        [param: MarshalAs(UnmanagedType.I8), In] Int64 Address);
    #endregion

    // Bluetooth Manager instance.
    private IntPtr FManager;

    #region Bluetooth Manager callback delegates
    private NOTIFY_EVENT FAfterOpen;
    private NOTIFY_EVENT FBeforeClose;
    private DEVICE_FOUND_EVENT FOnDeviceFound;
    private DISCOVERING_STARTED_EVENT FOnDiscoveringStarted;
    private DISCOVERING_COMPLETED_EVENT FOnDiscoveringCompleted;
    #endregion

    #region Bluetooth Manager callback handlers
    private void DoAfterOpen(IntPtr sender)
    {
        if (AfterOpen != null)
            AfterOpen(this, EventArgs.Empty);
    }

    private void DoBeofreClose(IntPtr sender)
    {
        if (BeforeClose != null)
            BeforeClose(this, EventArgs.Empty);
    }

    private void DoDeviceFound(IntPtr sender, IntPtr Radio, Int64 Address)
    {
        if (OnDeviceFound != null)
            OnDeviceFound(this, Radio, Address);
    }

    private void DoDiscoveringStarted(IntPtr sender, IntPtr Radio)
    {
        if (OnDiscoveringStarted != null)
            OnDiscoveringStarted(this, Radio);
    }

    private void DoDiscoveringCompleted(IntPtr sender, IntPtr Radio, Int32 Error)
    {
        if (OnDiscoveringCompleted != null)
            OnDiscoveringCompleted(this, Radio, Error);
    }
    #endregion

    protected override void Free()
    {
        // Call close first if we did not do it yet.
        Close();

        // Cleanup callbacks.
        ManagerSetAfterOpen(FManager, null);
        ManagerSetBeforeClose(FManager, null);
        ManagerSetOnDeviceFound(FManager, null);
        ManagerSetOnDiscoveringCompleted(FManager, null);
        ManagerSetOnDiscoveringStarted(FManager, null);
        
        // Dispose delegates.
        FAfterOpen = null;
        FBeforeClose = null;
        FOnDeviceFound = null;
        FOnDiscoveringStarted = null;
        FOnDiscoveringCompleted = null;
        
        // Delete Bluetooth Manager instance.
        ManagerDestroy(FManager);

        // And that's all
        FManager = IntPtr.Zero;
    }

    public BluetoothManager()
        : base()
    {
        AfterOpen = null;
        BeforeClose = null;
        OnDeviceFound = null;
        OnDiscoveringStarted = null;
        OnDiscoveringCompleted = null;
        
        // Initialize callback delegates with its handlers.
        FAfterOpen = new NOTIFY_EVENT(DoAfterOpen);
        FBeforeClose = new NOTIFY_EVENT(DoBeofreClose);
        FOnDeviceFound = new DEVICE_FOUND_EVENT(DoDeviceFound);
        FOnDiscoveringCompleted = new DISCOVERING_COMPLETED_EVENT(DoDiscoveringCompleted);
        FOnDiscoveringStarted = new DISCOVERING_STARTED_EVENT(DoDiscoveringStarted);
        
        // Create Bluetooth Manager instance...
        FManager = ManagerCreate();
        // ...and setup callbacks.
        ManagerSetAfterOpen(FManager, FAfterOpen);
        ManagerSetBeforeClose(FManager, FBeforeClose);
        ManagerSetOnDeviceFound(FManager, FOnDeviceFound);
        ManagerSetOnDiscoveringCompleted(FManager, FOnDiscoveringCompleted);
        ManagerSetOnDiscoveringStarted(FManager, FOnDiscoveringStarted);
    }

    #region Bluetooth Manager methods
    public Int32 Open()
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return ManagerOpen(FManager);
    }

    public Int32 Close()
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return ManagerClose(FManager);
    }

    public Int32 Discover(IntPtr Radio, UInt32 Timeout)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return RadioDiscover(Radio, Timeout);
    }

    public Int32 Terminate(IntPtr Radio)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return RadioTerminate(Radio);
    }

    public Boolean IsRadioAvailable(IntPtr Radio)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return RadioIsAvailable(Radio);
    }

    public Int32 GetRemoteName(IntPtr Radio, Int64 Address, out String Name)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        Name = "";

        // Bluetooth device's name can not be longer that 128 chars. To be 100% sure we will use 256 chars
        // memory block.
        IntPtr pName;
        Int32 Result;

        pName = Marshal.AllocHGlobal(256 * sizeof(Char));
        try
        {
            Result = RadioGetDeviceName(Radio, Address, pName, 256);
            if (Result == BluetoothErrors.WCL_E_SUCCESS)
                Name = Marshal.PtrToStringUni(pName);
        }
        finally
        {
            Marshal.FreeHGlobal(pName);
        }

        return Result;
    }

    public Int32 RemoteUnpair(IntPtr Radio, Int64 Address)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return RadioRemoteUnpair(Radio, Address);
    }
    #endregion

    #region Bluetooth Manager properties
    public Int32 Count
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return ManagerGetRadioCount(FManager);
        }
    }

    public IntPtr this[Int32 Index]
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return ManagerGetRadio(FManager, Index);
        }
    }
    #endregion

    #region Bluetooth Manager events
    public event EventHandler AfterOpen;
    public event EventHandler BeforeClose;
    public event DeviceFoundEvent OnDeviceFound;
    public event DiscoveringStartedEvent OnDiscoveringStarted;
    public event DiscoveringCompletedEvent OnDiscoveringCompleted;
    #endregion
};

#region GATT data types.
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GattUuid
{
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean IsShortUuid;
    [MarshalAs(UnmanagedType.U2)]
    public UInt16 ShortUuid;
    public Guid LongUuid;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GattService
{
    public GattUuid Uuid;
    [MarshalAs(UnmanagedType.U2)]
    public UInt16 Handle;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GattServices
{
    [MarshalAs(UnmanagedType.U1)]
    public Byte Count;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
    public GattService[] Services;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GattCharacteristic
{
    [MarshalAs(UnmanagedType.U2)]
    public UInt16 ServiceHandle;
    public GattUuid Uuid;
    [MarshalAs(UnmanagedType.U2)]
    public UInt16 Handle;
    [MarshalAs(UnmanagedType.U2)]
    public UInt16 ValueHandle;
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean IsBroadcastable;
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean IsReadable;
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean IsWritable;
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean IsWritableWithoutResponse;
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean IsSignedWritable;
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean IsNotifiable;
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean IsIndicatable;
    [MarshalAs(UnmanagedType.Bool)]
    public Boolean HasExtendedProperties;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GattCharacteristics
{
    [MarshalAs(UnmanagedType.U1)]
    public Byte Count;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
    public GattCharacteristic[] Chars;
};
#endregion

#region GATT Enumerations
public enum ClientState
{
    csDisconnected,
    csPreparing,
    csConnecting,
    csConnected,
    csDisconnecting
};

public enum GattOperationFlag
{
    goNone,
    goReadFromDevice,
    goReadFromCache
};

public enum GattProtectionLevel
{
    plNone,
    plAuthentication,
    plEncryption,
    plEncryptionAndAuthentication
};
#endregion

#region GATT client delegates
public delegate void ClietConnect(System.Object sender, Int32 Error);
public delegate void ClientDisconnect(System.Object sender, Int32 Reason);
public delegate void ClientChanged(System.Object sender, UInt16 Handle, Byte[] Value);
#endregion

#region GATT client callback types.
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void GATTCLIENT_CONNECT(
    [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr sender,
    [param: MarshalAs(UnmanagedType.I4), In] Int32 Error);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void GATTCLIENT_DISCONNECT(
    [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr sender,
    [param: MarshalAs(UnmanagedType.I4), In] Int32 Reason);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void GATTCLIENT_ONCHANGED(
    [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr sender,
    [param: MarshalAs(UnmanagedType.U2), In] UInt16 Handle,
    [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Value,
    [param: MarshalAs(UnmanagedType.U4), In] UInt32 ValueLen);
#endregion

public sealed class GattClientConnection : BluetoothImports
{
    #region GATT client imports
    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static extern IntPtr GattClientCreate();

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void GattClientDestroy(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void GattClientSetOnConnect(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] GATTCLIENT_CONNECT Event);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void GattClientSetOnDisconnect(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] GATTCLIENT_DISCONNECT Event);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void GattClientSetOnChanged(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] GATTCLIENT_ONCHANGED Event);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientConnect(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Radio,
        [param: MarshalAs(UnmanagedType.I8), In] Int64 Address);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientDisconnect(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientGetServices(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: In, Out] ref GattServices Services);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientGetCharas(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: In] ref GattService Service,
        [param: In, Out] ref GattCharacteristics Chars);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientSubscribe(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: In] ref GattCharacteristic Char);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientUnsubscribe(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: In] ref GattCharacteristic Char);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientReadCharacteristicValue(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: In] ref GattCharacteristic Char,
        [param: MarshalAs(UnmanagedType.SysInt), In, Out] ref IntPtr ppValue,
        [param: MarshalAs(UnmanagedType.U4), In, Out] ref UInt32 pSize);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientWriteCharacteristicValue(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client,
        [param: In] ref GattCharacteristic Char,
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr pValue,
        [param: MarshalAs(UnmanagedType.U4), In] UInt32 Size);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void GattClientFreeMem(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr pMem);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern ClientState GattClientGetState(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Client);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GattClientWriteClientConfiguration(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr ClientClient,
        [param: In] ref GattCharacteristic Char,
        [param: MarshalAs(UnmanagedType.Bool), In] Boolean Subscribe,
        [param: In] GattOperationFlag Flag,
        [param: In] GattProtectionLevel Protection);
    #endregion

    // GATT Client instance.
    private IntPtr FClient;

    #region GATT client delegates
    private GATTCLIENT_CONNECT FOnConnect;
    private GATTCLIENT_DISCONNECT FOnDisconnect;
    private GATTCLIENT_ONCHANGED FOnChanged;
    #endregion

    #region GATT client callback handlers
    private void DoConnect(IntPtr sender, Int32 Error)
    {
        if (OnConnect != null)
            OnConnect(this, Error);
    }

    private void DoDisconnect(IntPtr sender, Int32 Reason)
    {
        if (OnDisconnect != null)
            OnDisconnect(this, Reason);
    }

    private void DoChanged(IntPtr sender, UInt16 Handle, IntPtr Value, UInt32 ValueLen)
    {
        if (OnChanged != null)
        {
            Byte[] v = null;
            if (Value != IntPtr.Zero && ValueLen > 0)
            {
                v = new Byte[ValueLen];
                Marshal.Copy(Value, v, 0, (int)ValueLen);
            }
            OnChanged(this, Handle, v);
        }
    }
    #endregion

    protected override void Free()
    {
        // Call close first if we did not do it yet.
        Disconnect();

        // Cleanup callbacks.
        GattClientSetOnChanged(FClient, null);
        GattClientSetOnConnect(FClient, null);
        GattClientSetOnDisconnect(FClient, null);

        // Dispose delegates.
        FOnConnect = null;
        FOnDisconnect = null;
        FOnChanged = null;

        // Delete Bluetooth Manager instance.
        GattClientDestroy(FClient);

        // And that's all
        FClient = IntPtr.Zero;
    }

    public GattClientConnection()
        : base()
    {
        OnConnect = null;
        OnDisconnect = null;
        OnChanged = null;
        
        // Initialize callback delegates with its handlers.
        FOnConnect = new GATTCLIENT_CONNECT(DoConnect);
        FOnDisconnect = new GATTCLIENT_DISCONNECT(DoDisconnect);
        FOnChanged = new GATTCLIENT_ONCHANGED(DoChanged);
        
        // Create Bluetooth Manager instance...
        FClient = GattClientCreate();
        // ...and setup callbacks.
        GattClientSetOnChanged(FClient, FOnChanged);
        GattClientSetOnConnect(FClient, FOnConnect);
        GattClientSetOnDisconnect(FClient, FOnDisconnect);
    }

    #region GATT client methods
    public Int32 Connect(IntPtr Radio, Int64 Address)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return GattClientConnect(FClient, Radio, Address);
    }

    public Int32 Disconnect()
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return GattClientDisconnect(FClient);
    }

    public Int32 GetServices(out GattServices Services)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        Services = new GattServices();
        Services.Count = 0;
        Services.Services = new GattService[255];

        return GattClientGetServices(FClient, ref Services);
    }

    public Int32 GetCharacteristics(GattService Service, out GattCharacteristics Chars)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        Chars = new GattCharacteristics();
        Chars.Count = 0;
        Chars.Chars = new GattCharacteristic[255];

        return GattClientGetCharas(FClient, ref Service, ref Chars);
    }

    public Int32 Subscribe(GattCharacteristic Char)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return GattClientSubscribe(FClient, ref Char);
    }

    public Int32 Unsubscribe(GattCharacteristic Char)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return GattClientUnsubscribe(FClient, ref Char);
    }

    public Int32 ReadValue(GattCharacteristic Char, out Byte[] Value)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        Value = null;

        IntPtr ppValue = IntPtr.Zero;
        UInt32 pSize = 0;
        Int32 Result = GattClientReadCharacteristicValue(FClient, ref Char, ref ppValue, ref pSize);
        if (Result == BluetoothErrors.WCL_E_SUCCESS && ppValue != IntPtr.Zero && pSize > 0)
        {
            Value = new Byte[pSize];
            Marshal.Copy(ppValue, Value, 0, (Int32)pSize);
            GattClientFreeMem(ppValue);
        }
        return Result;
    }

    public Int32 WriteValue(GattCharacteristic Char, Byte[] Value)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        IntPtr pValue;
        try { pValue = Marshal.AllocHGlobal(Value.Length); } catch { pValue = IntPtr.Zero; }
        if (pValue == IntPtr.Zero)
            return BluetoothErrors.WCL_E_OUT_OF_MEMORY;

        Marshal.Copy(Value, 0, pValue, Value.Length);
        Int32 Res = GattClientWriteCharacteristicValue(FClient, ref Char, pValue, (UInt32)Value.Length);
        Marshal.FreeHGlobal(pValue);
        return Res;
    }

    public Int32 WriteClientConfiguration(GattCharacteristic Char, Boolean Subscribe,
        GattOperationFlag Flag, GattProtectionLevel Protection)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return GattClientWriteClientConfiguration(FClient, ref Char, Subscribe, Flag, Protection);
    }
    #endregion

    #region GATT client properties
    public ClientState State
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return GattClientGetState(FClient);
        }
    }
    #endregion

    #region GATT client events.
    public event ClietConnect OnConnect;
    public event ClientDisconnect OnDisconnect;
    public event ClientChanged OnChanged;
    #endregion
};

public sealed class MessageReceiver : BluetoothImports
{
    #region Message receiver imports
    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static extern IntPtr MessageReciverCreate(
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] GATTCLIENT_CONNECT OnConnect,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] GATTCLIENT_DISCONNECT OnDisconnect,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] GATTCLIENT_ONCHANGED OnChanged);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void MessageReciverDestroy(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Receiver);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 MessageReciverOpen(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Receiver);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 MessageReciverClose(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Receiver);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void MessageReceiverNotifyConnect(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Receiver,
        [param: MarshalAs(UnmanagedType.I4), In] Int32 Error);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void MessageReceiverNotifyDisconnect(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Receiver,
        [param: MarshalAs(UnmanagedType.I4), In] Int32 Result);

    [DllImport(WclGattClientDllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void MessageReceiverNotifyChanged(
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Receiver,
        [param: MarshalAs(UnmanagedType.U2), In] UInt16 Handle,
        [param: MarshalAs(UnmanagedType.SysInt), In] IntPtr Value,
        [param: MarshalAs(UnmanagedType.U2), In] UInt16 ValueLen);
    #endregion

    private IntPtr FReceiver;

    #region Delegates
    private GATTCLIENT_CONNECT FOnConnect;
    private GATTCLIENT_DISCONNECT FOnDisconnect;
    private GATTCLIENT_ONCHANGED FOnChanged;
    #endregion

    #region Callbacks
    private void DoConnect(IntPtr sender, Int32 Error)
    {
        if (OnConnect != null)
            OnConnect(this, Error);
    }

    private void DoDisconnect(IntPtr sender, Int32 Reason)
    {
        if (OnDisconnect != null)
            OnDisconnect(this, Reason);
    }

    private void DoChanged(IntPtr sender, UInt16 Handle, IntPtr Value, UInt32 ValueLen)
    {
        if (OnChanged != null)
        {
            Byte[] v = null;
            if (Value != IntPtr.Zero && ValueLen > 0)
            {
                v = new Byte[ValueLen];
                Marshal.Copy(Value, v, 0, (int)ValueLen);
            }
            OnChanged(this, Handle, v);
        }
    }
    #endregion

    protected override void Free()
    {
        MessageReciverClose(FReceiver);
        MessageReciverDestroy(FReceiver);

        FReceiver = IntPtr.Zero;

        FOnChanged = null;
        FOnChanged = null;
        FOnDisconnect = null;
    }

    public MessageReceiver()
    {
        OnConnect = null;
        OnDisconnect = null;
        OnChanged = null;

        FOnConnect = new GATTCLIENT_CONNECT(DoConnect);
        FOnDisconnect = new GATTCLIENT_DISCONNECT(DoDisconnect);
        FOnChanged = new GATTCLIENT_ONCHANGED(DoChanged);

        FReceiver = MessageReciverCreate(FOnConnect, FOnDisconnect, FOnChanged);
    }

    public Int32 Open()
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return MessageReciverOpen(FReceiver);
    }

    public Int32 Close()
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        return MessageReciverClose(FReceiver);
    }

    public void NotifyConnect(Int32 Error)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        MessageReceiverNotifyConnect(FReceiver, Error);
    }

    public void NotifyDisconnect(Int32 Reason)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        MessageReceiverNotifyDisconnect(FReceiver, Reason);
    }

    public void NotifyChanged(UInt16 Handle, Byte[] Value)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        IntPtr Val = IntPtr.Zero;
        UInt16 Len = 0;
        if (Value != null && Value.Length > 0)
        {
            Len = (UInt16)Value.Length;
            Val = Marshal.AllocHGlobal(Len);
            for (int i = 0; i < Len; i++)
                Marshal.WriteByte(Val, i, Value[i]);
        }
        MessageReceiverNotifyChanged(FReceiver, Handle, Val, Len);
        if (Val != IntPtr.Zero)
            Marshal.FreeHGlobal(Val);
    }

    #region Events.
    public event ClietConnect OnConnect;
    public event ClientDisconnect OnDisconnect;
    public event ClientChanged OnChanged;
    #endregion
};

// The class implements GATT client communication thread.
public class GattClientThread : BluetoothImports
{
    // Address used to connect.
    private Int64 FAddress;
    // GATT Client object.
    private GattClientConnection FClient;
    // Indicate should we use OnDisconnect event.
    private Boolean FDoDisconnect;
    // The event used to indicate that thread initialization has been completed.
    private ManualResetEvent FInitEvent;
    // Thread initialization result.
    private Int32 FInitResult;
    // The Radio object used to connect.
    private IntPtr FRadio;
    // Synchronization message receiver.
    private MessageReceiver FReceiver;
    // The thread termination event.
    private ManualResetEvent FTerminationEvent;
    // The communication thread object.
    private Thread FThread;

    #region Client events
    private void ClientConnect(System.Object sender, Int32 Error)
    {
        FDoDisconnect = false;

        if (Error == BluetoothErrors.WCL_E_SUCCESS)
        {
            // Try to read services
            GattServices Services;
            Int32 Res = FClient.GetServices(out Services);
            if (Res == BluetoothErrors.WCL_E_SUCCESS)
            {
                // Read characteristics for each service and try to subscribe for
                // notifications.
                for (Int32 i = 0; i < Services.Count; i++)
                {
                    GattCharacteristics Chars;
                    Res = FClient.GetCharacteristics(Services.Services[i], out Chars);
                    if (Res == BluetoothErrors.WCL_E_SUCCESS)
                    {
                        for (Int32 j = 0; j < Chars.Count; j++)
                        {
                            GattCharacteristic Ch = Chars.Chars[j];
                            if (Ch.IsIndicatable || Ch.IsNotifiable)
                            {
                                // Only one flag is allowed!
                                if (Ch.IsIndicatable && Ch.IsNotifiable)
                                    Ch.IsIndicatable = false;
                                // try to subscribe.
                                Res = FClient.Subscribe(Ch);
                                if (Res == BluetoothErrors.WCL_E_SUCCESS)
                                {
                                    Res = FClient.WriteClientConfiguration(Ch, true, GattOperationFlag.goNone, GattProtectionLevel.plNone);
                                    if (Res != BluetoothErrors.WCL_E_SUCCESS)
                                        FClient.Unsubscribe(Ch);
                                }
                            }
                        }
                    }
                }
            }

            // If was not able to read all required data simple disconnect.
            if (Error != BluetoothErrors.WCL_E_SUCCESS)
                FClient.Disconnect();
            else
                // We need OnDisconnect event!
                FDoDisconnect = true;
        }

        // Send synchronized notification.
        FReceiver.NotifyConnect(Error);
    }

    private void ClientDisconnect(System.Object sender, Int32 Reason)
    {
        // Send synchronized notification.
        if (FDoDisconnect)
            FReceiver.NotifyDisconnect(Reason);
    }

    private void ClientChanged(System.Object sender, UInt16 Handle, Byte[] Value)
    {
        // Send synchronized notification.
        FReceiver.NotifyChanged(Handle, Value);
    }
    #endregion

    #region Message receiver notifications
    private void ReceiverChanged(System.Object sender, UInt16 Handle, Byte[] Value)
    {
        DoChanged(Handle, Value);
    }

    private void ReceiverDisconnect(System.Object sender, Int32 Reason)
    {
        Disconnect();

        DoDisconnect(Reason);
    }

    private void ReceiverConnect(System.Object sender, Int32 Error)
    {
        DoConnect(Error);
        if (Error != BluetoothErrors.WCL_E_SUCCESS)
            Disconnect();
    }
    #endregion

    private void ThreadProc()
    {
        // Create client.
        FClient = new GattClientConnection();
        // Setup event handlers.
        FClient.OnConnect += ClientConnect;
        FClient.OnDisconnect += ClientDisconnect;
        FClient.OnChanged += ClientChanged;

        // Try to conenct.
        FInitResult = FClient.Connect(FRadio, FAddress);
        // Set intialization completed event.
        FInitEvent.Set();

        if (FInitResult == BluetoothErrors.WCL_E_SUCCESS)
        {
            // If connection started go into termination loop. Use alertabel wait to process APC.
            while (Helpers.AlertableWait(FTerminationEvent.SafeWaitHandle.DangerousGetHandle(), true) != Helpers.WAIT_OBJECT_0)
                ;

            // We have to disconnect here!
            FClient.Disconnect();
        }

        // Once thread terminated - release client.
        FClient.Dispose();
        FClient = null;
    }

    protected override void Free()
    {
        Disconnect();

        FReceiver.Dispose();
        FReceiver = null;
    }

    // Fires the OnConnet event.
    protected virtual void DoConnect(Int32 Error)
    {
        if (OnConnect != null)
            OnConnect(this, Error);
    }
    
    // Fires the OnDisconnect event.
    protected virtual void DoDisconnect(Int32 Reason)
    {
        if (OnDisconnect != null)
            OnDisconnect(this, Reason);
    }

    // Fires the OnChanged event.
    protected virtual void DoChanged(UInt16 Handle, Byte[] Value)
    {
        if (OnChanged != null)
            OnChanged(this, Handle, Value);
    }
    
    public GattClientThread()
    {
        FAddress = 0;
        FClient = null;
        FInitEvent = null;
        FInitResult = BluetoothErrors.WCL_E_SUCCESS;
        FRadio = IntPtr.Zero;
        FTerminationEvent = null;
        FThread = null;

        FReceiver = new global::MessageReceiver();
        FReceiver.OnChanged += ReceiverChanged;
        FReceiver.OnConnect += ReceiverConnect;
        FReceiver.OnDisconnect += ReceiverDisconnect;

        OnConnect = null;
        OnDisconnect = null;
        OnChanged = null;
    }

    // Starts connecting to the specified device.
    public Int32 Connect(IntPtr Radio, Int64 Address)
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        if (Connected)
            return BluetoothErrors.WCL_E_CONNECTION_ACTIVE;
        if (Address == 0 || Radio == IntPtr.Zero)
            return BluetoothErrors.WCL_E_INVALID_ARGUMENT;

        // Try to open message receiver.
        Int32 Result = FReceiver.Open();
        if (Result == BluetoothErrors.WCL_E_SUCCESS)
        {
            FAddress = Address;
            FRadio = Radio;

            // Try to create initialization event.
            try { FInitEvent = new ManualResetEvent(false); } catch { }
            if (FInitEvent == null)
                Result = BluetoothErrors.WCL_E_CONNECTION_UNABLE_CREATE_INIT_EVENT;
            else
            {
                // try to create termination event.
                try { FTerminationEvent = new ManualResetEvent(false); } catch { }
                if (FTerminationEvent == null)
                    Result = BluetoothErrors.WCL_E_CONNECTION_UNABLE_CREATE_TERMINATE_EVENT;
                else
                {
                    // Try to start thread.
                    try { FThread = new Thread(ThreadProc); } catch { }
                    if (FThread == null)
                        Result = BluetoothErrors.WCL_E_CONNECTION_UNABLE_START_COMMUNICATION;
                    else
                    {
                        // Do not forget to start thread!!!
                        FThread.Start();

                        // Wait for initialization complete.
                        FInitEvent.WaitOne();

                        // Copy result.
                        Result = FInitResult;

                        // Check initialization result.
                        if (Result != BluetoothErrors.WCL_E_SUCCESS)
                            FThread = null;
                    }

                    // If something went wrong we must close termination event.
                    if (Result != BluetoothErrors.WCL_E_SUCCESS)
                    {
                        FTerminationEvent.Close();
                        FTerminationEvent = null;
                    }
                }

                // Any way we can close initialization event cause we do not need it.
                FInitEvent.Close();
                FInitEvent = null;
            }

            // If something went wrong we must clear internal fields.
            if (Result != BluetoothErrors.WCL_E_SUCCESS)
            {
                FAddress = 0;
                FRadio = IntPtr.Zero;
                // ... and close the receiver.
                FReceiver.Close();
            }
        }

        // If somethign went wrong we must dispose the message receiver.
        if (Result != BluetoothErrors.WCL_E_SUCCESS)
        {
            FReceiver.Dispose();
            FReceiver = null;
        }

        return Result;
    }

    // Disconnects
    public Int32 Disconnect()
    {
        if (Disposed)
            throw new ObjectDisposedException(GetType().FullName);

        if (!Connected)
            return BluetoothErrors.WCL_E_CONNECTION_NOT_ACTIVE;

        // Close message receiver first to prevent from any message receiving.
        FReceiver.Close();

        // Set termination event.
        FTerminationEvent.Set();
        // Wait for thread termination.
        FThread.Join();

        // Cleanup.
        FAddress = 0;
        FInitResult = BluetoothErrors.WCL_E_SUCCESS;
        FTerminationEvent.Close();
        FTerminationEvent = null;
        FRadio = IntPtr.Zero;
        FThread = null;

        return BluetoothErrors.WCL_E_SUCCESS;
    }

    // Gets the address used to connect to device.
    public Int64 Address
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return FAddress;
        }
    }

    // True if connect is active.
    public Boolean Connected
    {
        get
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return FThread != null;
        }
    }

    // The event fires when client connected (with or without success).
    public event ClietConnect OnConnect;
    // The event fires when client disconnected.
    public event ClientDisconnect OnDisconnect;
    // The event fires when characteristic value changed.
    public event ClientChanged OnChanged;
};

public class GattClient : MonoBehaviour
{
    private BluetoothManager FManager;
    private Dictionary<Int64, GattClientThread> FConnections;
    private List<Int64> FFoundDevices;
    
    private void OnDestroy()
    {
        // Close and sispose the Bluetooth Manager. 
        // When we close the Bluetooth Manager it also must close all connections.
        FManager.Close();
        FManager.Dispose();
        FManager = null;

        // Clear and release lists.
        FConnections.Clear();
        FConnections = null;

        FFoundDevices.Clear();
        FFoundDevices = null;
    }

    void Start()
    {
        // The very first thing we have to do is to switch to APC synchronization.
        Helpers.SetApcSync();

        FFoundDevices = new List<Int64>();
        FConnections = new Dictionary<Int64, GattClientThread>();

        // Create Bluetooth Manager.
        FManager = new BluetoothManager();
        FManager.OnDeviceFound += ManagerDeviceFound;
        FManager.OnDiscoveringCompleted += ManagerDiscoveringCompleted;
        FManager.OnDiscoveringStarted += ManagerDiscoveringStarted;
        FManager.AfterOpen += ManagerAfterOpen;
        FManager.BeforeClose += ManagerBeforeClose;

        // Try to open Bluetooth Manager.
        Int32 Res = FManager.Open();
        if (Res != BluetoothErrors.WCL_E_SUCCESS)
            Debug.Log("Open Bluetooth Manager failed: 0x" + Res.ToString("X8"));
        else
        {
            // Try to find working radio.
            IntPtr Radio = IntPtr.Zero;
            if (FManager.Count == 0)
                Debug.Log("No Bluetooth hardware was found");
            else
            {
                for (int i = 0; i < FManager.Count; i++)
                {
                    if (FManager.IsRadioAvailable(FManager[i]))
                    {
                        Radio = FManager[i];
                        break;
                    }
                }

                if (Radio == IntPtr.Zero)
                    Debug.Log("No available Bluetooth radio found");
                else
                {
                    // Start discovering for BLE devices.
                    Res = FManager.Discover(Radio, 10);
                    if (Res != BluetoothErrors.WCL_E_SUCCESS)
                        Debug.Log("Start discoverign failed: 0x" + Res.ToString("X8"));
                }
            }
        }
    }

    private void ManagerBeforeClose(object sender, EventArgs e)
    {
        Debug.Log("Bluetooth manager is closing");
    }

    private void ManagerAfterOpen(object sender, EventArgs e)
    {
        Debug.Log("Bluetooth manager has been opened"); 
    }

    private void ManagerDiscoveringStarted(System.Object sender, IntPtr Radio)
    {
        Debug.Log("Discovering has been started");
        // Do not forget to clear found devices list.
        FFoundDevices.Clear();
    }

    private void ManagerDeviceFound(System.Object sender, IntPtr Radio, Int64 Address)
    {
        // Add just found device into the list.
        Debug.Log("Device found " + Address.ToString("X12"));
        FFoundDevices.Add(Address);
    }

    private void ManagerDiscoveringCompleted(System.Object sender, IntPtr Radio, Int32 Error)
    {
        Debug.Log("Discovering completed with result: 0x" + Error.ToString("X8"));
        // Continue (connect) only if discovering completed with success.
        if (Error == BluetoothErrors.WCL_E_SUCCESS)
        {
            if (FFoundDevices.Count == 0)
                Debug.Log("No GATT devices were found");
            else
            {
                Debug.Log("Found " + FFoundDevices.Count.ToString() + " devices");
                foreach (Int64 Address in FFoundDevices)
                {
                    // Connect to only new found devices.
                    if (!FConnections.ContainsKey(Address))
                    {
                        Debug.Log("Try to connect to " + Address.ToString("X12"));

                        // Create new connection
                        GattClientThread Client = new global::GattClientThread();
                        Client.OnConnect += ClientConnect;
                        Client.OnDisconnect += ClientDisconnect;
                        Client.OnChanged += ClientChanged;
                        // Try to connect.
                        Int32 Res = Client.Connect(Radio, Address);
                        if (Res != BluetoothErrors.WCL_E_SUCCESS)
                            Debug.Log("Connect to " + Address.ToString("X12") + " failed: 0x" + Res.ToString("X8"));
                        else
                            // Add new client into the list
                            FConnections.Add(Address, Client);
                    }
                }
            }
        }
    }

    private void ClientChanged(System.Object sender, UInt16 Handle, Byte[] Value)
    {
        if (FConnections != null)
        {
            Debug.Log("Chaned");
        }
    }

    private void ClientDisconnect(System.Object sender, Int32 Reason)
    {
        Debug.Log("Client disconnected with reason: 0x" + Reason.ToString("X8"));
        if (FConnections != null)
            // Remove client from the list.
            FConnections.Remove(((GattClientThread)sender).Address);
    }

    private void ClientConnect(System.Object sender, Int32 Error)
    {
        if (FConnections != null)
        {
            GattClientThread Client = (GattClientThread)sender;
            if (Error != BluetoothErrors.WCL_E_SUCCESS)
            {
                // If connection failed remove client from list
                Debug.Log("Connect to " + Client.Address.ToString("X12") + " failed: 0x" + Error.ToString("X8"));
                FConnections.Remove(Client.Address);
            }
            else
                Debug.Log("Client " + Client.Address.ToString("X12") + " connected");
        }
    }

    void Update()
    {
        // Process APC calls.
        Helpers.ProcessApc();
    }
};
