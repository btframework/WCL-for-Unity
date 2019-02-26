using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public delegate void SoftApEvent();
public delegate void DeviceConnectedEvent(String LocalAddress, String RemoteAddress);
public delegate void DeviceDisconnectedEvent();

internal class SoftAp
{
    #region Event delegates
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void DEVICE_EVENT(IntPtr Ap, IntPtr Device);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void AP_EVENT(IntPtr Ap);
    #endregion

    private const String Dll32 = "..\\..\\..\\WfdDll\\WfdDll\\build\\WfdDll32.dll";
    private const String Dll64 = "..\\..\\..\\WfdDll\\WfdDll\\build\\WfdDll64.dll";

    #region Function imports
    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateSoftAp")]
    private static extern IntPtr CreateSoftAp32(AP_EVENT OnStarted, AP_EVENT OnStopped,
        DEVICE_EVENT OnConnected, DEVICE_EVENT OnDisconnected);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateSoftAp")]
    private static extern IntPtr CreateSoftAp64(AP_EVENT OnStarted, AP_EVENT OnStopped,
        DEVICE_EVENT OnConnected, DEVICE_EVENT OnDisconnected);

    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroySoftAp")]
    private static extern void DestroySoftAp32(IntPtr o);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroySoftAp")]
    private static extern void DestroySoftAp64(IntPtr o);

    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "StartSoftAp")]
    private static extern Int32 StartSoftAp32(IntPtr o, [MarshalAs(UnmanagedType.LPWStr)] String Ssid,
        [MarshalAs(UnmanagedType.LPWStr)] String Passphrase);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "StartSoftAp")]
    private static extern Int32 StartSoftAp64(IntPtr o, [MarshalAs(UnmanagedType.LPWStr)] String Ssid,
        [MarshalAs(UnmanagedType.LPWStr)] String Passphrase);

    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "StopSoftAp")]
    private static extern Int32 StopSoftAp32(IntPtr o);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "StopSoftAp")]
    private static extern Int32 StopSoftAp64(IntPtr o);

    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSoftApPassphrase")]
    private static extern Int32 GetSoftApPassphrase32(IntPtr o, IntPtr pPassphrase);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSoftApPassphrase")]
    private static extern Int32 GetSoftApPassphrase64(IntPtr o, IntPtr pPassphrase);

    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSoftApSsid")]
    private static extern Int32 GetSoftApSsid32(IntPtr o, IntPtr pSsid);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSoftApSsid")]
    private static extern Int32 GetSoftApSsid64(IntPtr o, IntPtr pSsid);

    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSoftApActive")]
    private static extern Boolean GetSoftApActive32(IntPtr o);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSoftApActive")]
    private static extern Boolean GetSoftApActive64(IntPtr o);

    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDeviceLocalMac")]
    private static extern void GetDeviceLocalMac32(IntPtr o, IntPtr pMac);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDeviceLocalMac")]
    private static extern void GetDeviceLocalMac64(IntPtr o, IntPtr pMac);

    [DllImport(Dll32, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDeviceRemoteMac")]
    private static extern void GetDeviceRemoteMac32(IntPtr o, IntPtr pMac);
    [DllImport(Dll64, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDeviceRemoteMac")]
    private static extern void GetDeviceRemoteMac64(IntPtr o, IntPtr pMac);
    #endregion

    #region Imported function wrappers
    private static IntPtr CreateSoftAp(AP_EVENT OnStarted, AP_EVENT OnStopped, DEVICE_EVENT OnConnected,
        DEVICE_EVENT OnDisconnected)
    {
        if (IntPtr.Size == 4)
            return CreateSoftAp32(OnStarted, OnStopped, OnConnected, OnDisconnected);
        return CreateSoftAp64(OnStarted, OnStopped, OnConnected, OnDisconnected);
    }

    private static void DestroySoftAp(IntPtr o)
    {
        if (IntPtr.Size == 4)
            DestroySoftAp32(o);
        DestroySoftAp64(o);
    }

    private static Int32 StartSoftAp(IntPtr o, String Ssid, String Passphrase)
    {
        if (IntPtr.Size == 4)
            return StartSoftAp32(o, Ssid, Passphrase);
        return StartSoftAp64(o, Ssid, Passphrase);
    }

    private static Int32 StopSoftAp(IntPtr o)
    {
        if (IntPtr.Size == 4)
            return StopSoftAp32(o);
        return StopSoftAp64(o);
    }

    private static Int32 GetSoftApPassphrase(IntPtr o, out String pPassphrase)
    {
        pPassphrase = "";

        IntPtr s = Marshal.AllocHGlobal(512);
        Int32 Res;
        if (IntPtr.Size == 4)
            Res = GetSoftApPassphrase32(o, s);
        else
            Res = GetSoftApPassphrase64(o, s);
        if (Res == 0)
            pPassphrase = Marshal.PtrToStringUni(s);
        Marshal.FreeHGlobal(s);

        return Res;
    }

    private static Int32 GetSoftApSsid(IntPtr o, out String pSsid)
    {
        pSsid = "";

        IntPtr s = Marshal.AllocHGlobal(512);
        Int32 Res;
        if (IntPtr.Size == 4)
            Res = GetSoftApSsid32(o, s);
        else
            Res = GetSoftApSsid64(o, s);
        if (Res == 0)
            pSsid = Marshal.PtrToStringUni(s);
        Marshal.FreeHGlobal(s);

        return Res;
    }

    private static Boolean GetSoftApActive(IntPtr o)
    {
        if (IntPtr.Size == 4)
            return GetSoftApActive32(o);
        return GetSoftApActive64(o);
    }

    private static void GetDeviceLocalMac(IntPtr o, out String pMac)
    {
        pMac = "";

        IntPtr s = Marshal.AllocHGlobal(512);
        if (IntPtr.Size == 4)
            GetDeviceLocalMac32(o, s);
        else
            GetDeviceLocalMac64(o, s);
        pMac = Marshal.PtrToStringUni(s);
        Marshal.FreeHGlobal(s);
    }

    private static void GetDeviceRemoteMac(IntPtr o, out String pMac)
    {
        pMac = "";

        IntPtr s = Marshal.AllocHGlobal(512);
        if (IntPtr.Size == 4)
            GetDeviceRemoteMac32(o, s);
        else
            GetDeviceRemoteMac64(o, s);
        pMac = Marshal.PtrToStringUni(s);
        Marshal.FreeHGlobal(s);
    }
    #endregion

    private IntPtr FAp;
    private DEVICE_EVENT FConnectedCb;
    private DEVICE_EVENT FDisconnectedCb;
    private AP_EVENT FStartedCb;
    private AP_EVENT FStoppedCb;

    #region Event handlers
    private void DeviceConnected(IntPtr Ap, IntPtr Device)
    {
        String LocalAddress;
        GetDeviceLocalMac(Device, out LocalAddress);
        String RemoteAddress;
        GetDeviceRemoteMac(Device, out RemoteAddress);

        DoDeviceConnected(LocalAddress, RemoteAddress);
    }

    private void DeviceDisconnected(IntPtr Ap, IntPtr Device)
    {
        DoDeviceDisconnected();
    }

    private void ApStarted(IntPtr Ap)
    {
        DoStarted();
    }

    private void ApStopped(IntPtr Ap)
    {
        DoStopped();
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

    protected virtual void DoDeviceDisconnected()
    {
        if (OnDeviceDisconnected != null)
            OnDeviceDisconnected();
    }
    #endregion

    public SoftAp()
    {
        Cleanup();

        OnStarted = null;
        OnStopped = null;
        OnDeviceConnected = null;
        OnDeviceDisconnected = null;
    }

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

    private void Ap_OnDeviceDisconnected()
    {
        Debug.Log("Device disconnected");
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
