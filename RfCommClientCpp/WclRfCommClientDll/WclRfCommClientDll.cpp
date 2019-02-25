// WclRfCommClientDll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "combaseapi.h"

// Change the lines below to provide path to the Bluetooth Framework headers.
#include "..\..\..\WCL7\CPP\Source\Bluetooth\wclBluetooth.h"
#include "..\..\..\WCL7\CPP\Source\Common\wclErrors.h"

#pragma comment(lib, "ws2_32.lib")

// Change the line below to provide path to the Bluetooth Framework lib.
#pragma comment(lib, "..\\..\\..\\WCL7\\CPP\\Lib\\2017\\x64\\wclBluetoothFramework.lib")

using namespace wclCommon;
using namespace wclCommunication;
using namespace wclBluetooth;

// Event callbacks for Bluetooth Manager.
typedef void(__stdcall *NOTIFY_EVENT)(void* sender);
typedef void(__stdcall *DEVICE_FOUND_EVENT)(void* sender, CwclBluetoothRadio* Radio, __int64 Address);
typedef void(__stdcall *DISCOVERING_STARTED_EVENT)(void* sender, CwclBluetoothRadio* Radio);
typedef void(__stdcall *DISCOVERING_COMPLETED_EVENT)(void* sender, CwclBluetoothRadio* Radio, int Error);
typedef void(__stdcall *NUMERIC_COMPARISON_EVENT)(void* sender, CwclBluetoothRadio* Radio,  __int64 Address,
	unsigned long Number);
typedef void(__stdcall *PASSKEY_NOTIFICATION_EVENT)(void* sender, CwclBluetoothRadio* Radio, __int64 Address,
	unsigned long Passkey);
typedef void(__stdcall *PASSKEY_REQUEST_EVENT)(void* sender, CwclBluetoothRadio* Radio, __int64 Address);
typedef void(__stdcall *PIN_REQUEST_EVENT)(void* sender, CwclBluetoothRadio* Radio, __int64 Address);
typedef void(__stdcall *AUTHENTICATION_COMPLETED_EVENT)(void* sender, CwclBluetoothRadio* Radio,
	__int64 Address, int Error);

// We need this wrapper to be able to setup event handlers from the c# code.
class CBluetoothManager : public CwclBluetoothManager
{
private:
	NOTIFY_EVENT					FAfterOpen;
	NOTIFY_EVENT					FBeforeClose;

	DEVICE_FOUND_EVENT				FOnDeviceFound;
	DISCOVERING_STARTED_EVENT		FOnDiscoveringStarted;
	DISCOVERING_COMPLETED_EVENT		FOnDiscoveringCompleted;

	NUMERIC_COMPARISON_EVENT		FOnNumericComparison;
	PASSKEY_NOTIFICATION_EVENT		FOnPasskeyNotification;
	PASSKEY_REQUEST_EVENT			FOnPasskeyRequest;
	PIN_REQUEST_EVENT				FOnPinRequest;
	AUTHENTICATION_COMPLETED_EVENT	FOnAuthenticationCompleted;

	BOOL			FConfirm;
	unsigned long	FPasskey;
	tstring			FPin;

protected:
	virtual void DoAfterOpen() override
	{
		if (FAfterOpen != NULL)
			FAfterOpen(this);
	}

	virtual void DoBeforeClose() override
	{
		if (FBeforeClose != NULL)
			FBeforeClose(this);
	}

	virtual void DoDeviceFound(CwclBluetoothRadio* Radio, __int64 Address) override
	{
		if (FOnDeviceFound != NULL)
			FOnDeviceFound(this, Radio, Address);
	}

	virtual void DoDiscoveringStarted(CwclBluetoothRadio* Radio) override
	{
		if (FOnDiscoveringStarted != NULL)
			FOnDiscoveringStarted(this, Radio);
	}

	virtual void DoDiscoveringCompleted(CwclBluetoothRadio* Radio, int Error) override
	{
		if (FOnDiscoveringCompleted != NULL)
			FOnDiscoveringCompleted(this, Radio, Error);
	}

	virtual void DoNumericComparison(CwclBluetoothRadio* Radio, __int64 Address,
		unsigned long Number, bool& Confirm) override
	{
		FConfirm = FALSE;
		if (FOnNumericComparison != NULL)
			FOnNumericComparison(this, Radio, Address, Number);
		Confirm = FConfirm != FALSE;
	}

	virtual void DoPasskeyNotification(CwclBluetoothRadio* Radio, __int64 Address,
		unsigned long Passkey) override
	{
		if (FOnPasskeyNotification != NULL)
			FOnPasskeyNotification(this, Radio, Address, Passkey);
	}
	
	virtual void DoPasskeyRequest(CwclBluetoothRadio* Radio, __int64 Address,
		unsigned long& Passkey) override
	{
		FPasskey = 0;
		if (FOnPasskeyRequest != NULL)
			FOnPasskeyRequest(this, Radio, Address);
		Passkey = FPasskey;
	}

	virtual void DoPinRequest(CwclBluetoothRadio* Radio, __int64 Address, tstring& Pin) override
	{
		FPin = _T("");
		if (FOnPinRequest != NULL)
			FOnPinRequest(this, Radio, Address);
		Pin = FPin;
	}

	virtual void DoAuthenticationCompleted(CwclBluetoothRadio* Radio, __int64 Address,
		int Error) override
	{
		if (FOnAuthenticationCompleted != NULL)
			FOnAuthenticationCompleted(this, Radio, Address, Error);
	}

public:
	CBluetoothManager()
		: CwclBluetoothManager()
	{
		FAfterOpen = NULL;
		FBeforeClose = NULL;

		FOnDeviceFound = NULL;
		FOnDiscoveringStarted = NULL;
		FOnDiscoveringCompleted = NULL;

		FOnNumericComparison = NULL;
		FOnPasskeyNotification = NULL;
		FOnPasskeyRequest = NULL;
		FOnPinRequest = NULL;
		FOnAuthenticationCompleted = NULL;

		FConfirm = false;
		FPasskey = 0;
		FPin = _T("");
	}

	void SetAfterOpen(NOTIFY_EVENT Event)
	{
		FAfterOpen = Event;
	}

	void SetBeforeClose(NOTIFY_EVENT Event)
	{
		FBeforeClose = Event;
	}

	void SetOnDeviceFound(DEVICE_FOUND_EVENT Event)
	{
		FOnDeviceFound = Event;
	}

	void SetOnDiscoveringStarted(DISCOVERING_STARTED_EVENT Event)
	{
		FOnDiscoveringStarted = Event;
	}

	void SetOnDiscoveringCompleted(DISCOVERING_COMPLETED_EVENT Event)
	{
		FOnDiscoveringCompleted = Event;
	}

	void SetOnNumericComparison(NUMERIC_COMPARISON_EVENT Event)
	{
		FOnNumericComparison = Event;
	}

	void SetOnPasskeyNotification(PASSKEY_NOTIFICATION_EVENT Event)
	{
		FOnPasskeyNotification = Event;
	}

	void SetOnPasskeyRequest(PASSKEY_REQUEST_EVENT Event)
	{
		FOnPasskeyRequest = Event;
	}

	void SetOnPinRequest(PIN_REQUEST_EVENT Event)
	{
		FOnPinRequest = Event;
	}

	void SetOnAuthenticationCompleted(AUTHENTICATION_COMPLETED_EVENT Event)
	{
		FOnAuthenticationCompleted = Event;
	}

	void SetConfirm(bool Confirm)
	{
		FConfirm = Confirm;
	}

	void SetPasskey(unsigned long Passkey)
	{
		FPasskey = Passkey;
	}

	void SetPin(tstring Pin)
	{
		FPin = Pin;
	}
};

// Bluetooth Manager exports.
extern "C"
{
	__declspec(dllexport) CBluetoothManager* __stdcall ManagerCreate()
	{
		return new CBluetoothManager();
	}

	__declspec(dllexport) void __stdcall ManagerDestroy(CBluetoothManager* Manager)
	{
		if (Manager != NULL)
			delete Manager;
	}

	__declspec(dllexport) int __stdcall ManagerOpen(CBluetoothManager* Manager)
	{
		if (Manager == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Manager->Open();
	}

	__declspec(dllexport) int __stdcall ManagerClose(CBluetoothManager* Manager)
	{
		if (Manager == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Manager->Close();
	}

	__declspec(dllexport) unsigned int __stdcall ManagerGetRadioCount(CBluetoothManager* Manager)
	{
		if (Manager == NULL)
			return 0;
		return (unsigned int)Manager->GetCount();
	}

	__declspec(dllexport) CwclBluetoothRadio* __stdcall ManagerGetRadio(CBluetoothManager* Manager,
		unsigned int Index)
	{
		if (Manager == NULL)
			return NULL;
		if (Index >= Manager->GetCount())
			return NULL;
		return Manager->GetRadios(Index);
	}

	__declspec(dllexport) void __stdcall ManagerSetAfterOpen(CBluetoothManager* Manager,
		NOTIFY_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetAfterOpen(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetBeforeClose(CBluetoothManager* Manager,
		NOTIFY_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetBeforeClose(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetOnDeviceFound(CBluetoothManager* Manager,
		DEVICE_FOUND_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetOnDeviceFound(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetOnDiscoveringStarted(CBluetoothManager* Manager,
		DISCOVERING_STARTED_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetOnDiscoveringStarted(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetOnDiscoveringCompleted(CBluetoothManager* Manager,
		DISCOVERING_COMPLETED_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetOnDiscoveringCompleted(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetOnNumericComparison(CBluetoothManager* Manager,
		NUMERIC_COMPARISON_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetOnNumericComparison(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetOnPasskeyNotification(CBluetoothManager* Manager,
		PASSKEY_NOTIFICATION_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetOnPasskeyNotification(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetOnPasskeyRequest(CBluetoothManager* Manager,
		PASSKEY_REQUEST_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetOnPasskeyRequest(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetOnPinRequest(CBluetoothManager* Manager,
		PIN_REQUEST_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetOnPinRequest(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetOnAuthenticationCompleted(CBluetoothManager* Manager,
		AUTHENTICATION_COMPLETED_EVENT Event)
	{
		if (Manager != NULL)
			Manager->SetOnAuthenticationCompleted(Event);
	}

	__declspec(dllexport) void __stdcall ManagerSetConfirm(CBluetoothManager* Manager, BOOL Confirm)
	{
		if (Manager != NULL)
			Manager->SetConfirm(Confirm != FALSE);
	}

	__declspec(dllexport) void __stdcall ManagerSetPasskey(CBluetoothManager* Manager, unsigned long Passkey)
	{
		if (Manager != NULL)
			Manager->SetPasskey(Passkey);
	}

	__declspec(dllexport) void __stdcall ManagerSetPin(CBluetoothManager* Manager, LPCTSTR Pin)
	{
		if (Manager != NULL)
			Manager->SetPin(Pin);
	}

	__declspec(dllexport) int __stdcall RadioDiscover(CwclBluetoothRadio* Radio, unsigned char Timeout)
	{
		if (Radio == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Radio->Discover(Timeout, dkClassic);
	}

	__declspec(dllexport) int __stdcall RadioTerminate(CwclBluetoothRadio* Radio)
	{
		if (Radio == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Radio->Terminate();
	}

	__declspec(dllexport) BOOL __stdcall RadioIsAvailable(CwclBluetoothRadio* Radio)
	{
		if (Radio == NULL)
			return FALSE;
		return Radio->GetAvailable();
	}

	__declspec(dllexport) int __stdcall RadioGetDeviceName(CwclBluetoothRadio* Radio, __int64 Address, LPTSTR Name, int Len)
	{
		if (Radio == NULL || Name == NULL || Len == 0)
			return WCL_E_INVALID_ARGUMENT;

		tstring TmpName;
		int res = Radio->GetRemoteName(Address, TmpName);

		if (res == WCL_E_SUCCESS && TmpName.length() > 0)
		{
			if (TmpName.length() > Len)
				return WCL_E_OUT_OF_MEMORY;
			_tcscpy_s(Name, Len, TmpName.c_str());
		}

		return res;
	}

	__declspec(dllexport) int __stdcall RadioPair(CwclBluetoothRadio* Radio, __int64 Address)
	{
		if (Radio == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Radio->RemotePair(Address);
	}

	__declspec(dllexport) int __stdcall RadioUnpair(CwclBluetoothRadio* Radio, __int64 Address)
	{
		if (Radio == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Radio->RemoteUnpair(Address);
	}

	__declspec(dllexport) int __stdcall RadioEnumPaired(CwclBluetoothRadio* Radio, unsigned long* Size, __int64* Addresses)
	{
		if (Radio == NULL || Size == NULL)
			return WCL_E_INVALID_ARGUMENT;
		
		wclBluetoothAddresses Devices;
		int Result = Radio->EnumPairedDevices(Devices);
		if (Result == WCL_E_SUCCESS)
		{
			if (*Size < Devices.size() && Addresses != NULL)
				Result = WCL_E_OUT_OF_MEMORY;
			*Size = (unsigned long)Devices.size();
			if (Addresses != NULL && Result == WCL_E_SUCCESS)
				memcpy(Addresses, Devices.data(), *Size * sizeof(__int64));
		}
		return Result;
	}

	__declspec(dllexport) int __stdcall RadioGetConnectable(CwclBluetoothRadio* Radio, BOOL* Connectable)
	{
		if (Radio == NULL || Connectable == NULL)
			return WCL_E_INVALID_ARGUMENT;

		bool val;
		int Res = Radio->GetConnectable(val);
		*Connectable = val;
		return Res;
	}

	__declspec(dllexport) int __stdcall RadioGetDiscoverable(CwclBluetoothRadio* Radio, BOOL* Discoverable)
	{
		if (Radio == NULL || DisableLoad == NULL)
			return WCL_E_INVALID_ARGUMENT;

		bool val;
		int Res = Radio->GetDiscoverable(val);
		*Discoverable = val;
		return Res;
	}

	__declspec(dllexport) int __stdcall RadioSetConnectable(CwclBluetoothRadio* Radio, BOOL Connectable)
	{
		if (Radio == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Radio->SetConnectable(Connectable);
	}

	__declspec(dllexport) int __stdcall RadioSetDiscoverable(CwclBluetoothRadio* Radio, BOOL Discoverable)
	{
		if (Radio == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Radio->SetDiscoverable(Discoverable);
	}
}

// RFCOMM client callbacks.
typedef void(__stdcall *CLIENT_EVENT)(void* Sender, int Error);
typedef void(__stdcall *DATA_EVENT)(void* Sender, void* Data, unsigned long Size);

// RFCOMM client wrapper. We need this one for events.
class CRfCommClient : public CwclRfCommClient
{
private:
	CLIENT_EVENT	FOnConnect;
	CLIENT_EVENT	FOnDisconnect;
	DATA_EVENT		FOnData;

protected:
	virtual void DoConnect(int Error) override
	{
		if (FOnConnect != NULL)
			FOnConnect(this, Error);
	}

	virtual void DoDisconnect(int Reason) override
	{
		if (FOnDisconnect != NULL)
			FOnDisconnect(this, Reason);
	}

	virtual void DoData(void* Data, unsigned long Size) override
	{
		if (FOnData != NULL)
			FOnData(this, Data, Size);
	}

public:
	CRfCommClient()
		: CwclRfCommClient()
	{
		FOnConnect = NULL;
		FOnDisconnect = NULL;
		FOnData = NULL;
	}

	void SetOnConnect(CLIENT_EVENT Event)
	{
		FOnConnect = Event;
	}

	void SetOnDisconnect(CLIENT_EVENT Event)
	{
		FOnDisconnect = Event;
	}

	void SetOnData(DATA_EVENT Event)
	{
		FOnData = Event;
	}
};

// RFCOMM client exports.
extern "C"
{
	__declspec(dllexport) CRfCommClient* __stdcall ClientCreate()
	{
		return new CRfCommClient();
	}

	__declspec(dllexport) void __stdcall ClientDestroy(CRfCommClient* Client)
	{
		if (Client != NULL)
			delete Client;
	}

	__declspec(dllexport) void __stdcall ClientSetOnConnect(CRfCommClient* Client, CLIENT_EVENT Event)
	{
		if (Client != NULL)
			Client->SetOnConnect(Event);
	}

	__declspec(dllexport) void __stdcall ClientSetOnDisconnect(CRfCommClient* Client, CLIENT_EVENT Event)
	{
		if (Client != NULL)
			Client->SetOnDisconnect(Event);
	}

	__declspec(dllexport) void __stdcall ClientSetOnData(CRfCommClient* Client, DATA_EVENT Event)
	{
		if (Client != NULL)
			Client->SetOnData(Event);
	}

	__declspec(dllexport) int __stdcall ClientConnect(CRfCommClient* Client, CwclBluetoothRadio* Radio)
	{
		if (Client == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Client->Connect(Radio);
	}

	__declspec(dllexport) int __stdcall ClientDisconnect(CRfCommClient* Client)
	{
		if (Client == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Client->Disconnect();
	}

	__declspec(dllexport) int __stdcall ClientGetReadBufferSize(CRfCommClient* Client,
		unsigned long* Size)
	{
		if (Client == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Client->GetReadBufferSize(*Size);
	}

	__declspec(dllexport) int __stdcall ClientGetWriteBufferSize(CRfCommClient* Client,
		unsigned long* Size)
	{
		if (Client == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Client->GetWriteBufferSize(*Size);
	}

	__declspec(dllexport) int __stdcall ClientSetReadBufferSize(CRfCommClient* Client,
		unsigned long Size)
	{
		if (Client == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Client->SetReadBufferSize(Size);
	}

	__declspec(dllexport) int __stdcall ClientSetWriteBufferSize(CRfCommClient* Client,
		unsigned long Size)
	{
		if (Client == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Client->SetWriteBufferSize(Size);
	}

	__declspec(dllexport) int __stdcall ClientWrite(CRfCommClient* Client,
		void* Data, unsigned long Size, unsigned long* Written)
	{
		if (Client == NULL)
			return WCL_E_INVALID_ARGUMENT;
		return Client->Write(Data, Size, *Written);
	}

	__declspec(dllexport) CwclBluetoothRadio* __stdcall ClientGetRadio(CRfCommClient* Client)
	{
		if (Client == NULL)
			return NULL;
		return Client->GetRadio();
	}

	__declspec(dllexport) GUID __stdcall ClientGetService(CRfCommClient* Client)
	{
		if (Client == NULL)
			return GUID_NULL;
		return Client->GetService();
	}

	__declspec(dllexport) void __stdcall ClientSetService(CRfCommClient* Client, GUID Value)
	{
		if (Client != NULL)
			Client->SetService(Value);
	}

	__declspec(dllexport) BOOL __stdcall ClientGetConnected(CRfCommClient* Client)
	{
		if (Client == NULL)
			return FALSE;
		return Client->GetState() != csDisconnected;
	}

	__declspec(dllexport) __int64 __stdcall ClientGetAddress(CRfCommClient* Client)
	{
		if (Client == NULL)
			return 0;
		return Client->GetAddress();
	}

	__declspec(dllexport) void __stdcall ClientSetAddress(CRfCommClient* Client, __int64 Value)
	{
		if (Client != NULL)
			Client->SetAddress(Value);
	}

	__declspec(dllexport) BOOL __stdcall ClientGetAuthentication(CRfCommClient* Client)
	{
		if (Client == NULL)
			return FALSE;
		return Client->GetAuthentication();
	}

	__declspec(dllexport) void __stdcall ClientSetAuthentication(CRfCommClient* Client, BOOL Value)
	{
		if (Client != NULL)
			Client->SetAuthentication(Value != FALSE);
	}

	__declspec(dllexport) unsigned char __stdcall ClientGetChannel(CRfCommClient* Client)
	{
		if (Client == NULL)
			return 0;
		return Client->GetChannel();
	}

	__declspec(dllexport) void __stdcall ClientSetChannel(CRfCommClient* Client, unsigned char Value)
	{
		if (Client != NULL)
			Client->SetChannel(Value);
	}

	__declspec(dllexport) BOOL __stdcall ClientGetEncryption(CRfCommClient* Client)
	{
		if (Client == NULL)
			return FALSE;
		return Client->GetEncryption();
	}

	__declspec(dllexport) void __stdcall ClientSetEncryption(CRfCommClient* Client, BOOL Value)
	{
		if (Client != NULL)
			Client->SetEncryption(Value != FALSE);
	}

	__declspec(dllexport) unsigned long __stdcall ClientGetTimeout(CRfCommClient* Client)
	{
		if (Client == NULL)
			return 0;
		return Client->GetTimeout();
	}

	__declspec(dllexport) void __stdcall ClientSetTimeout(CRfCommClient* Client, unsigned long Value)
	{
		if (Client != NULL)
			Client->SetTimeout(Value);
	}
}