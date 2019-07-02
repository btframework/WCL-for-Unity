using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public delegate void SoftApEvent();
public delegate void DeviceConnectedEvent(String LocalAddress, String RemoteAddress);
public delegate void DeviceDisconnectedEvent(String LocalAddress, String RemoteAddress);

internal class SoftAp
{
    #region Event delegates
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void DEVICE_EVENT(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr Ap,
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr Device);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void AP_EVENT(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr Ap);
    #endregion

    private const String DllName = "..\\..\\..\\WfdDll\\WfdDll\\build\\WfdDll64.dll";

    #region Function imports
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.SysInt)]
    private static extern IntPtr CreateSoftAp(
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] AP_EVENT OnStarted,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] AP_EVENT OnStopped,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] DEVICE_EVENT OnConnected,
        [param: MarshalAs(UnmanagedType.FunctionPtr), In] DEVICE_EVENT OnDisconnected);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void DestroySoftAp(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 StartSoftAp(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o,
        [MarshalAs(UnmanagedType.LPWStr)] String Ssid,
        [MarshalAs(UnmanagedType.LPWStr)] String Passphrase);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 StopSoftAp(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern Boolean GetSoftApActive(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 DisconnectDevice(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o);

    #region GetSoftApPassphrase
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GetSoftApPassphrase(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o,
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr pPassphrase);

    private static Int32 GetSoftApPassphrase(IntPtr o, out String pPassphrase)
    {
        pPassphrase = "";
        IntPtr s = Marshal.AllocHGlobal(512);
        Int32 Res;
        try
        {
            Res = GetSoftApPassphrase(o, s);
            if (Res == 0)
                pPassphrase = Marshal.PtrToStringUni(s);
        }
        finally
        {
            Marshal.FreeHGlobal(s);
        }
        return Res;
    }
    #endregion

    #region GetSoftApSsid
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern Int32 GetSoftApSsid(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o,
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr pSsid);

    private static Int32 GetSoftApSsid(IntPtr o, out String pSsid)
    {
        pSsid = "";
        IntPtr s = Marshal.AllocHGlobal(512);
        Int32 Res;
        try
        {
            Res = GetSoftApSsid(o, s);
            if (Res == 0)
                pSsid = Marshal.PtrToStringUni(s);
        }
        finally
        {
            Marshal.FreeHGlobal(s);
        }
        return Res;
    }
    #endregion

    #region GetDeviceLocalMac
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void GetDeviceLocalMac(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o,
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr pMac);

    private static void GetDeviceLocalMac(IntPtr o, out String pMac)
    {
        pMac = "";
        IntPtr s = Marshal.AllocHGlobal(512);
        try
        {
            GetDeviceLocalMac(o, s);
            pMac = Marshal.PtrToStringUni(s);
        }
        finally
        {
            Marshal.FreeHGlobal(s);
        }
    }
    #endregion

    #region GetDeviceRemoteMac
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    private static extern void GetDeviceRemoteMac(
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr o,
        [MarshalAs(UnmanagedType.SysInt), In] IntPtr pMac);

    private static void GetDeviceRemoteMac(IntPtr o, out String pMac)
    {
        pMac = "";
        IntPtr s = Marshal.AllocHGlobal(512);
        try
        {
            GetDeviceRemoteMac(o, s);
            pMac = Marshal.PtrToStringUni(s);
        }
        finally
        {
            Marshal.FreeHGlobal(s);
        }
    }
    #endregion
    #endregion

    private IntPtr FAp;
    private DEVICE_EVENT FConnectedCb;
    private DEVICE_EVENT FDisconnectedCb;
    private AP_EVENT FStartedCb;
    private AP_EVENT FStoppedCb;
    private List<IntPtr> FDevices;

    #region Event handlers
    private void DeviceConnected(IntPtr Ap, IntPtr Device)
    {
        String LocalAddress;
        GetDeviceLocalMac(Device, out LocalAddress);
        String RemoteAddress;
        GetDeviceRemoteMac(Device, out RemoteAddress);
        FDevices.Add(Device);

        DoDeviceConnected(LocalAddress, RemoteAddress);
    }

    private void DeviceDisconnected(IntPtr Ap, IntPtr Device)
    {
        String LocalAddress;
        GetDeviceLocalMac(Device, out LocalAddress);
        String RemoteAddress;
        GetDeviceRemoteMac(Device, out RemoteAddress);
        FDevices.Remove(Device);

        DoDeviceDisconnected(LocalAddress, RemoteAddress);
    }

    private void ApStarted(IntPtr Ap)
    {
        FDevices = new List<IntPtr>();

        DoStarted();
    }

    private void ApStopped(IntPtr Ap)
    {
        DoStopped();

        FDevices.Clear();
        FDevices = null;
    }
    #endregion

    private void Cleanup()
    {
        FAp = IntPtr.Zero;
        FConnectedCb = null;
        FDisconnectedCb = null;
        FStartedCb = null;
        FStoppedCb = null;
    }

    #region Event control functions
    protected virtual void DoStarted()
    {
        if (OnStarted != null)
            OnStarted();
    }

    protected virtual void DoStopped()
    {
        if (OnStopped != null)
            OnStopped();
    }

    protected virtual void DoDeviceConnected(String LoadAddres, String RemoteAddress)
    {
        if (OnDeviceConnected != null)
            OnDeviceConnected(LoadAddres, RemoteAddress);
    }

    protected virtual void DoDeviceDisconnected(String LoadAddres, String RemoteAddress)
    {
        if (OnDeviceDisconnected != null)
            OnDeviceDisconnected(LoadAddres, RemoteAddress);
    }
    #endregion

    public SoftAp()
    {
        Cleanup();

        FDevices = null;

        OnStarted = null;
        OnStopped = null;
        OnDeviceConnected = null;
        OnDeviceDisconnected = null;
    }

    #region Methods
    public void Start()
    {
        if (FAp == IntPtr.Zero)
        {
            FConnectedCb = new DEVICE_EVENT(DeviceConnected);
            FDisconnectedCb = new DEVICE_EVENT(DeviceDisconnected);
            FStartedCb = new AP_EVENT(ApStarted);
            FStoppedCb = new AP_EVENT(ApStopped);

            FAp = CreateSoftAp(FStartedCb, FStoppedCb, FConnectedCb, FDisconnectedCb);
            if (FAp == IntPtr.Zero)
            {
                Cleanup();
                Debug.Log("Unable create WiFi Direct Soft AP");
            }
            else
            {
                Int32 Res = StartSoftAp(FAp, "btframework", "12345678");
                if (Res != 0)
                {
                    DestroySoftAp(FAp);
                    Cleanup();
                    Debug.Log("Failed to start WiFi Direct Soft AP: 0x" + Res.ToString("X8"));
                }
            }
        }
        else
            Debug.Log("Already started");
    }

    public void Stop()
    {
        if (FAp == IntPtr.Zero)
            Debug.Log("Not created");
        else
        {
            Int32 Res = StopSoftAp(FAp);
            if (Res != 0)
                Debug.Log("Failed to stop Soft AP: 0x" + Res.ToString("X8"));
            else
            {
                DestroySoftAp(FAp);
                Cleanup();
            }
        }
    }

    public Boolean Disconnect(String RemoteMac)
    {
        if (FDevices == null)
            return false;
        foreach (IntPtr o in FDevices)
        {
            String Mac;
            GetDeviceRemoteMac(o, out Mac);
            if (Mac == RemoteMac)
                return (DisconnectDevice(o) == 0);
        }
        return false;
    }
    #endregion

    #region Properties
    public String Ssid
    {
        get
        {
            String s;
            Int32 Res = GetSoftApSsid(FAp, out s);
            if (Res != 0)
                return "  Failed to get SSID: 0x" + Res.ToString("X8");
            return s;
        }
    }

    public String Passphrase
    {
        get
        {
            String s;
            Int32 Res = GetSoftApPassphrase(FAp, out s);
            if (Res != 0)
                return "  Failed to get PASSPHRASE: 0x" + Res.ToString("X8");
            return s;
        }
    }

    public Boolean Active
    {
        get
        {
            return GetSoftApActive(FAp);
        }
    }
    #endregion

    #region Public events
    public event SoftApEvent OnStarted;
    public event SoftApEvent OnStopped;
    public event DeviceConnectedEvent OnDeviceConnected;
    public event DeviceDisconnectedEvent OnDeviceDisconnected;
    #endregion
};

public class NewBehaviourScript : MonoBehaviour
{
    private SoftAp Ap;

	// Use this for initialization
	void Start()
    {
        Debug.Log("Scrip started");

        Ap = new SoftAp();
        Ap.OnStarted += Ap_OnStarted;
        Ap.OnStopped += Ap_OnStopped;
        Ap.OnDeviceConnected += Ap_OnDeviceConnected;
        Ap.OnDeviceDisconnected += Ap_OnDeviceDisconnected;

        Ap.Start();
	}

    private void Ap_OnDeviceDisconnected(string LocalAddress, string RemoteAddress)
    {
        Debug.Log("Device disconnected");
        Debug.Log("  Local Address: " + LocalAddress);
        Debug.Log("  Remote Address: " + RemoteAddress);
    }

    private void Ap_OnDeviceConnected(string LocalAddress, string RemoteAddress)
    {
        Debug.Log("Device connected");
        Debug.Log("  Local Address: " + LocalAddress);
        Debug.Log("  Remote Address: " + RemoteAddress);
    }

    private void Ap_OnStopped()
    {
        Debug.Log("AP stopped");
    }

    private void Ap_OnStarted()
    {
        Debug.Log("AP started");
        Debug.Log("  SSID: " + Ap.Ssid);
        Debug.Log("  PASSPHRASE: " + Ap.Passphrase);
    }

    // Update is called once per frame
    void Update()
    {
		
	}

    private void OnDestroy()
    {
        Ap.Stop();
        Ap = null;
    }
}
