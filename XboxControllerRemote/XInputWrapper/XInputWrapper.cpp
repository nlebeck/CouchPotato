// This is the main DLL file.

#include "stdafx.h"

#include "XInputWrapper.h"

#pragma comment(lib, "User32.lib")
#pragma comment(lib, "Xinput.lib")

namespace XInputWrapper {

	DWORD XInputState::XInputGetStateWrapper(DWORD dwUserIndex, XInputState% state) {
		XINPUT_STATE rawState;
		DWORD ret = XInputGetState(dwUserIndex, &rawState);
		state.dwPacketNumber = rawState.dwPacketNumber;
		state.Gamepad.bLeftTrigger = rawState.Gamepad.bLeftTrigger;
		state.Gamepad.bRightTrigger = rawState.Gamepad.bRightTrigger;
		state.Gamepad.sThumbLX = rawState.Gamepad.sThumbLX;
		state.Gamepad.sThumbLY = rawState.Gamepad.sThumbLY;
		state.Gamepad.sThumbRX = rawState.Gamepad.sThumbRX;
		state.Gamepad.sThumbRY = rawState.Gamepad.sThumbRY;
		state.Gamepad.wButtons = rawState.Gamepad.wButtons;
		return ret;
	}

	void MouseEventWrapper::MouseEvent(DWORD dwFlags, DWORD dx, DWORD dy, DWORD dwData, ULONG_PTR dwExtraInfo) {
		mouse_event(dwFlags, dx, dy, dwData, dwExtraInfo);
	}

}
